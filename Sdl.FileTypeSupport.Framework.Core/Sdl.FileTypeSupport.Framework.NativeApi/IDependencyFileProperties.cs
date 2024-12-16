using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IDependencyFileProperties : ICloneable
	{
		string Id
		{
			get;
			set;
		}

		bool FileExists
		{
			get;
		}

		string CurrentFilePath
		{
			get;
			set;
		}

		FileJanitor ZippedCurrentFile
		{
			get;
		}

		string OriginalFilePath
		{
			get;
			set;
		}

		DateTime OriginalLastChangeDate
		{
			get;
			set;
		}

		string PathRelativeToConverted
		{
			get;
			set;
		}

		string Description
		{
			get;
			set;
		}

		DependencyFileUsage ExpectedUsage
		{
			get;
			set;
		}

		DependencyFileLinkOption PreferredLinkage
		{
			get;
			set;
		}

		IDisposable DisposableObject
		{
			get;
			set;
		}
	}
}
