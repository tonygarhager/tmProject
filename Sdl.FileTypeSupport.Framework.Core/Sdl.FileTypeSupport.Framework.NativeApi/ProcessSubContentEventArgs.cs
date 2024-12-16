using System;
using System.IO;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public class ProcessSubContentEventArgs : EventArgs
	{
		public string FileTypeDefinitionId
		{
			get;
			set;
		}

		public Stream SubContentStream
		{
			get;
			set;
		}

		public ProcessSubContentEventArgs(string fileTypeDefinitionId, Stream subContentStream)
		{
			FileTypeDefinitionId = fileTypeDefinitionId;
			SubContentStream = subContentStream;
		}
	}
}
