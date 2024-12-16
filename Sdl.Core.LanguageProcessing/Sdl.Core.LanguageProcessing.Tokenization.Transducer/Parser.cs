using System;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	internal class Parser
	{
		private enum Symbol
		{
			EOF,
			Char,
			LParen,
			RParen,
			LAngle,
			RAngle,
			OpDisj,
			OpKleene,
			OpPlus,
			OpOpt,
			Colon,
			AnchorBOL,
			AnchorEOL,
			AnchorBOW,
			AnchorEOW,
			SpecialDigit,
			SpecialWhitespace,
			None
		}

		private Symbol _currentSymbol;

		private char _currentChar;

		private string _input;

		private int _inputPosition;

		private int _inputLength;

		public FST Parse(string expression)
		{
			_input = expression;
			_inputLength = expression.Length;
			_inputPosition = 0;
			Scan(insideTransducerSymbol: false);
			if (_currentSymbol == Symbol.EOF)
			{
				throw new Exception("Empty input expression");
			}
			Node node = ParseRX();
			if (_currentSymbol != 0)
			{
				throw new Exception("Trailing garbage");
			}
			return node.GetFst();
		}

		private Node ParseRX()
		{
			Node node = ParseTerm();
			if (_currentSymbol != Symbol.OpDisj)
			{
				return node;
			}
			DisjunctionNode disjunctionNode = new DisjunctionNode();
			disjunctionNode.Add(node);
			node = disjunctionNode;
			while (_currentSymbol == Symbol.OpDisj)
			{
				Scan(insideTransducerSymbol: false);
				Node node2 = ParseTerm();
				if (node2 == null)
				{
					throw new Exception("Premature End of Input");
				}
				disjunctionNode.Add(node2);
			}
			return node;
		}

		private Node ParseTerm()
		{
			Node node = ParseFactor();
			if (_currentSymbol != Symbol.LParen && _currentSymbol != Symbol.Char && _currentSymbol != Symbol.SpecialDigit && _currentSymbol != Symbol.SpecialWhitespace && _currentSymbol != Symbol.AnchorBOL && _currentSymbol != Symbol.AnchorEOL && _currentSymbol != Symbol.AnchorBOW && _currentSymbol != Symbol.AnchorEOW && _currentSymbol != Symbol.LAngle)
			{
				return node;
			}
			SequenceNode sequenceNode = new SequenceNode();
			sequenceNode.Add(node);
			node = sequenceNode;
			while (_currentSymbol == Symbol.LParen || _currentSymbol == Symbol.Char || _currentSymbol == Symbol.SpecialDigit || _currentSymbol == Symbol.SpecialWhitespace || _currentSymbol == Symbol.AnchorBOL || _currentSymbol == Symbol.AnchorEOL || _currentSymbol == Symbol.AnchorBOW || _currentSymbol == Symbol.AnchorEOW || _currentSymbol == Symbol.LAngle)
			{
				Node node2 = ParseFactor();
				if (node2 == null)
				{
					throw new Exception("Premature end of input");
				}
				sequenceNode.Add(node2);
			}
			return node;
		}

		private Node ParseFactor()
		{
			Node node = ParseSingle();
			while (_currentSymbol == Symbol.OpKleene || _currentSymbol == Symbol.OpPlus || _currentSymbol == Symbol.OpOpt)
			{
				switch (_currentSymbol)
				{
				case Symbol.OpKleene:
				{
					Scan(insideTransducerSymbol: false);
					RepetitiveNode repetitiveNode3 = new RepetitiveNode(0, -1)
					{
						Content = node
					};
					node = repetitiveNode3;
					break;
				}
				case Symbol.OpPlus:
				{
					Scan(insideTransducerSymbol: false);
					RepetitiveNode repetitiveNode2 = new RepetitiveNode(1, -1)
					{
						Content = node
					};
					node = repetitiveNode2;
					break;
				}
				case Symbol.OpOpt:
				{
					Scan(insideTransducerSymbol: false);
					RepetitiveNode repetitiveNode = new RepetitiveNode(0, 1)
					{
						Content = node
					};
					node = repetitiveNode;
					break;
				}
				}
			}
			return node;
		}

		private void Expect(Symbol sym, bool insideTransducerSymbol = false)
		{
			if (_currentSymbol != sym)
			{
				throw new Exception("Expected symbol " + sym.ToString());
			}
			Scan(insideTransducerSymbol);
		}

		private Node ParseSingle()
		{
			switch (_currentSymbol)
			{
			case Symbol.EOF:
				throw new Exception("Premature end of input");
			case Symbol.LParen:
			{
				Scan(insideTransducerSymbol: false);
				Node result = ParseRX();
				Expect(Symbol.RParen);
				return result;
			}
			case Symbol.LAngle:
			{
				Scan(insideTransducerSymbol: true);
				Label input = (_currentSymbol == Symbol.Colon) ? new Label(-1) : ParseSymbol(insideTransducerSymbol: true);
				Expect(Symbol.Colon, insideTransducerSymbol: true);
				Label output = (_currentSymbol == Symbol.RAngle) ? new Label(-1) : ParseSymbol(insideTransducerSymbol: true);
				Expect(Symbol.RAngle);
				return new TransitionNode(input, output);
			}
			default:
			{
				Label label = ParseSymbol(insideTransducerSymbol: false);
				if (label == null)
				{
					throw new Exception("Invalid transducer symbol syntax");
				}
				return new TransitionNode(label, new Label(label));
			}
			}
		}

		private Label ParseSymbol(bool insideTransducerSymbol)
		{
			switch (_currentSymbol)
			{
			case Symbol.EOF:
				throw new Exception("Premature end of input");
			case Symbol.Char:
			{
				Label result = new Label(_currentChar);
				Scan(insideTransducerSymbol);
				return result;
			}
			default:
				switch (_currentSymbol)
				{
				case Symbol.SpecialDigit:
					Scan(insideTransducerSymbol: false);
					return new Label(-8);
				case Symbol.SpecialWhitespace:
					Scan(insideTransducerSymbol: false);
					return new Label(-7);
				case Symbol.AnchorBOL:
					Scan(insideTransducerSymbol: false);
					return new Label(-5);
				case Symbol.AnchorEOL:
					Scan(insideTransducerSymbol: false);
					return new Label(-6);
				case Symbol.AnchorBOW:
					Scan(insideTransducerSymbol: false);
					return new Label(-3);
				case Symbol.AnchorEOW:
					Scan(insideTransducerSymbol: false);
					return new Label(-4);
				default:
					return null;
				}
			}
		}

		private void SetCurrentChar(char c)
		{
			_currentSymbol = Symbol.Char;
			_currentChar = c;
		}

		private void Scan(bool insideTransducerSymbol)
		{
			if (_inputPosition >= _inputLength)
			{
				_currentSymbol = Symbol.EOF;
				return;
			}
			ReadUnicodeEscape();
			if (_currentSymbol == Symbol.Char)
			{
				return;
			}
			char c = CharAt(_inputPosition);
			_inputPosition++;
			switch (c)
			{
			case 'U':
			case 'u':
			{
				int hexValue;
				int hexValue2;
				int hexValue3;
				int hexValue4;
				if (_inputPosition + 4 < _inputPosition && CharAt(_inputPosition) == '+' && (hexValue = GetHexValue(CharAt(_inputPosition + 1))) >= 0 && (hexValue2 = GetHexValue(CharAt(_inputPosition + 2))) >= 0 && (hexValue3 = GetHexValue(CharAt(_inputPosition + 3))) >= 0 && (hexValue4 = GetHexValue(CharAt(_inputPosition + 4))) >= 0)
				{
					_inputPosition += 5;
					_currentSymbol = Symbol.Char;
					_currentChar = (char)(hexValue * 4096 + hexValue2 * 256 + hexValue3 * 16 + hexValue4);
				}
				else
				{
					_currentSymbol = Symbol.Char;
					_currentChar = c;
				}
				break;
			}
			case '\\':
				if (_inputPosition < _inputLength)
				{
					_currentChar = CharAt(_inputPosition);
					switch (_currentChar)
					{
					case 's':
						_currentSymbol = Symbol.SpecialWhitespace;
						break;
					case 'd':
						_currentSymbol = Symbol.SpecialDigit;
						break;
					default:
						_currentSymbol = Symbol.Char;
						break;
					}
					_inputPosition++;
					break;
				}
				throw new Exception("Trailing backslash");
			case '(':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.LParen;
				}
				break;
			case ')':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.RParen;
				}
				break;
			case '<':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.LAngle;
				}
				break;
			case '>':
				_currentSymbol = Symbol.RAngle;
				break;
			case ':':
				_currentSymbol = Symbol.Colon;
				break;
			case '|':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.OpDisj;
				}
				break;
			case '*':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.OpKleene;
				}
				break;
			case '+':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.OpPlus;
				}
				break;
			case '?':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.OpOpt;
				}
				break;
			case '^':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.AnchorBOL;
				}
				break;
			case '$':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
				}
				else
				{
					_currentSymbol = Symbol.AnchorEOL;
				}
				break;
			case '#':
				if (insideTransducerSymbol)
				{
					SetCurrentChar(c);
					break;
				}
				_currentSymbol = Symbol.Char;
				_currentChar = c;
				if (_inputPosition < _inputLength)
				{
					switch (_input[_inputPosition])
					{
					case '<':
						_currentSymbol = Symbol.AnchorBOW;
						_inputPosition++;
						break;
					case '>':
						_currentSymbol = Symbol.AnchorEOW;
						_inputPosition++;
						break;
					}
				}
				break;
			default:
				SetCurrentChar(c);
				break;
			}
		}

		private static int GetHexValue(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - 48;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 97 + 10;
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 65 + 10;
			}
			return -1;
		}

		private void ReadUnicodeEscape()
		{
			if (_inputPosition + 5 >= _inputLength)
			{
				_currentSymbol = Symbol.None;
				return;
			}
			if (_input[_inputPosition] != '\\' || _input[_inputPosition + 1] != 'u')
			{
				_currentSymbol = Symbol.None;
				return;
			}
			string s = _input.Substring(_inputPosition + 2, 4);
			if (ushort.TryParse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ushort result))
			{
				char currentChar = Convert.ToChar(result);
				_inputPosition += 6;
				_currentSymbol = Symbol.Char;
				_currentChar = currentChar;
			}
			else
			{
				_currentSymbol = Symbol.None;
			}
		}

		private char CharAt(int position)
		{
			if (position >= _inputLength)
			{
				return '\0';
			}
			return _input[position];
		}
	}
}
