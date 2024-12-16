using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class QuickTagContent : MarkupDataContainer, IQuickTagContent, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ICloneable
	{
		public QuickTagContent()
		{
		}

		protected QuickTagContent(QuickTagContent other)
			: base(other)
		{
		}

		public override object Clone()
		{
			return new QuickTagContent(this);
		}

		public override void Add(IAbstractMarkupData item)
		{
			if (item.Parent != null)
			{
				throw new FileTypeSupportException(StringResources.MarkupDataContainer_AlreadyInCollectionError);
			}
			_Content.Add(item);
			item.Parent = null;
		}
	}
}
