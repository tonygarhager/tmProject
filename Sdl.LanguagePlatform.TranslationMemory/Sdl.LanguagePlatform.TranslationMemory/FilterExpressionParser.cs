using Sdl.LanguagePlatform.Core;
using System;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class FilterExpressionParser
	{
		private enum Symbol
		{
			Eof,
			StringValue,
			IntValue,
			OperatorEqual,
			OperatorSmaller,
			OperatorSmallerEqual,
			OperatorGreater,
			OperatorGreaterEqual,
			OperatorNotEqual,
			OperatorContains,
			OperatorContainsNot,
			OperatorMatches,
			OperatorMatchesNot,
			LogopAnd,
			LogopOr,
			LogopNot,
			LeftParenthesis,
			RightParenthesis,
			Comma
		}

		private Symbol _currentSymbol;

		private string _input;

		private string _originalInput;

		private int _inputPosition;

		private int _inputLength;

		private readonly IFieldDefinitions _fieldDeclarations;

		private int _intValue;

		private string _stringValue;

		public static FilterExpression Parse(string expression, IFieldDefinitions fieldDeclarations)
		{
			return new FilterExpressionParser(fieldDeclarations).Parse(expression);
		}

		public FilterExpressionParser(IFieldDefinitions fieldDeclarations)
		{
			_fieldDeclarations = fieldDeclarations;
		}

		public FilterExpression Parse(string s)
		{
			_input = s + "\0";
			_inputPosition = 0;
			_inputLength = _input.Length;
			_originalInput = s;
			_currentSymbol = Scan();
			FilterExpression result = ParseExpression();
			if (_currentSymbol != 0)
			{
				throw GenerateException(ErrorCode.ErrorFilterTrailingJunk);
			}
			return result;
		}

		private Exception GenerateException(ErrorCode code)
		{
			return new LanguagePlatformException(new FaultDescription(ErrorCode.ErrorInFilterExpression, FaultStatus.Error)
			{
				Message = string.Format(FaultDescription.GetDescriptionFromErrorCode(ErrorCode.ErrorInFilterExpression), code.ToString(), _inputPosition - 1, _originalInput)
			});
		}

		private FilterExpression ParseExpression()
		{
			FilterExpression filterExpression = ParseTerm();
			while (_currentSymbol == Symbol.LogopOr)
			{
				_currentSymbol = Scan();
				FilterExpression rightOperand = ParseTerm();
				filterExpression = new ComposedExpression(filterExpression, ComposedExpression.Operator.Or, rightOperand);
			}
			return filterExpression;
		}

		private FilterExpression ParseTerm()
		{
			FilterExpression filterExpression = ParseFactor();
			while (_currentSymbol == Symbol.LogopAnd)
			{
				_currentSymbol = Scan();
				FilterExpression rightOperand = ParseFactor();
				filterExpression = new ComposedExpression(filterExpression, ComposedExpression.Operator.And, rightOperand);
			}
			return filterExpression;
		}

		private FilterExpression ParseFactor()
		{
			if (_currentSymbol != Symbol.LogopNot)
			{
				return ParseAtomicExpression();
			}
			_currentSymbol = Scan();
			FilterExpression rightOperand = ParseAtomicExpression();
			return new ComposedExpression(null, ComposedExpression.Operator.Not, rightOperand);
		}

		private FilterExpression ParseAtomicExpression()
		{
			if (_currentSymbol == Symbol.LeftParenthesis)
			{
				_currentSymbol = Scan();
				FilterExpression result = ParseExpression();
				if (_currentSymbol != Symbol.RightParenthesis)
				{
					throw GenerateException(ErrorCode.ErrorFilterUnbalancedParenthesis);
				}
				_currentSymbol = Scan();
				return result;
			}
			if (_currentSymbol != Symbol.StringValue)
			{
				throw GenerateException(ErrorCode.ErrorFilterExpectFieldName);
			}
			string text;
			if (_stringValue[0] == '"')
			{
				if (!_stringValue.EndsWith("\"") || _stringValue.Length < 2)
				{
					throw GenerateException(ErrorCode.ErrorFilterInvalidFieldName);
				}
				text = _stringValue.Substring(1, _stringValue.Length - 2);
			}
			else
			{
				text = _stringValue;
			}
			if (text.Length == 0)
			{
				throw GenerateException(ErrorCode.ErrorFilterEmptyFieldName);
			}
			FieldValueType fieldValueType = TranslationUnit.GetSystemFieldType(text);
			IField field = null;
			switch (fieldValueType)
			{
			case FieldValueType.SinglePicklist:
				field = Field.LookupSpecialField(text);
				break;
			case FieldValueType.Unknown:
				if (_fieldDeclarations != null)
				{
					field = _fieldDeclarations.LookupIField(text);
					if (field != null)
					{
						fieldValueType = field.ValueType;
					}
				}
				break;
			}
			if (fieldValueType == FieldValueType.Unknown)
			{
				throw GenerateException(ErrorCode.ErrorFilterUnknownField);
			}
			_currentSymbol = Scan();
			AtomicExpression.Operator op;
			switch (_currentSymbol)
			{
			case Symbol.Eof:
				throw GenerateException(ErrorCode.ErrorFilterPrematureEndOfInput);
			case Symbol.OperatorEqual:
				op = AtomicExpression.Operator.Equal;
				break;
			case Symbol.OperatorSmaller:
				op = AtomicExpression.Operator.Smaller;
				break;
			case Symbol.OperatorSmallerEqual:
				op = AtomicExpression.Operator.SmallerEqual;
				break;
			case Symbol.OperatorGreater:
				op = AtomicExpression.Operator.Greater;
				break;
			case Symbol.OperatorGreaterEqual:
				op = AtomicExpression.Operator.GreaterEqual;
				break;
			case Symbol.OperatorNotEqual:
				op = AtomicExpression.Operator.NotEqual;
				break;
			case Symbol.OperatorContains:
				op = AtomicExpression.Operator.Contains;
				break;
			case Symbol.OperatorContainsNot:
				op = AtomicExpression.Operator.ContainsNot;
				break;
			case Symbol.OperatorMatches:
				op = AtomicExpression.Operator.Matches;
				break;
			case Symbol.OperatorMatchesNot:
				op = AtomicExpression.Operator.MatchesNot;
				break;
			default:
				throw GenerateException(ErrorCode.ErrorFilterOperatorExpected);
			}
			_currentSymbol = Scan();
			FieldValue fieldValue;
			switch (fieldValueType)
			{
			case FieldValueType.MultipleString:
				fieldValue = new MultipleStringFieldValue(text);
				goto IL_01cf;
			case FieldValueType.MultiplePicklist:
				fieldValue = new MultiplePicklistFieldValue(text);
				goto IL_01cf;
			case FieldValueType.Integer:
				if (_currentSymbol != Symbol.IntValue)
				{
					throw GenerateException(ErrorCode.ErrorFilterFieldRequiresIntegerValue);
				}
				fieldValue = new IntFieldValue(text, _intValue);
				_currentSymbol = Scan();
				break;
			default:
				{
					if (_currentSymbol != Symbol.StringValue)
					{
						throw GenerateException(ErrorCode.ErrorFilterValueNotSurroundedByDoubleQuotes);
					}
					switch (fieldValueType)
					{
					case FieldValueType.SingleString:
						fieldValue = new SingleStringFieldValue(text, _stringValue);
						break;
					case FieldValueType.DateTime:
					{
						if (!DateTimeUtilities.TryParseWithFallback(_stringValue, out DateTime result2))
						{
							throw GenerateException(ErrorCode.ErrorFilterCannotParseDate);
						}
						fieldValue = new DateTimeFieldValue(text, result2);
						break;
					}
					case FieldValueType.SinglePicklist:
						if (!IsPicklistField(field))
						{
							throw GenerateException(ErrorCode.ErrorFilterNotAPicklistField);
						}
						fieldValue = new SinglePicklistFieldValue(text, new PicklistItem(_stringValue));
						break;
					default:
						throw GenerateException(ErrorCode.ErrorInFilterExpression);
					}
					_currentSymbol = Scan();
					break;
				}
				IL_01cf:
				switch (_currentSymbol)
				{
				case Symbol.StringValue:
					if (fieldValueType == FieldValueType.MultipleString)
					{
						((MultipleStringFieldValue)fieldValue).Add(_stringValue);
						break;
					}
					if (!IsPicklistField(field))
					{
						throw GenerateException(ErrorCode.ErrorFilterNotAPicklistField);
					}
					((MultiplePicklistFieldValue)fieldValue).Add(new PicklistItem(_stringValue));
					break;
				case Symbol.LeftParenthesis:
					_currentSymbol = Scan();
					do
					{
						if (_currentSymbol != Symbol.StringValue)
						{
							throw GenerateException(ErrorCode.ErrorFilterValueNotSurroundedByDoubleQuotes);
						}
						if (fieldValueType == FieldValueType.MultipleString)
						{
							((MultipleStringFieldValue)fieldValue).Add(_stringValue);
						}
						else
						{
							if (!IsPicklistField(field))
							{
								throw GenerateException(ErrorCode.ErrorFilterNotAPicklistField);
							}
							((MultiplePicklistFieldValue)fieldValue).Add(new PicklistItem(_stringValue));
						}
						_currentSymbol = Scan();
						if (_currentSymbol == Symbol.Comma)
						{
							_currentSymbol = Scan();
						}
					}
					while (_currentSymbol == Symbol.StringValue);
					if (_currentSymbol != Symbol.RightParenthesis)
					{
						throw GenerateException(ErrorCode.ErrorFilterMultipleValuesNotBracketed);
					}
					break;
				default:
					throw GenerateException(ErrorCode.ErrorFilterMultivalueFieldNotBracketed);
				}
				_currentSymbol = Scan();
				break;
			}
			if (fieldValue == null)
			{
				throw GenerateException(ErrorCode.ErrorInFilterExpression);
			}
			return new AtomicExpression(fieldValue, op);
		}

		private bool FieldContainsPicklistItem(IField field, string picklistItemName)
		{
			return field.PicklistItemNames.FirstOrDefault((string name) => name.Equals(picklistItemName, StringComparison.OrdinalIgnoreCase)) != null;
		}

		private static bool IsPicklistField(IField field)
		{
			if (field.ValueType != FieldValueType.SinglePicklist)
			{
				return field.ValueType == FieldValueType.MultiplePicklist;
			}
			return true;
		}

		private Symbol Scan()
		{
			if (_inputPosition >= _inputLength)
			{
				return Symbol.Eof;
			}
			char c = _input[_inputPosition++];
			if (c == '\0')
			{
				return Symbol.Eof;
			}
			while (CharacterProperties.IsWhitespace(c))
			{
				c = _input[_inputPosition++];
			}
			switch (c)
			{
			case '\0':
				return Symbol.Eof;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				_intValue = c - 48;
				while ((c = _input[_inputPosition]) >= '0' && c <= '9')
				{
					_intValue = _intValue * 10 + c - 48;
					_inputPosition++;
				}
				return Symbol.IntValue;
			default:
				if (char.IsLetter(c))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(c);
					while (char.IsLetterOrDigit(c = _input[_inputPosition]))
					{
						stringBuilder.Append(c);
						_inputPosition++;
					}
					_stringValue = stringBuilder.ToString();
					return Symbol.StringValue;
				}
				switch (c)
				{
				case '"':
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					while ((c = _input[_inputPosition++]) != 0)
					{
						switch (c)
						{
						case '\\':
							c = _input[_inputPosition++];
							if (c == '\0')
							{
								throw GenerateException(ErrorCode.ErrorFilterTrailingBackslash);
							}
							stringBuilder2.Append(c);
							continue;
						default:
							stringBuilder2.Append(c);
							continue;
						case '"':
							break;
						}
						break;
					}
					_stringValue = stringBuilder2.ToString();
					return Symbol.StringValue;
				}
				case '(':
					return Symbol.LeftParenthesis;
				case ')':
					return Symbol.RightParenthesis;
				case '!':
					switch (_input[_inputPosition])
					{
					case '=':
						_inputPosition++;
						return Symbol.OperatorNotEqual;
					case '~':
						_inputPosition++;
						return Symbol.OperatorMatchesNot;
					case '@':
						_inputPosition++;
						return Symbol.OperatorContainsNot;
					default:
						return Symbol.LogopNot;
					}
				case '&':
					return Symbol.LogopAnd;
				case '|':
					return Symbol.LogopOr;
				case ',':
					return Symbol.Comma;
				case '=':
					return Symbol.OperatorEqual;
				case '<':
					if (_input[_inputPosition] == '=')
					{
						_inputPosition++;
						return Symbol.OperatorSmallerEqual;
					}
					return Symbol.OperatorSmaller;
				case '>':
					if (_input[_inputPosition] == '=')
					{
						_inputPosition++;
						return Symbol.OperatorGreaterEqual;
					}
					return Symbol.OperatorGreater;
				case '~':
					return Symbol.OperatorMatches;
				case '@':
					return Symbol.OperatorContains;
				default:
					throw GenerateException(ErrorCode.ErrorFilterIllegalInputSymbol);
				}
			}
		}
	}
}
