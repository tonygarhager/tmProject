using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class BilingualTagPairToken : IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		public IAbstractMarkupDataContainer RealMarkupContainer;

		public int Anchor
		{
			get;
			set;
		}

		public bool IsStart
		{
			get;
			set;
		}

		public BilingualTagPairToken StartToken
		{
			get;
			set;
		}

		public IAbstractMarkupDataContainer Parent
		{
			get;
			set;
		}

		public int IndexInParent => Parent.IndexOf(this);

		public IParagraph ParentParagraph => ((IAbstractMarkupData)Parent).ParentParagraph;

		public int UniqueId
		{
			get;
			set;
		}

		public BilingualTagPairToken(TagToken linguaTagToken, IAbstractMarkupDataContainer realMarkupContainer)
		{
			Anchor = linguaTagToken.Tag.Anchor;
			IsStart = (linguaTagToken.Tag.Type == TagType.Start);
			RealMarkupContainer = realMarkupContainer;
		}

		public override string ToString()
		{
			return string.Format("<{0}t{1}>", (!IsStart) ? "/" : "", Anchor);
		}

		public void AcceptVisitor(IMarkupDataVisitor visitor)
		{
		}

		public void RemoveFromParent()
		{
			Parent.Remove(this);
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}
	}
}
