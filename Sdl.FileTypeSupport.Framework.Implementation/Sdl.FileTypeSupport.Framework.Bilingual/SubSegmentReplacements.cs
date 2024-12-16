using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	internal class SubSegmentReplacements
	{
		private class Replacement
		{
			internal ISubSegmentProperties Properties;

			internal string NewText;
		}

		private List<Replacement> _Replacements = new List<Replacement>();

		private IAbstractTag _Tag;

		public IAbstractTag Tag
		{
			get
			{
				return _Tag;
			}
			set
			{
				_Tag = value;
			}
		}

		internal int ReplacementsCount => _Replacements.Count;

		internal void AddReplacement(ISubSegmentProperties properties, string newText)
		{
			Replacement replacement = new Replacement();
			replacement.Properties = properties;
			replacement.NewText = newText;
			for (int i = 0; i < _Replacements.Count; i++)
			{
				if (properties.StartOffset > _Replacements[i].Properties.StartOffset)
				{
					_Replacements.Insert(i, replacement);
					replacement = null;
					break;
				}
			}
			if (replacement != null)
			{
				_Replacements.Add(replacement);
			}
		}

		internal void ApplyReplacements()
		{
			foreach (Replacement replacement2 in _Replacements)
			{
				_Tag.TagProperties.TagContent = _Tag.TagProperties.TagContent.Substring(0, replacement2.Properties.StartOffset) + replacement2.NewText + _Tag.TagProperties.TagContent.Substring(replacement2.Properties.StartOffset + replacement2.Properties.Length);
			}
			int num = 0;
			_Tag.TagProperties.ClearSubSegments();
			for (int num2 = _Replacements.Count - 1; num2 >= 0; num2--)
			{
				Replacement replacement = _Replacements[num2];
				ISubSegmentProperties subSegmentProperties = (ISubSegmentProperties)replacement.Properties.Clone();
				subSegmentProperties.StartOffset += num;
				num += replacement.NewText.Length - subSegmentProperties.Length;
				subSegmentProperties.Length = replacement.NewText.Length;
				if (subSegmentProperties.Contexts != null && subSegmentProperties.Contexts.Contexts != null)
				{
					List<IContextInfo> list = new List<IContextInfo>();
					bool flag = false;
					foreach (IContextInfo context in subSegmentProperties.Contexts.Contexts)
					{
						if (context.MetaDataContainsKey("SDL:AddedByFramework"))
						{
							list.Add(context);
							if (context.GetMetaData("SDL:AddedByFramework") == "SDL:CreatedByFramework")
							{
								flag = true;
							}
						}
					}
					foreach (IContextInfo item in list)
					{
						subSegmentProperties.Contexts.Contexts.Remove(item);
					}
					if (subSegmentProperties.Contexts.Contexts.Count == 0 && flag)
					{
						subSegmentProperties.Contexts = null;
					}
				}
				_Tag.TagProperties.AddSubSegment(subSegmentProperties);
			}
		}
	}
}
