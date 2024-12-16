using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class ImportStatistics
	{
		public int Errors
		{
			get;
			set;
		}

		public int DiscardedTranslationUnits
		{
			get;
			set;
		}

		public int AddedTranslationUnits
		{
			get;
			set;
		}

		public int MergedTranslationUnits
		{
			get;
			set;
		}

		public int OverwrittenTranslationUnits
		{
			get;
			set;
		}

		public int DeletedTranslationUnits
		{
			get;
			set;
		}

		public int RawTUs
		{
			get;
			set;
		}

		public int TotalRead
		{
			get;
			set;
		}

		public int BadTranslationUnits
		{
			get;
			set;
		}

		public int TotalImported => AddedTranslationUnits + MergedTranslationUnits + OverwrittenTranslationUnits;

		public void Add(ImportStatistics other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			TotalRead += other.TotalRead;
			AddedTranslationUnits += other.AddedTranslationUnits;
			MergedTranslationUnits += other.MergedTranslationUnits;
			Errors += other.Errors;
			OverwrittenTranslationUnits += other.OverwrittenTranslationUnits;
			DiscardedTranslationUnits += other.DiscardedTranslationUnits;
			BadTranslationUnits += other.BadTranslationUnits;
			DeletedTranslationUnits += other.DeletedTranslationUnits;
		}
	}
}
