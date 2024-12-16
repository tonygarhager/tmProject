using Sdl.Common.Licensing.Provider.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class LicenseManager
	{
		internal delegate void GetLicenseCompletedHandler(object sender, GetLicenseCompletedEventArgs e);

		public const string RequireElevatedRights = "REQUIRE_ELEVATED_RIGHTS";

		private readonly IList<ILicensingProviderConfiguration> _configurations;

		private ILicensingProvider _licProvider;

		private ILicensingControlsProvider _licControlsProvider;

		private IProductLicense _productLicense;

		private static string delayedErrorMessage;

		public ILicensingForm LicensingForm
		{
			get;
			private set;
		}

		public ILicensingProvider LicensingProvider => _licProvider;

		public ILicensingControlsProvider LicensingControlsProvider => _licControlsProvider;

		public bool CheckOutLicense
		{
			get;
			set;
		}

		public bool EnableServerLicensing
		{
			get;
			set;
		}

		public IProductLicense ProductLicense => GetProductLicense(throwOnError: false);

		public bool UnlockEnabled
		{
			get
			{
				if (_productLicense == null)
				{
					return false;
				}
				object property = _productLicense.GetProperty("Unlocked");
				return (dynamic)(property ?? ((object)false));
			}
		}

		public bool CanUpdateLicenseServerName => _licProvider.Configuration.Registry.CanUpdateLicenseServerName();

		public bool UseLicenseServer
		{
			get
			{
				return _licProvider.Configuration.Registry.UseLicenseServer;
			}
			set
			{
				_licProvider.Configuration.Registry.UseLicenseServer = value;
			}
		}

		public string LicenseServer
		{
			get
			{
				return _licProvider.Configuration.Registry.LicenseServer;
			}
			set
			{
				_licProvider.Configuration.Registry.LicenseServer = value;
			}
		}

		public bool IsLicenseServerRunning
		{
			get;
			set;
		}

		internal string ProxyAddress
		{
			get;
			set;
		}

		internal int ProxyPort
		{
			get;
			set;
		}

		public ILicensingHelpProvider LicensingHelpProvider
		{
			get
			{
				if (LicensingForm.LicensingFormConfiguration != null)
				{
					return LicensingForm.LicensingFormConfiguration.LicensingHelpProvider;
				}
				return null;
			}
		}

		public LicenseInfoProviderDelegate LicenseInfoProvider => Configuration?.LicenseInfoProvider;

		public IsLicenseValidForMachineDelegate IsLicenseValidForMachine => Configuration?.IsLicenseValidForMachine;

		public IsProductAllowedDelegate IsProductAllowed => Configuration?.IsProductAllowed;

		public string StatusPageTitle => Configuration?.StatusPageTitle;

		public string StatusPageSubtitle => Configuration?.StatusPageSubtitle;

		public Icon Icon => Configuration?.Icon;

		public Image TitleBarImage => Configuration?.TitleBarImage;

		public string PurchaseLinkUrl => Configuration?.PurchaseLinkUrl;

		public string MyAccountLinkUrl => Configuration?.MyAccountLinkUrl;

		public string ResetActivationLinkUrl => Configuration?.ResetActivationLinkUrl;

		internal LicensingFormConfiguration Configuration => LicensingForm?.LicensingFormConfiguration;

		internal event GetLicenseCompletedHandler GetLicenseCompleted;

		public LicenseManager(ILicensingProvider licProvider, IProductLicense productLicense, ILicensingForm licensingForm, IList<ILicensingProviderConfiguration> configurations, bool enableServerLicensing)
		{
			GetProxySettings();
			EnableServerLicensing = enableServerLicensing;
			CheckOutLicense = true;
			LicensingForm = licensingForm;
			_licProvider = licProvider;
			_productLicense = productLicense;
			_configurations = configurations;
			_licControlsProvider = LicensingProviderManager.Instance.CreateControlsProvider(licProvider.Configuration);
		}

		private IProductLicense CreateProductLicense()
		{
			return _licProvider.GetProducts().FirstOrDefault();
		}

		public IProductLicense GetProductLicense(bool throwOnError = true)
		{
			if (_productLicense != null)
			{
				return _productLicense;
			}
			try
			{
				_productLicense = CreateProductLicense();
			}
			catch
			{
				if (throwOnError)
				{
					throw;
				}
			}
			return _productLicense;
		}

		public void Activate(string activationCode)
		{
			ILicensingProviderFactory factory = LicensingProviderManager.Instance.LicensingProviderFactories.Single((ILicensingProviderFactory lf) => lf.IsActivationCode(activationCode));
			ILicensingProvider licensingProvider = factory.CreateLicensingProvider(_configurations.SingleOrDefault((ILicensingProviderConfiguration lpc) => lpc.ProviderId == factory.ProviderId));
			IProductLicense license = licensingProvider.Activate(activationCode);
			licensingProvider.Configuration.Registry.LicenseCode = activationCode;
			licensingProvider.Configuration.Registry.DeactivationCode = null;
			SetCurrentProductLicense(licensingProvider, license);
		}

		public void Deactivate(string activationCode)
		{
			_licProvider.Deactivate(activationCode);
			_productLicense = CreateProductLicense();
			_licProvider.Configuration.Registry.LicenseCode = null;
		}

		public List<string> GetFeaturesAvailableOnServer(string serverName)
		{
			List<string> list = new List<string>();
			foreach (ILicensingProviderFactory factory in LicensingProviderManager.Instance.LicensingProviderFactories)
			{
				if (factory.IsValidServerName(serverName))
				{
					ILicensingProviderConfiguration licensingProviderConfiguration = _configurations.SingleOrDefault((ILicensingProviderConfiguration lpc) => lpc.ProviderId == factory.ProviderId);
					try
					{
						if (licensingProviderConfiguration != null)
						{
							licensingProviderConfiguration.Registry.UseLicenseServer = true;
							licensingProviderConfiguration.Registry.LicenseServer = serverName;
							ILicensingProvider licensingProvider = factory.CreateLicensingProvider(licensingProviderConfiguration);
							IList<IProductLicense> products = licensingProvider.GetProducts();
							if (products?.Any() ?? false)
							{
								IProductLicense productLicense = products.FirstOrDefault();
								if (productLicense != null)
								{
									list.AddRange(from feature in productLicense.GetFeatures()
										select feature.Name);
								}
							}
						}
					}
					catch (Exception message)
					{
						MessageLog.DefaultLog.Error(message);
					}
				}
			}
			return list;
		}

		public void NetworkActivate(string serverName)
		{
			foreach (ILicensingProviderFactory factory in LicensingProviderManager.Instance.LicensingProviderFactories)
			{
				if (factory.IsValidServerName(serverName))
				{
					string licenseServer = string.Empty;
					ILicensingProviderConfiguration licensingProviderConfiguration = _configurations.SingleOrDefault((ILicensingProviderConfiguration lpc) => lpc.ProviderId == factory.ProviderId);
					try
					{
						if (licensingProviderConfiguration != null)
						{
							licenseServer = licensingProviderConfiguration.Registry.LicenseServer;
							licensingProviderConfiguration.Registry.UseLicenseServer = true;
							licensingProviderConfiguration.Registry.LicenseServer = serverName;
							string checkedOutEdition = licensingProviderConfiguration.Registry.CheckedOutEdition;
							ILicensingProvider licensingProvider = factory.CreateLicensingProvider(licensingProviderConfiguration);
							IList<IProductLicense> products = licensingProvider.GetProducts();
							if (products?.Any() ?? false)
							{
								IProductLicense productLicense = products.FirstOrDefault();
								productLicense.GetFeaturesToCheckout(checkedOutEdition);
								productLicense.CheckOut();
								if (productLicense.Status == LicenseStatus.LeaseExpired)
								{
									delayedErrorMessage = string.Format(StringResources.NetworkLicenseExpired, serverName);
									SetCurrentProductLicense(licensingProvider, productLicense);
									goto IL_024e;
								}
								if (productLicense.IsLoggedIn)
								{
									SetCurrentProductLicense(licensingProvider, productLicense);
									return;
								}
								licensingProviderConfiguration.Registry.LicenseServer = licenseServer;
							}
						}
					}
					catch (LicensingProviderException ex)
					{
						MessageLog.DefaultLog.Error(ex);
						switch (ex.ErrorCode)
						{
						case 4L:
							licensingProviderConfiguration.Registry.UseLicenseServer = false;
							licensingProviderConfiguration.Registry.CurrentLicensingProvider = null;
							_productLicense = null;
							delayedErrorMessage = StringResources.LicenseStatusServerControl_Error_No_Seats;
							break;
						case 32L:
							licensingProviderConfiguration.Registry.UseLicenseServer = false;
							licensingProviderConfiguration.Registry.CurrentLicensingProvider = null;
							_productLicense = null;
							throw;
						}
					}
					catch (Exception ex2)
					{
						MessageLog.DefaultLog.Error(ex2);
						if (ex2.Message == "REQUIRE_ELEVATED_RIGHTS")
						{
							delayedErrorMessage = StringResources.RequireElevationForChangingTheServerName;
						}
						licensingProviderConfiguration.Registry.LicenseServer = licenseServer;
						licensingProviderConfiguration.Registry.CurrentLicensingProvider = null;
						_productLicense = null;
					}
				}
			}
			goto IL_024e;
			IL_024e:
			UseLicenseServer = false;
			if (!string.IsNullOrEmpty(delayedErrorMessage))
			{
				Helpers.ShowError(delayedErrorMessage, StringResources.LicenseServerActivationTitleBar);
				delayedErrorMessage = null;
				return;
			}
			throw new LicensingProviderException(string.Format(StringResources.NoNetworkLicenseFound, serverName));
		}

		public void OfflineActivate(string activationCode, string activationCertificate)
		{
			ILicensingProviderFactory factory = LicensingProviderManager.Instance.LicensingProviderFactories.Single((ILicensingProviderFactory lf) => lf.IsActivationCode(activationCode));
			ILicensingProvider licensingProvider = factory.CreateLicensingProvider(_configurations.SingleOrDefault((ILicensingProviderConfiguration lpc) => lpc.ProviderId == factory.ProviderId));
			IProductLicense license = licensingProvider.OfflineActivate(activationCertificate);
			SetCurrentProductLicense(licensingProvider, license);
			licensingProvider.Configuration.Registry.LicenseCode = activationCode;
			licensingProvider.Configuration.Registry.DeactivationCode = null;
		}

		public string OfflineDeactivate(string activationCode)
		{
			string text = _licProvider.OfflineDeactivate(activationCode);
			_productLicense = CreateProductLicense();
			_licProvider.Configuration.Registry.DeactivationCode = text;
			_licProvider.Configuration.Registry.LicenseCode = null;
			return text;
		}

		public string GetInstallationId(string activationCode)
		{
			ILicensingProviderFactory factory = LicensingProviderManager.Instance.LicensingProviderFactories.Single((ILicensingProviderFactory lf) => lf.IsActivationCode(activationCode));
			ILicensingProvider licensingProvider = factory.CreateLicensingProvider(_configurations.SingleOrDefault((ILicensingProviderConfiguration lpc) => lpc.ProviderId == factory.ProviderId));
			return licensingProvider.GetInstallationId();
		}

		public void ClearCachedLicense()
		{
			if (_productLicense != null)
			{
				ReturnLicense();
				_productLicense.Dispose();
				_productLicense = null;
			}
		}

		public void ReturnLicense()
		{
			if (_productLicense != null)
			{
				_productLicense.CheckIn();
			}
		}

		public void ResetCurrentLicense()
		{
			ClearCachedLicense();
		}

		public void GetLicense()
		{
			GetLicense(CheckOutLicense);
		}

		private void GetLicense(bool checkOutLicense)
		{
			try
			{
				if (ProductLicense != null)
				{
					if (checkOutLicense)
					{
						ProductLicense.CheckOut();
					}
					else
					{
						ProductLicense.CheckOut();
						ProductLicense.CheckIn();
					}
				}
			}
			catch (Exception)
			{
				if (_productLicense != null)
				{
					_productLicense.Dispose();
					_productLicense = null;
				}
				throw;
			}
		}

		public void GetLicenseWithoutConsumingSeatsOrUsages()
		{
			GetLicense(checkOutLicense: false);
		}

		private void GetProxySettings()
		{
			string proxyUrl = null;
			int port = 0;
			NetworkConnectivity.GetProxyDetails(ref proxyUrl, ref port);
			ProxyAddress = proxyUrl;
			ProxyPort = port;
		}

		public void ShowContextHelp(LicensingHelpIDs LicensingHelpID)
		{
			if (LicensingHelpProvider != null)
			{
				LicensingHelpProvider.ShowHelp(LicensingHelpID);
			}
		}

		public bool CallIsLicenseValidForMachine(out string shortStatus, out string longerWarningMessage)
		{
			shortStatus = string.Empty;
			longerWarningMessage = string.Empty;
			return IsLicenseValidForMachine == null || IsLicenseValidForMachine(_productLicense, out shortStatus, out longerWarningMessage);
		}

		internal string GetLicenseInfo()
		{
			string result;
			if (LicenseInfoProvider != null && (result = LicenseInfoProvider(_productLicense)) != null)
			{
				return result;
			}
			string text = Helper.GetStatus(ProductLicense.ModeDetail);
			dynamic property = ProductLicense.GetProperty("CheckOutFailReason");
			if (property != null)
			{
				text = (string)(text + (" [" + property + "]"));
			}
			return text;
		}

		internal void GetLicenseAsync()
		{
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += GetLicenseAsync_DoWork;
			backgroundWorker.RunWorkerCompleted += GetLicenseAsync_RunWorkerCompleted;
			backgroundWorker.RunWorkerAsync();
		}

		private void GetLicenseAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			RaiseGetLicenseCompleted(e.Error);
			(sender as BackgroundWorker).DoWork -= GetLicenseAsync_DoWork;
			(sender as BackgroundWorker).RunWorkerCompleted -= GetLicenseAsync_RunWorkerCompleted;
		}

		private void GetLicenseAsync_DoWork(object sender, DoWorkEventArgs e)
		{
			ClearCachedLicense();
			GetLicense();
		}

		private void RaiseGetLicenseCompleted(Exception error)
		{
			GetLicenseCompletedHandler getLicenseCompleted = this.GetLicenseCompleted;
			if (getLicenseCompleted != null)
			{
				this.GetLicenseCompleted(this, new GetLicenseCompletedEventArgs(error));
			}
		}

		private void SetCurrentProductLicense(ILicensingProvider provider, IProductLicense license)
		{
			_licProvider = provider;
			_licControlsProvider = LicensingProviderManager.Instance.CreateControlsProvider(provider.Configuration);
			_productLicense = license;
			provider.Configuration.Registry.CurrentLicensingProvider = provider.Id;
		}
	}
}
