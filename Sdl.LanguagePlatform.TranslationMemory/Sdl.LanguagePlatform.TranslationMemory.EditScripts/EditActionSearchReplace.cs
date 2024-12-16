using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditActionSearchReplace : EditAction
	{
		private Regex _CompiledPattern;

		[DataMember]
		public bool ApplyToTarget
		{
			get;
			set;
		}

		[DataMember]
		public string SearchPattern
		{
			get;
			set;
		}

		[DataMember]
		public string ReplacementPattern
		{
			get;
			set;
		}

		[DataMember]
		public PatternType PatternType
		{
			get;
			set;
		}

		[DataMember]
		public bool IgnoreCase
		{
			get;
			set;
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			return true;
		}

		public override bool Apply(TranslationUnit tu)
		{
			if (tu == null)
			{
				throw new ArgumentNullException("tu");
			}
			return Apply(ApplyToTarget ? tu.TargetSegment : tu.SourceSegment);
		}

		private bool Apply(Segment segment)
		{
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			bool result = false;
			foreach (SegmentElement element in segment.Elements)
			{
				Text text = element as Text;
				if (text != null)
				{
					string value = Apply(text);
					if (!text.Value.Equals(value))
					{
						text.Value = value;
						result = true;
					}
				}
			}
			return result;
		}

		private string Apply(Text txt)
		{
			if (PatternType == PatternType.RegularExpression)
			{
				if (_CompiledPattern != null)
				{
					return _CompiledPattern.Replace(txt.Value, ReplacementPattern);
				}
				RegexOptions regexOptions = RegexOptions.CultureInvariant;
				if (IgnoreCase)
				{
					regexOptions |= RegexOptions.IgnoreCase;
				}
				_CompiledPattern = new Regex(SearchPattern, regexOptions);
				return _CompiledPattern.Replace(txt.Value, ReplacementPattern);
			}
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			int length = SearchPattern.Length;
			int length2 = txt.Value.Length;
			if (string.IsNullOrEmpty(SearchPattern))
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptEmptySearchPattern);
			}
			bool flag = IgnoreCase && CharacterProperties.IsAll(ReplacementPattern, char.IsLower);
			do
			{
				int num2 = txt.Value.IndexOf(SearchPattern, num, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
				if (num2 < 0)
				{
					stringBuilder.Append(txt.Value.Substring(num));
					break;
				}
				if (num2 > num)
				{
					stringBuilder.Append(txt.Value.Substring(num, num2 - num));
				}
				if (!string.IsNullOrEmpty(ReplacementPattern))
				{
					string text = ReplacementPattern;
					if (flag)
					{
						string text2 = txt.Value.Substring(num2, length);
						if (!SearchPattern.Equals(text2, StringComparison.Ordinal))
						{
							switch (StringUtilities.DetermineCasing(text2))
							{
							case StringUtilities.Casing.AllUpper:
								text = text.ToUpper(CultureInfo.InvariantCulture);
								break;
							case StringUtilities.Casing.AllLower:
								text = text.ToLower(CultureInfo.InvariantCulture);
								break;
							case StringUtilities.Casing.InitialUpper:
								text = char.ToUpper(text[0]).ToString() + text.Substring(1).ToLower(CultureInfo.InvariantCulture);
								break;
							}
						}
					}
					stringBuilder.Append(text);
				}
				num = num2 + length;
			}
			while (num < length2);
			return stringBuilder.ToString();
		}
	}
}
