using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IMetaDataContainer
	{
		IEnumerable<KeyValuePair<string, string>> MetaData
		{
			get;
		}

		bool HasMetaData
		{
			get;
		}

		int MetaDataCount
		{
			get;
		}

		bool MetaDataContainsKey(string key);

		string GetMetaData(string key);

		void SetMetaData(string key, string value);

		bool RemoveMetaData(string key);

		void ClearMetaData();
	}
}
