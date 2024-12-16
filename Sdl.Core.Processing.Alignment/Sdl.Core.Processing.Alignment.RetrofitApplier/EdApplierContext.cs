using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class EdApplierContext
	{
		public IDocumentItemFactory ItemFactory;

		public EditDistance EditDistance
		{
			get;
			set;
		}

		public List<EditDistanceItem> AllEditDistanceItems
		{
			get;
			set;
		}

		public Segment OriginalLinguaSegment
		{
			get;
			set;
		}

		public Segment UpdatedLinguaSegment
		{
			get;
			set;
		}

		public Dictionary<int, int> LinguaTagPairEDMapper
		{
			get;
			set;
		}

		public List<IAbstractMarkupData> BilingualDeleteTokens
		{
			get;
			set;
		}

		public List<Token> LinguaInsertTokens
		{
			get;
			set;
		}

		public ModelMapper Map
		{
			get;
			set;
		}

		public ISegment OriginalBilingualSegment
		{
			get;
			set;
		}

		public ISegment UpdatedBilingualSegment
		{
			get;
			set;
		}

		public int EdIndex
		{
			get;
			set;
		}

		public int EdRightIndex
		{
			get;
			set;
		}

		public int EdLeftIndex
		{
			get;
			set;
		}
	}
}
