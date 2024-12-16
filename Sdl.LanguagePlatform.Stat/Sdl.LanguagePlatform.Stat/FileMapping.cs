using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class FileMapping : IDisposable
	{
		private IntPtr _mapHandle;

		private FileStream _file;

		public FileMapping(string fileName)
		{
			_file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			if (_file.SafeFileHandle != null)
			{
				_mapHandle = MMap.CreateFileMapping(_file.SafeFileHandle.DangerousGetHandle(), IntPtr.Zero, MMap.PageProtectionMode.PAGE_READONLY, 0, 0, null);
			}
		}

		public void Dispose()
		{
			if (_mapHandle != IntPtr.Zero)
			{
				MMap.CloseHandle(_mapHandle);
				_mapHandle = IntPtr.Zero;
			}
			if (_file != null)
			{
				_file.Close();
				_file.Dispose();
				_file = null;
			}
		}
	}
}
