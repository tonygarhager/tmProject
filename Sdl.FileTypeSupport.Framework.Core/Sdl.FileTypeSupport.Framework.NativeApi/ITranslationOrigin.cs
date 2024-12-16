using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ITranslationOrigin : IMetaDataContainer, ICloneable
	{
		string OriginType
		{
			get;
			set;
		}

		string OriginSystem
		{
			get;
			set;
		}

		ITranslationOrigin OriginBeforeAdaptation
		{
			get;
			set;
		}

		byte MatchPercent
		{
			get;
			set;
		}

		bool IsStructureContextMatch
		{
			get;
			set;
		}

		TextContextMatchLevel TextContextMatchLevel
		{
			get;
			set;
		}

		bool IsSIDContextMatch
		{
			get;
			set;
		}

		RepetitionId RepetitionTableId
		{
			get;
			set;
		}

		bool IsRepeated
		{
			get;
		}

		string OriginalTranslationHash
		{
			get;
			set;
		}

		string this[string key]
		{
			get;
			set;
		}
	}
}
