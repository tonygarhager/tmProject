using Sdl.Core.Globalization;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IPersistentFileConversionProperties : IMetaDataContainer, ICloneable
	{
		FileId FileId
		{
			get;
			set;
		}

		FileTypeDefinitionId FileTypeDefinitionId
		{
			get;
			set;
		}

		string this[string key]
		{
			get;
			set;
		}

		IList<IDependencyFileProperties> DependencyFiles
		{
			get;
		}

		string OriginalFilePath
		{
			get;
			set;
		}

		string InputFilePath
		{
			get;
			set;
		}

		Codepage OriginalEncoding
		{
			get;
			set;
		}

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

		Codepage PreferredTargetEncoding
		{
			get;
			set;
		}

		SniffInfo FileSnifferInfo
		{
			get;
			set;
		}

		string CreationTool
		{
			get;
			set;
		}

		string CreationToolVersion
		{
			get;
			set;
		}

		DateTime CreationDate
		{
			get;
			set;
		}
	}
}
