using System.IO;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// Namespace for methods that have to do with temporary file manipulations.
	/// </summary>
	public static class TempFileUtils
	{
		/// <summary>
		/// Construct a temp file name based on an original file name (which may include a path).
		/// This can optionally be done using a list of file extension mappings given in extensionMaps parameter.
		/// </summary>
		/// <param name="extensionMaps">
		/// <para>
		/// A list of file extension maps that can be used to map the original filename extension to a new file extension
		/// that can be used for previews.
		/// </para>
		/// <para>
		/// Each string in the extensionMaps array can either be in two formats.  This first is 
		/// the original file extension followed by '|' and then the new file extension that will be used for a preview.  This will
		/// generate preview files using the second extension for all previews of file with the original file that matches first extension.  E.g.
		/// <code>
		/// ".txt | .htm"
		/// </code>
		/// Will preview all .txt files by generating the a temp file using the .htm extension.
		/// </para>
		/// <para>
		/// The second format that can be used for a string in the extensionMaps array is simply the file extension to be used for
		/// the preview and all original file extensions will be previewed using this preview file extension. E.g.
		/// <code>
		/// ".html"
		/// </code>
		/// </para>
		/// </param>
		/// <param name="originalFilePath">
		/// The original file path of the file being previewed.
		/// </param>
		/// <returns>
		/// A tempoary file name that can be used to generate a preview.
		/// </returns>
		public static string GetModifiedTempFilePath(string[] extensionMaps, string originalFilePath)
		{
			if (extensionMaps == null)
			{
				return null;
			}
			string text = string.Empty;
			if (!string.IsNullOrEmpty(originalFilePath))
			{
				text = Path.GetFileName(originalFilePath);
			}
			string path = string.IsNullOrEmpty(originalFilePath) ? string.Empty : Path.GetDirectoryName(originalFilePath);
			string extension = Path.GetExtension(text);
			string text2 = null;
			foreach (string text3 in extensionMaps)
			{
				string[] array = text3.Split("|".ToCharArray(), 2);
				array[0] = array[0].Trim();
				if (array.Length == 2)
				{
					array[1] = array[1].Trim();
					if (string.Compare(extension, array[0], ignoreCase: true) == 0 && array[1][0] == '.')
					{
						return Path.Combine(path, text + array[1]);
					}
				}
				else if (array.Length == 1 && array[0][0] == '.')
				{
					text2 = array[0];
				}
			}
			if (text2 != null && string.Compare(extension, text2, ignoreCase: true) != 0 && !text.EndsWith(text2))
			{
				text += text2;
			}
			return Path.Combine(path, text);
		}
	}
}
