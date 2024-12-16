using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SearchResult
	{
		[DataMember]
		public ScoringResult ScoringResult
		{
			get;
			set;
		}

		[DataMember]
		public List<Placeable> MemoryPlaceables
		{
			get;
			set;
		}

		[DataMember]
		public List<PlaceableAssociation> PlaceableAssociations
		{
			get;
			set;
		}

		[DataMember]
		public TranslationUnit MemoryTranslationUnit
		{
			get;
			set;
		}

		[DataMember]
		public TranslationUnit TranslationProposal
		{
			get;
			set;
		}

		[DataMember]
		public TuContext ContextData
		{
			get;
			set;
		}

		[DataMember]
		public int CascadeEntryIndex
		{
			get;
			set;
		} = -1;


		public int MatchingPlaceholderTokens
		{
			get;
			set;
		}

		public Dictionary<string, object> MetaData
		{
			get;
			set;
		}

		public SearchResult(TranslationUnit tmTu)
		{
			MemoryTranslationUnit = tmTu;
			MetaData = new Dictionary<string, object>();
		}
	}
}
