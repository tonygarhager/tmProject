using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TranslationModelDetails
	{
		public TranslationModelId Id
		{
			get;
			set;
		}

		public CultureInfo SourceCulture
		{
			get;
			set;
		}

		public CultureInfo TargetCulture
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<AlignableCorpusId> CorpusIds
		{
			get;
			set;
		}

		public DateTime? ModelDate
		{
			get;
			set;
		}

		public TranslationModelTypes ModelType
		{
			get;
			set;
		}

		public int SampleCount
		{
			get;
			set;
		}

		public int Version
		{
			get;
			set;
		}
	}
}
