using System;
using System.Runtime.InteropServices;

namespace Sdl.LanguagePlatform.Stat
{
	internal class MMap
	{
		public enum FileMapMode
		{
			FILE_MAP_READ = 4,
			FILE_MAP_WRITE = 2,
			FILE_MAP_COPY = 1,
			FILE_MAP_ALL_ACCESS = 983071
		}

		public enum PageProtectionMode
		{
			PAGE_NOACCESS = 1,
			PAGE_READONLY = 2,
			PAGE_READWRITE = 4,
			PAGE_WRITECOPY = 8,
			PAGE_EXECUTE = 0x10
		}

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateFileMapping(IntPtr fileHandle, IntPtr securityAttributes, PageProtectionMode protectionMode, int maximumSizeHigh, int maximumSizeLow, string mapName);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(IntPtr fileMappingObject, FileMapMode desiredAccess, int offsetHigh, int offsetLow, int numberOfBytesToMap);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool UnmapViewOfFile(IntPtr map);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern int GetFileSize(IntPtr file, out int fileSize);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool GetFileSizeEx(IntPtr file, out long fileSize);
	}
}
