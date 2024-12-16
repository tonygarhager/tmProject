using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Sdl.LanguagePlatform.IO.TMX
{
	public class TMXWriter : IEventReceiver, IDisposable
	{
		private XmlWriter _Output;

		private Stream _OwnedOutputStream;

		private TMXWriterSettings _WriterSettings;

		private AbstractEmitter _FieldEmitter;

		public TMXWriter(string filename)
			: this(filename, new TMXWriterSettings(Encoding.UTF8))
		{
		}

		public TMXWriter(Stream stream)
			: this(stream, new TMXWriterSettings(Encoding.UTF8))
		{
		}

		public TMXWriter(string filename, TMXWriterSettings writerSettings)
		{
			_WriterSettings = (writerSettings ?? new TMXWriterSettings());
			XmlWriterSettings settings = CreateXmlWriterSettings(writerSettings.Encoding);
			if (filename.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
			{
				Stream stream = File.Create(filename);
				_OwnedOutputStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: false);
			}
			else
			{
				_OwnedOutputStream = File.Create(filename);
			}
			_Output = XmlWriter.Create(_OwnedOutputStream, settings);
			SetupFormatDependentSettings(_Output);
		}

		public TMXWriter(Stream outStream, TMXWriterSettings writerSettings)
		{
			_WriterSettings = (writerSettings ?? new TMXWriterSettings());
			XmlWriterSettings settings = CreateXmlWriterSettings(writerSettings.Encoding);
			_OwnedOutputStream = null;
			_Output = XmlWriter.Create(outStream, settings);
			SetupFormatDependentSettings(_Output);
		}

		private void SetupFormatDependentSettings(XmlWriter output)
		{
			_FieldEmitter = null;
			if (_WriterSettings != null)
			{
				switch (_WriterSettings.TargetFormat)
				{
				case TranslationUnitFormat.SDLX:
					_FieldEmitter = new SDLXEmitter(output, _WriterSettings);
					break;
				case TranslationUnitFormat.TradosTranslatorsWorkbench:
					_FieldEmitter = new WorkbenchEmitter(output, _WriterSettings);
					break;
				case TranslationUnitFormat.IdiomWorldServer:
					_FieldEmitter = new WSEmitter(output, _WriterSettings);
					break;
				default:
					_FieldEmitter = new TM8Emitter(output, _WriterSettings);
					break;
				}
			}
			if (_FieldEmitter == null)
			{
				TMXWriterSettings writerSettings = new TMXWriterSettings
				{
					TargetFormat = TranslationUnitFormat.SDLTradosStudio2009
				};
				_FieldEmitter = new TM8Emitter(output, writerSettings);
			}
		}

		private XmlWriterSettings CreateXmlWriterSettings(Encoding encoding)
		{
			return new XmlWriterSettings
			{
				Indent = true,
				Encoding = encoding,
				NewLineOnAttributes = false,
				CheckCharacters = false,
				NewLineHandling = NewLineHandling.Entitize
			};
		}

		public void Emit(Event e)
		{
			if (_Output == null)
			{
				return;
			}
			if (e is StartOfInputEvent)
			{
				_Output.WriteStartDocument();
				_FieldEmitter.EmitHeader(e as StartOfInputEvent);
				_Output.WriteStartElement("body");
			}
			else if (e is TUEvent)
			{
				TranslationUnit translationUnit = (e as TUEvent).TranslationUnit;
				_FieldEmitter.EmitTranslationUnit(translationUnit, _WriterSettings.PlainText);
			}
			else if (e is EndOfInputEvent)
			{
				_Output.WriteEndElement();
				_Output.WriteEndElement();
				Close();
			}
			else if (e is CommentEvent)
			{
				string message = (e as CommentEvent).Message;
				if (!string.IsNullOrEmpty(message))
				{
					_Output.WriteComment(message);
				}
			}
		}

		public void Close()
		{
			_FieldEmitter = null;
			if (_Output != null)
			{
				_Output.Close();
				if (_OwnedOutputStream != null)
				{
					_OwnedOutputStream.Close();
					_OwnedOutputStream.Dispose();
					_OwnedOutputStream = null;
				}
			}
		}

		public void Dispose()
		{
			Close();
		}
	}
}
