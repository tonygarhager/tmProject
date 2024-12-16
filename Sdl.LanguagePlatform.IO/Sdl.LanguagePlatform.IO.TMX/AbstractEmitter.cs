using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.Lingua.Locales;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal abstract class AbstractEmitter
	{
		protected XmlWriter _Output;

		protected LocaleInfoSet _TargetLocaleSystem;

		protected TranslationUnitFormat _TargetFormat;

		protected TMXWriterSettings _WriterSettings;

		protected LanguageResources _SourceResources;

		protected LanguageResources _TargetResources;

		protected LanguageTools _SourceLanguageTools;

		protected LanguageTools _TargetLanguageTools;

		private CompositeResourceDataAccessor _Accessor = new CompositeResourceDataAccessor(addDefaultAccessor: true);

		public AbstractEmitter(XmlWriter output, TMXWriterSettings writerSettings)
		{
			_Output = (output ?? throw new ArgumentNullException());
			_TargetLocaleSystem = null;
			_WriterSettings = writerSettings;
		}

		public abstract void EmitHeaderFormat();

		public abstract void EmitExtendedHeader(StartOfInputEvent soi);

		public abstract void EmitOrigin(TranslationUnitOrigin origin);

		public abstract void EmitFormat(TranslationUnitFormat format);

		public abstract void EmitConfirmationLevel(ConfirmationLevel level);

		public abstract void EmitContext(TuContext tuc);

		public abstract void EmitField(FieldValue field);

		public abstract void EmitLastUsedByField(string fieldValue);

		public abstract void EmitIdContext(string idContext);

		public void EmitHeader(StartOfInputEvent soi)
		{
			_Output.WriteStartElement("tmx");
			_Output.WriteAttributeString("version", "1.4");
			string value = (soi.SourceCulture != null && !soi.SourceCulture.Equals(CultureInfo.InvariantCulture)) ? LookupTargetLanguageCode(soi.SourceCulture) : "en-US";
			_Output.WriteStartElement("header");
			_Output.WriteAttributeString("creationtool", "SDL Language Platform");
			_Output.WriteAttributeString("creationtoolversion", "8.1");
			EmitHeaderFormat();
			_Output.WriteAttributeString("segtype", "sentence");
			_Output.WriteAttributeString("adminlang", value);
			_Output.WriteAttributeString("srclang", value);
			if (soi.CreationDate != default(DateTime))
			{
				_Output.WriteAttributeString("creationdate", TMXConversions.DateTimeToTMX(soi.CreationDate));
			}
			if (!string.IsNullOrEmpty(soi.CreationUser))
			{
				_Output.WriteAttributeString("creationid", soi.CreationUser);
			}
			else
			{
				_Output.WriteAttributeString("creationid", "unknown");
			}
			EmitExtendedHeader(soi);
			_Output.WriteEndElement();
		}

		public void EmitTranslationUnit(TranslationUnit tu)
		{
			EmitTranslationUnit(tu, plainText: false);
		}

		public void EmitTranslationUnit(TranslationUnit tu, bool plainText)
		{
			if (tu == null || (tu.SourceSegment == null && tu.TargetSegment == null))
			{
				return;
			}
			if (tu.SourceSegment != null && tu.TargetSegment != null && _SourceLanguageTools == null)
			{
				_SourceResources = new LanguageResources(tu.SourceSegment.Culture, _Accessor);
				_SourceLanguageTools = new LanguageTools(_SourceResources, BuiltinRecognizers.RecognizeAll);
				_TargetResources = new LanguageResources(tu.TargetSegment.Culture, _Accessor);
				_TargetLanguageTools = new LanguageTools(_TargetResources, BuiltinRecognizers.RecognizeAll);
			}
			_Output.WriteStartElement("tu");
			if (tu.SystemFields != null)
			{
				if (tu.SystemFields.CreationDate != default(DateTime))
				{
					_Output.WriteAttributeString("creationdate", TMXConversions.DateTimeToTMX(tu.SystemFields.CreationDate));
				}
				if (!string.IsNullOrEmpty(tu.SystemFields.CreationUser))
				{
					_Output.WriteAttributeString("creationid", tu.SystemFields.CreationUser);
				}
				if (tu.SystemFields.ChangeDate != default(DateTime))
				{
					_Output.WriteAttributeString("changedate", TMXConversions.DateTimeToTMX(tu.SystemFields.ChangeDate));
				}
				if (!string.IsNullOrEmpty(tu.SystemFields.ChangeUser))
				{
					_Output.WriteAttributeString("changeid", tu.SystemFields.ChangeUser);
				}
				if (tu.SystemFields.UseDate != default(DateTime))
				{
					_Output.WriteAttributeString("lastusagedate", TMXConversions.DateTimeToTMX(tu.SystemFields.UseDate));
				}
				if (tu.SystemFields.UseCount > 0)
				{
					_Output.WriteAttributeString("usagecount", tu.SystemFields.UseCount.ToString(CultureInfo.InvariantCulture));
				}
				if (!string.IsNullOrEmpty(tu.SystemFields.UseUser))
				{
					EmitLastUsedByField(tu.SystemFields.UseUser);
				}
			}
			if (_TargetFormat != TranslationUnitFormat.Unknown)
			{
				if (tu.Contexts?.Values != null)
				{
					foreach (TuContext value in tu.Contexts.Values)
					{
						TuContext tuContext = value;
						if (value.Segment1 != null || value.Segment2 != null)
						{
							tuContext = new TuContext(value.Context1, value.Context2);
							tuContext.Segment1 = value.Segment1;
							tuContext.Segment2 = value.Segment2;
							List<SegmentRange> positionTokenAssociation = null;
							if (value.Segment1 != null)
							{
								string s = _SourceLanguageTools.ComputeIdentityString(value.Segment1, LanguageTools.TokenToFeatureMappingMode.Stem, ref positionTokenAssociation);
								tuContext.Context1 = Hash.GetHashCodeLong(s);
							}
							if (value.Segment2 != null)
							{
								LanguageTools languageTools = _TargetLanguageTools;
								if (string.CompareOrdinal(value.Segment2.Culture.Name, _SourceResources.Culture.Name) == 0)
								{
									languageTools = _SourceLanguageTools;
								}
								string s2 = languageTools.ComputeIdentityString(value.Segment2, LanguageTools.TokenToFeatureMappingMode.Stem, ref positionTokenAssociation);
								tuContext.Context2 = Hash.GetHashCodeLong(s2);
							}
						}
						EmitContext(tuContext);
					}
				}
				if (tu.IdContexts?.Values != null)
				{
					foreach (string value2 in tu.IdContexts.Values)
					{
						EmitIdContext(value2);
					}
				}
				if (tu.Origin != 0)
				{
					EmitOrigin(tu.Origin);
				}
				if (tu.Format != 0)
				{
					EmitFormat(tu.Format);
				}
				if (tu.ConfirmationLevel != 0)
				{
					EmitConfirmationLevel(tu.ConfirmationLevel);
				}
				if (tu.FieldValues != null)
				{
					foreach (FieldValue fieldValue in tu.FieldValues)
					{
						EmitField(fieldValue);
					}
				}
			}
			if (tu.SourceSegment != null)
			{
				EmitSegment(tu.SourceSegment, LookupTargetLanguageCode(tu.SourceSegment.Culture), plainText);
			}
			if (tu.TargetSegment != null)
			{
				EmitSegment(tu.TargetSegment, LookupTargetLanguageCode(tu.TargetSegment.Culture), plainText);
			}
			_Output.WriteEndElement();
		}

		public void EmitSegment(Segment s, string lang, bool plainText)
		{
			_Output.WriteStartElement("tuv");
			_Output.WriteAttributeString("xml", "lang", "http://www.w3.org/XML/1998/namespace", lang);
			_Output.WriteStartElement("seg");
			_Output.WriteString("");
			foreach (SegmentElement element in s.Elements)
			{
				if (element is Text)
				{
					EmitText(((Text)element).Value);
				}
				else if (element is Tag)
				{
					if (!plainText)
					{
						EmitTag((Tag)element);
					}
				}
				else if (element is TagToken)
				{
					if (!plainText)
					{
						EmitTag(((TagToken)element).Tag);
					}
				}
				else if (element is Token)
				{
					EmitText((element as Token).Text);
				}
			}
			_Output.WriteEndElement();
			_Output.WriteEndElement();
		}

		private void EmitTag(Tag t)
		{
			string value = t.Anchor.ToString(CultureInfo.InvariantCulture);
			string value2 = t.AlignmentAnchor.ToString(CultureInfo.InvariantCulture);
			switch (t.Type)
			{
			case TagType.Undefined:
				break;
			case TagType.Start:
				_Output.WriteStartElement("bpt");
				_Output.WriteAttributeString("i", value);
				if (!string.IsNullOrEmpty(t.TagID))
				{
					_Output.WriteAttributeString("type", t.TagID);
				}
				if (t.AlignmentAnchor > 0)
				{
					_Output.WriteAttributeString("x", value2);
				}
				_Output.WriteEndElement();
				break;
			case TagType.End:
				_Output.WriteStartElement("ept");
				_Output.WriteAttributeString("i", value);
				_Output.WriteEndElement();
				break;
			case TagType.Standalone:
				_Output.WriteStartElement("ph");
				if (t.AlignmentAnchor > 0)
				{
					_Output.WriteAttributeString("x", value2);
				}
				if (!string.IsNullOrEmpty(t.TagID))
				{
					_Output.WriteAttributeString("type", t.TagID);
				}
				_Output.WriteEndElement();
				break;
			case TagType.TextPlaceholder:
			case TagType.LockedContent:
			{
				_Output.WriteStartElement("bpt");
				_Output.WriteAttributeString("i", value);
				string text = (t.Type == TagType.TextPlaceholder) ? TMXReader.TextPlaceholderTagID : TMXReader.LockedContentTagID;
				if (!string.IsNullOrWhiteSpace(t.TagID))
				{
					text += t.TagID;
				}
				_Output.WriteAttributeString("type", text);
				if (t.AlignmentAnchor > 0)
				{
					_Output.WriteAttributeString("x", value2);
				}
				_Output.WriteEndElement();
				_Output.WriteString(t.TextEquivalent ?? string.Empty);
				_Output.WriteStartElement("ept");
				_Output.WriteAttributeString("i", value);
				_Output.WriteEndElement();
				break;
			}
			}
		}

		private void EmitText(string t)
		{
			if (_WriterSettings.ReplaceInvalidCharacters)
			{
				StringBuilder stringBuilder = new StringBuilder(t);
				for (int num = stringBuilder.Length - 1; num >= 0; num--)
				{
					char c = stringBuilder[num];
					if (c < ' ' && c != '\r' && c != '\n' && c != '\t')
					{
						if (_WriterSettings.ReplacementCharacter == '\0')
						{
							stringBuilder.Remove(num, 1);
						}
						else
						{
							stringBuilder[num] = _WriterSettings.ReplacementCharacter;
						}
					}
				}
				_Output.WriteString(stringBuilder.ToString());
			}
			else
			{
				_Output.WriteString(t);
			}
		}

		protected string LookupTargetLanguageCode(CultureInfo culture)
		{
			if (_TargetLocaleSystem == null)
			{
				return culture.Name;
			}
			LocaleInfo localeInfo = _TargetLocaleSystem.LookupCanonicalCode(culture.Name);
			if (localeInfo == null)
			{
				return culture.Name;
			}
			return localeInfo.Code;
		}
	}
}
