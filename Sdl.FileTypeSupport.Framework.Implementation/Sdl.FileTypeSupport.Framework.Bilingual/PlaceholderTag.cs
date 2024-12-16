using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class PlaceholderTag : AbstractTag, IPlaceholderTag, IAbstractTag, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		public IPlaceholderTagProperties Properties
		{
			get
			{
				return (IPlaceholderTagProperties)TagProperties;
			}
			set
			{
				TagProperties = value;
			}
		}

		public PlaceholderTag()
		{
		}

		protected PlaceholderTag(PlaceholderTag other)
			: base(other)
		{
		}

		public override object Clone()
		{
			return new PlaceholderTag(this);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitPlaceholderTag(this);
		}
	}
}
