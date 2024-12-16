using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	internal class RegExFileHelper
	{
		private List<Pair<string, string>> _replaceInfo = new List<Pair<string, string>>();

		public List<Pair<string, string>> ReplaceInfo
		{
			get
			{
				return _replaceInfo;
			}
			set
			{
				_replaceInfo = value;
			}
		}

		public void StoreReplaceInfo(string regExMatch, string regExReplace)
		{
			List<Pair<string, string>> list = new List<Pair<string, string>>();
			list.Add(new Pair<string, string>(regExMatch, regExReplace));
			_replaceInfo = list;
		}

		public void StoreReplaceInfo(List<string> regExMatchList, List<string> regExReplaceList)
		{
			if (regExMatchList.Count != regExReplaceList.Count)
			{
				throw new ArgumentException("Number of items in regExMatchList and regExReplaceList does not match");
			}
			List<Pair<string, string>> list = new List<Pair<string, string>>();
			for (int i = 0; i < regExMatchList.Count; i++)
			{
				list.Add(new Pair<string, string>(regExMatchList[i], regExReplaceList[i]));
			}
			_replaceInfo = list;
		}

		public bool RegExsWillApplyToFile(string filename)
		{
			if (_replaceInfo.Count == 0)
			{
				return false;
			}
			using (StreamReader streamReader = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				string input;
				while ((input = streamReader.ReadLine()) != null)
				{
					foreach (Pair<string, string> item in _replaceInfo)
					{
						Regex regex = new Regex(item.First);
						if (regex.IsMatch(input))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void ApplyRegExsToFile(string filename, string outputFilename, Encoding outputEncoding)
		{
			if (outputEncoding == null)
			{
				outputEncoding = Encoding.UTF8;
			}
			string tempFileName = Path.GetTempFileName();
			using (StreamReader sr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), outputEncoding))
			{
				using (StreamWriter streamWriter = new StreamWriter(File.Open(tempFileName, FileMode.Append), outputEncoding))
				{
					string text;
					while ((text = ReadLine(sr)) != null)
					{
						foreach (Pair<string, string> item in _replaceInfo)
						{
							Regex regex = new Regex(item.First);
							text = regex.Replace(text, item.Second);
						}
						streamWriter.Write(text);
					}
				}
			}
			if (string.Compare(filename, outputFilename, ignoreCase: true) == 0)
			{
				File.Delete(filename);
			}
			File.Move(tempFileName, outputFilename);
		}

		private string ReadLine(StreamReader sr)
		{
			StringBuilder stringBuilder = null;
			int num;
			while ((num = sr.Read()) != -1)
			{
				char c = (char)num;
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.Append(c);
				if (c == '\n')
				{
					break;
				}
			}
			return stringBuilder?.ToString();
		}
	}
}
