using System;
using System.ServiceProcess;

namespace Sdl.Common.Licensing.Provider.Core.Helpers
{
	public class ServiceManager
	{
		private const int Timeout = 60;

		private readonly ServiceController service;

		public string Status => Exists ? service.Status.ToString() : "Not Installed";

		public bool IsStopped => service.Status == ServiceControllerStatus.Stopped;

		public bool Exists
		{
			get
			{
				try
				{
					return !string.IsNullOrEmpty(service.DisplayName);
				}
				catch
				{
					return false;
				}
			}
		}

		public ServiceManager(string serviceName, string computerName)
		{
			if (string.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentNullException("serviceName");
			}
			service = new ServiceController(serviceName, computerName);
		}

		public ServiceManager(string serviceName)
		{
			if (string.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentNullException("serviceName");
			}
			service = new ServiceController(serviceName);
		}

		public bool ReStart()
		{
			if (!Exists)
			{
				return false;
			}
			try
			{
				TimeSpan timeout = TimeSpan.FromSeconds(60.0);
				if (service.Status == ServiceControllerStatus.Running)
				{
					MessageLog.DefaultLog.DebugFormat("Stopping service {0}.", service.DisplayName);
					service.Stop();
					service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
				}
				MessageLog.DefaultLog.DebugFormat("Starting service {0}.", service.DisplayName);
				service.Start();
				service.WaitForStatus(ServiceControllerStatus.Running, timeout);
				return true;
			}
			catch (Exception ex)
			{
				MessageLog.DefaultLog.DebugFormat("Failed to restart service {0}. {1}", service.DisplayName, ex.Message);
				return false;
			}
		}

		public bool Start()
		{
			if (Exists)
			{
				try
				{
					TimeSpan timeout = TimeSpan.FromSeconds(60.0);
					if (service.Status == ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.StopPending)
					{
						service.Start();
						service.WaitForStatus(ServiceControllerStatus.Running, timeout);
					}
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		public bool Stop()
		{
			if (Exists)
			{
				TimeSpan timeout = TimeSpan.FromSeconds(60.0);
				if (service.Status == ServiceControllerStatus.Running)
				{
					service.Stop();
					service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
				}
				return true;
			}
			return false;
		}
	}
}
