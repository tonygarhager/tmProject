using Sdl.Core.PluginFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Common.Licensing.Provider.Core
{
	public sealed class LicensingProviderManager : ILicensingProviderManager
	{
		private static readonly Lazy<LicensingProviderManager> _instance = new Lazy<LicensingProviderManager>(() => new LicensingProviderManager());

		private IList<ILicensingProviderFactory> _factories;

		public static LicensingProviderManager Instance => _instance.Value;

		public IList<ILicensingProviderFactory> LicensingProviderFactories
		{
			get
			{
				if (_factories == null)
				{
					_factories = new ObjectRegistry<LicensingProviderFactoryAttribute, ILicensingProviderFactory>(PluginManager.DefaultPluginRegistry).CreateObjects();
				}
				return _factories;
			}
		}

		private LicensingProviderManager()
		{
		}

		public ILicensingProvider CreateProvider(ILicensingProviderConfiguration config, string preferredProviderId)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (!string.IsNullOrEmpty(preferredProviderId))
			{
				ILicensingProviderFactory licensingProviderFactory = LicensingProviderFactories.FirstOrDefault((ILicensingProviderFactory lf) => lf.ProviderId == preferredProviderId);
				if (licensingProviderFactory != null)
				{
					return licensingProviderFactory.CreateLicensingProvider(config);
				}
			}
			ILicensingProviderFactory licensingProviderFactory2 = LicensingProviderFactories.FirstOrDefault((ILicensingProviderFactory lf) => lf.ProviderId == config.ProviderId);
			if (licensingProviderFactory2 != null)
			{
				return licensingProviderFactory2.CreateLicensingProvider(config);
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, StringResources.LicensingProviderManager_FactoryNotFound, config.ProviderId));
		}

		public ILicensingControlsProvider CreateControlsProvider(ILicensingProviderConfiguration config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			ILicensingProviderFactory licensingProviderFactory = LicensingProviderFactories.FirstOrDefault((ILicensingProviderFactory lf) => lf.ProviderId == config.ProviderId);
			if (licensingProviderFactory != null)
			{
				return licensingProviderFactory.CreateLicensingControlsProvider(config);
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, StringResources.LicensingProviderManager_ControlProviderFactoryNotFound, config.ProviderId));
		}
	}
}
