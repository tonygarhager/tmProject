using System;
using System.Collections.Specialized;
using System.IO;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class PathUtil
	{
		public static string RelativePathTo(string fromDirectory, string toPath)
		{
			if (fromDirectory == null)
			{
				throw new ArgumentNullException("fromDirectory");
			}
			if (toPath == null)
			{
				throw new ArgumentNullException("toPath");
			}
			if (Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath) && string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), ignoreCase: true) != 0)
			{
				return toPath;
			}
			StringCollection stringCollection = new StringCollection();
			string[] array = fromDirectory.Split(Path.DirectorySeparatorChar);
			string[] array2 = toPath.Split(Path.DirectorySeparatorChar);
			int num = Math.Min(array.Length, array2.Length);
			int num2 = -1;
			for (int i = 0; i < num && string.Compare(array[i], array2[i], ignoreCase: true) == 0; i++)
			{
				num2 = i;
			}
			if (num2 == -1)
			{
				return toPath;
			}
			for (int j = num2 + 1; j < array.Length; j++)
			{
				if (array[j].Length > 0)
				{
					stringCollection.Add("..");
				}
			}
			for (int k = num2 + 1; k < array2.Length; k++)
			{
				stringCollection.Add(array2[k]);
			}
			string[] array3 = new string[stringCollection.Count];
			stringCollection.CopyTo(array3, 0);
			return string.Join(Path.DirectorySeparatorChar.ToString(), array3);
		}
	}
}
