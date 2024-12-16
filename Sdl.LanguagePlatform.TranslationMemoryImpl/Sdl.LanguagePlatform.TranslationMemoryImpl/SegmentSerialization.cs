using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class SegmentSerialization
	{
		public const int CurrentSerializationVersion = 1;

		public static void Save(Sdl.LanguagePlatform.Core.Segment segment, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.Segment storageSegment)
		{
			storageSegment.Text = null;
			storageSegment.SerializedTags = null;
			List<byte> list = new List<byte>();
			storageSegment.Text = SegmentSerializer.Save(segment, list);
			if (list.Count > 0)
			{
				storageSegment.SerializedTags = list.ToArray();
			}
		}

		public static Sdl.LanguagePlatform.Core.Segment Load(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.Segment storageSegment, CultureInfo culture)
		{
			return SegmentSerializer.Load(storageSegment.Text, storageSegment.SerializedTags, culture);
		}
	}
}
