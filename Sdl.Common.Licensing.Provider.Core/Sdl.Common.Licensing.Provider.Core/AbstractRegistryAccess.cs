using Microsoft.Win32;

namespace Sdl.Common.Licensing.Provider.Core
{
	public abstract class AbstractRegistryAccess : IAbstractRegistryAccess
	{
		private readonly string _registryPath;

		protected AbstractRegistryAccess(string registryPath)
		{
			_registryPath = registryPath;
		}

		public virtual bool CanUpdateLicenseServerName()
		{
			return CanUpdateLicenseServerName(_registryPath);
		}

		protected bool CanUpdateLicenseServerName(string key)
		{
			bool flag = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key))
			{
				if (registryKey != null)
				{
					flag = true;
				}
			}
			if (flag)
			{
				try
				{
					using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(key, writable: true))
					{
						if (registryKey2 != null)
						{
							return true;
						}
					}
				}
				catch
				{
				}
				return false;
			}
			return true;
		}

		protected object GetKeyValue(string key)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(_registryPath);
			if (registryKey == null)
			{
				registryKey = Registry.CurrentUser.OpenSubKey(_registryPath);
			}
			if (registryKey == null)
			{
				return null;
			}
			using (registryKey)
			{
				return registryKey.GetValue(key);
			}
		}

		protected void SetKeyValue(string key, object keyValue)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(_registryPath, writable: true);
			}
			catch
			{
			}
			if (registryKey == null)
			{
				registryKey = Registry.LocalMachine.OpenSubKey(_registryPath);
				if (registryKey != null)
				{
					return;
				}
			}
			if (registryKey == null)
			{
				registryKey = Registry.CurrentUser.OpenSubKey(_registryPath, writable: true);
			}
			if (registryKey == null)
			{
				try
				{
					registryKey = Registry.LocalMachine.CreateSubKey(_registryPath);
				}
				catch
				{
				}
			}
			if (registryKey == null)
			{
				registryKey = Registry.CurrentUser.CreateSubKey(_registryPath);
			}
			if (registryKey != null)
			{
				using (registryKey)
				{
					if (keyValue != null)
					{
						registryKey.SetValue(key, keyValue);
					}
					else
					{
						if (registryKey.GetValue(key) != null)
						{
							registryKey.DeleteValue(key);
						}
						registryKey.Close();
					}
				}
			}
		}
	}
}
