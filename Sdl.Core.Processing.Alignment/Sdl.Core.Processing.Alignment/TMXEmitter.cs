using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryTools;
using System;
using System.Text;
using System.Threading;

namespace Sdl.Core.Processing.Alignment
{
	internal class TMXEmitter : AbstractBilingualContentProcessor, IDisposable
	{
		private TMXWriter _writer;

		private bool _writerOwned;

		private bool _emitHeader;

		private LanguagePair _languageDirection;

		private bool _validate;

		private string _userId;

		public string UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("User ID must not be null or empty");
				}
				_userId = value;
			}
		}

		public bool Validate
		{
			get
			{
				return _validate;
			}
			set
			{
				_validate = value;
			}
		}

		public TMXEmitter(string outputFileName)
		{
			TMXWriterSettings writerSettings = new TMXWriterSettings(Encoding.UTF8);
			_writer = new TMXWriter(outputFileName, writerSettings);
			_writerOwned = true;
			_emitHeader = true;
			Init();
		}

		public TMXEmitter(TMXWriter writer)
		{
			_writer = writer;
			_writerOwned = false;
			_emitHeader = false;
			Init();
		}

		private void Init()
		{
			_userId = Thread.CurrentPrincipal.Identity.Name;
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_languageDirection = new LanguagePair(documentInfo.SourceLanguage.CultureInfo, documentInfo.TargetLanguage.CultureInfo);
			if (_emitHeader)
			{
				TMXStartOfInputEvent e = new TMXStartOfInputEvent
				{
					CreationDate = DateTime.UtcNow,
					CreationUser = _userId,
					SourceCulture = _languageDirection.SourceCulture,
					TargetCulture = _languageDirection.TargetCulture
				};
				_writer.Emit(e);
				_emitHeader = false;
			}
			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			base.SetFileProperties(fileInfo);
		}

		public override void Complete()
		{
			Dispose();
			base.Complete();
		}

		public void Dispose()
		{
			if (_writerOwned && _writer != null)
			{
				_writer.Close();
				_writer.Dispose();
			}
			_writer = null;
		}

		private void Emit(ISegmentPair sp, IParagraphUnitProperties paragraphProperties)
		{
			if (sp == null || sp.Source == null || sp.Target == null)
			{
				return;
			}
			bool hasSourceTrackChanges;
			TranslationUnit translationUnit = TUConverter.BuildLinguaTranslationUnit(_languageDirection, sp, paragraphProperties, stripTags: false, excludeTagsInLockedContentText: true, acceptTrackChanges: false, alignTags: true, out hasSourceTrackChanges);
			if (translationUnit == null)
			{
				return;
			}
			translationUnit.CheckAndComputeTagAssociations();
			if (sp.Properties.TranslationOrigin != null)
			{
				string metaData = sp.Properties.TranslationOrigin.GetMetaData("AlignmentQuality");
				if (metaData != null)
				{
					translationUnit.FieldValues.Add(new SinglePicklistFieldValue("AlignmentQuality", new PicklistItem(metaData)));
				}
			}
			if (sp.Properties.TranslationOrigin != null && !string.IsNullOrEmpty(sp.Properties.TranslationOrigin.OriginSystem) && sp.Properties.TranslationOrigin.OriginSystem.StartsWith("Alignment"))
			{
				translationUnit.FieldValues.Add(new SinglePicklistFieldValue("AlignmentType", new PicklistItem(sp.Properties.TranslationOrigin.OriginSystem)));
			}
			if (!_validate || translationUnit.Validate(Segment.ValidationMode.ReportAllErrors) == ErrorCode.OK)
			{
				_writer.Emit(new TUEvent(translationUnit));
			}
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit != null && !paragraphUnit.IsStructure)
			{
				foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
				{
					if (segmentPair != null && segmentPair.Source != null && segmentPair.Target != null)
					{
						Emit(segmentPair, paragraphUnit.Properties);
					}
				}
			}
			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
