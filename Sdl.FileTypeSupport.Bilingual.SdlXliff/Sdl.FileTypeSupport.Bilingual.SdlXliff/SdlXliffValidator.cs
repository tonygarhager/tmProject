using Sdl.FileTypeSupport.Bilingual.SdlXliff.Util;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class SdlXliffValidator
	{
		private static readonly XmlSchemaSet _schemaSet;

		private bool _issuesReported;

		private string _filePath;

		private readonly EventHandler<ProgressEventArgs> _progress;

		public event EventHandler<SdlXliffValidationEventArgs> ValidationIssue;

		static SdlXliffValidator()
		{
			_schemaSet = new XmlSchemaSet();
			_schemaSet.XmlResolver = null;
			AddSchemaFromResource("http://www.w3.org/XML/1998/namespace", "Sdl.FileTypeSupport.Bilingual.SdlXliff.Xliff.xml.xsd");
			AddSchemaFromResource("urn:oasis:names:tc:xliff:document:1.2", "Sdl.FileTypeSupport.Bilingual.SdlXliff.Xliff.xliff-core-1.2-strict_20032007.xsd");
			AddSchemaFromResource("http://sdl.com/FileTypes/SdlXliff/1.0", "Sdl.FileTypeSupport.Bilingual.SdlXliff.Xliff.sdl-FilterFramework2-1.0.xsd");
		}

		private static void AddSchemaFromResource(string schemaUri, string resourceId)
		{
			using (XmlReader schemaDocument = XmlReader.Create(ResourceExtractor.GetResourceStream(resourceId)))
			{
				_schemaSet.Add(schemaUri, schemaDocument);
			}
		}

		public SdlXliffValidator(EventHandler<ProgressEventArgs> progress = null)
		{
			_progress = progress;
		}

		public virtual void OnValidationIssue(SdlXliffValidationEventArgs args)
		{
			if (this.ValidationIssue != null)
			{
				this.ValidationIssue(this, args);
			}
		}

		protected virtual void OnProgress(byte percent)
		{
			if (_progress != null)
			{
				_progress(this, new ProgressEventArgs(percent));
			}
		}

		public bool Validate(string sdlXliffFilePath)
		{
			_issuesReported = false;
			_filePath = sdlXliffFilePath;
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.Schemas = _schemaSet;
				xmlReaderSettings.ValidationEventHandler += ValidationEvent;
				xmlReaderSettings.CheckCharacters = false;
				using (FileStream input = File.OpenRead(sdlXliffFilePath))
				{
					using (XmlReader xmlReader = XmlReader.Create(input, xmlReaderSettings))
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						while (xmlReader.Read())
						{
							if (stopwatch.ElapsedMilliseconds >= 1000)
							{
								stopwatch.Reset();
								stopwatch.Start();
								OnProgress(5);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ReportValidationIssue(XmlSeverityType.Error, ex.Message, -1, -1);
			}
			return _issuesReported;
		}

		private void ValidationEvent(object sender, ValidationEventArgs e)
		{
			ReportValidationIssue(e.Severity, e.Message, e.Exception.LineNumber, e.Exception.LinePosition);
		}

		private void ReportValidationIssue(XmlSeverityType severity, string message, int line, int offset)
		{
			_issuesReported = true;
			OnValidationIssue(new SdlXliffValidationEventArgs(severity, message, line, offset, _filePath));
		}
	}
}
