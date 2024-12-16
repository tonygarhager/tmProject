using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	internal class ParagraphDirectionCache
	{
		private const string SourceKey = "-S";

		private const string TargetKey = "-T";

		private const char KeyValueSplit = ':';

		private const char ParagraphSplit = ';';

		private readonly Dictionary<string, Direction> _paragraphIdToDirectionMap = new Dictionary<string, Direction>();

		public void InitializeFromString(string paragraphDirections)
		{
			if (string.IsNullOrEmpty(paragraphDirections))
			{
				return;
			}
			string[] array = paragraphDirections.Split(';');
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string[] array2 = text.Split(':');
					if (array2.Length == 2)
					{
						try
						{
							Direction value = (Direction)Enum.Parse(typeof(Direction), array2[1]);
							_paragraphIdToDirectionMap.Add(array2[0], value);
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		public void StoreParagraphDirection(IParagraph paragraph)
		{
			string paragraphDirectionsKey = GetParagraphDirectionsKey(paragraph);
			if (paragraph.TextDirection != 0 && !_paragraphIdToDirectionMap.ContainsKey(paragraphDirectionsKey))
			{
				_paragraphIdToDirectionMap.Add(paragraphDirectionsKey, paragraph.TextDirection);
			}
		}

		public Direction GetDirection(IParagraph paragraph)
		{
			string paragraphDirectionsKey = GetParagraphDirectionsKey(paragraph);
			if (_paragraphIdToDirectionMap.Count > 0 && _paragraphIdToDirectionMap.ContainsKey(paragraphDirectionsKey))
			{
				return _paragraphIdToDirectionMap[paragraphDirectionsKey];
			}
			return Direction.Inherit;
		}

		public string SerializeAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, Direction> item in _paragraphIdToDirectionMap)
			{
				stringBuilder.Append(SerializeDirection(item.Key, item.Value));
			}
			return stringBuilder.ToString();
		}

		private static string SerializeDirection(string paragraph, Direction textDirection)
		{
			return paragraph + ":" + textDirection.ToString() + ";";
		}

		private static string GetParagraphDirectionsKey(IParagraph paragraph)
		{
			string id = paragraph.Parent.Properties.ParagraphUnitId.Id;
			string str = paragraph.IsSource ? "-S" : "-T";
			return id + str;
		}
	}
}
