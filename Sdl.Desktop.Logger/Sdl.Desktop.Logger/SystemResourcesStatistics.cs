using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sdl.Desktop.Logger
{
	internal class SystemResourcesStatistics
	{
		private enum ResourceType
		{
			Gdi,
			User
		}

		private readonly Process _process;

		[DllImport("user32.dll")]
		private static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

		public SystemResourcesStatistics()
			: this(Process.GetCurrentProcess())
		{
		}

		public SystemResourcesStatistics(Process process)
		{
			_process = process;
		}

		public IDictionary<string, object> GetSystemResourcesStatisticsDictionary()
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ProcessName", GetProcessName());
			dictionary.Add("PhysicalMemorySize", GetPhysicalMemorySize());
			dictionary.Add("VirtualMemorySize", GetVirtualMemorySize());
			dictionary.Add("ThreadCount", GetThreadCount());
			dictionary.Add("HandleCount", GetHandleCount());
			dictionary.Add("GdiHandleCount", GetGdiHandleCount());
			dictionary.Add("UserHandleCount", GetUserHandleCount());
			dictionary.Add("TotalProcessorTime", GetTotalProcessorTime());
			return dictionary;
		}

		public string GetProcessName()
		{
			return _process.ProcessName;
		}

		public long GetPhysicalMemorySize()
		{
			return _process.WorkingSet64;
		}

		public long GetVirtualMemorySize()
		{
			return _process.PrivateMemorySize64;
		}

		public int GetThreadCount()
		{
			return _process.Threads.Count;
		}

		public int GetHandleCount()
		{
			return _process.HandleCount;
		}

		public int GetGdiHandleCount()
		{
			IntPtr handle = _process.Handle;
			uint guiResources = GetGuiResources(handle, 0u);
			return Convert.ToInt32(guiResources);
		}

		public int GetUserHandleCount()
		{
			IntPtr handle = _process.Handle;
			uint guiResources = GetGuiResources(handle, 1u);
			return Convert.ToInt32(guiResources);
		}

		public TimeSpan GetTotalProcessorTime()
		{
			return _process.TotalProcessorTime;
		}
	}
}
