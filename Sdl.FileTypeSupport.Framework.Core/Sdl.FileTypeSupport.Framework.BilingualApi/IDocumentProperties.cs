using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IDocumentProperties : ICloneable
	{
		Language SourceLanguage
		{
			get;
			set;
		}

		Language TargetLanguage
		{
			get;
			set;
		}

		SourceCount SourceCount
		{
			get;
			set;
		}

		IRepetitionsTable Repetitions
		{
			get;
			set;
		}

		string LastSavedAsPath
		{
			get;
			set;
		}

		string LastOpenedAsPath
		{
			get;
			set;
		}
	}
}
