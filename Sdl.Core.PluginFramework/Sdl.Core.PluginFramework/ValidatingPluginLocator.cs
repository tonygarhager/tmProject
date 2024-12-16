using Sdl.Core.PluginFramework.Implementation;
using Sdl.Core.PluginFramework.PackageSupport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Sdl.Core.PluginFramework
{
	public class ValidatingPluginLocator : IPluginLocator, IDisposable
	{
		private const string PLUGIN_PACKAGE_EXTENSION = ".sdlplugin";

		private const string OPEN_EXCHANGE_CERT_PATH = "Sdl.Core.PluginFramework.OpenXCert.cer";

		private const string SECURE_MODE_CERT_PATH = "Sdl.Core.PluginFramework.SecureModeCert.cer";

		private readonly IPluginLocator _baseLocator;

		private readonly X509Certificate _openExchangePublishingCertificate;

		private readonly X509Certificate _secureModeCertificate;

		private readonly bool _isSecureMode;

		public List<IPluginDescriptor> ValidatedDescriptors
		{
			get;
			private set;
		}

		public List<IPluginDescriptor> InvalidDescriptors
		{
			get;
			private set;
		}

		public ValidatingPluginLocator(IPluginLocator baseLocator)
			: this(baseLocator, isSecureMode: false)
		{
		}

		public ValidatingPluginLocator(IPluginLocator baseLocator, bool isSecureMode)
		{
			_baseLocator = (baseLocator ?? throw new ArgumentNullException("baseLocator"));
			_isSecureMode = isSecureMode;
			_openExchangePublishingCertificate = LoadCertificate("Sdl.Core.PluginFramework.OpenXCert.cer");
			_secureModeCertificate = LoadCertificate("Sdl.Core.PluginFramework.SecureModeCert.cer");
			ValidateThirdPartyPluginDescriptors();
		}

		private X509Certificate LoadCertificate(string certificatePath)
		{
			X509Certificate result = null;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(certificatePath))
			{
				if (stream == null)
				{
					return result;
				}
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
				result = new X509Certificate();
				result.Import(array);
				return result;
			}
		}

		private void ValidateThirdPartyPluginDescriptors()
		{
			ValidatedDescriptors = new List<IPluginDescriptor>();
			InvalidDescriptors = new List<IPluginDescriptor>();
			IPluginDescriptor[] pluginDescriptors = _baseLocator.GetPluginDescriptors();
			foreach (IPluginDescriptor pluginDescriptor in pluginDescriptors)
			{
				IThirdPartyPluginDescriptor thirdPartyPluginDescriptor = null;
				if ((thirdPartyPluginDescriptor = (pluginDescriptor as IThirdPartyPluginDescriptor)) != null)
				{
					string directoryName = Path.GetDirectoryName(thirdPartyPluginDescriptor.ThirdPartyManifestFilePath);
					string fileName = Path.GetFileName(directoryName);
					if (string.IsNullOrEmpty(XmlPluginConfig.Current.ThirdPartyPluginPackagesRelativePath))
					{
						continue;
					}
					int length = directoryName.IndexOf(XmlPluginConfig.Current.ThirdPartyPluginsRelativePath);
					string text = Path.Combine(Path.Combine(directoryName.Substring(0, length), XmlPluginConfig.Current.ThirdPartyPluginPackagesRelativePath), fileName + ".sdlplugin");
					if (!File.Exists(text) || !Directory.Exists(directoryName) || _openExchangePublishingCertificate == null || _secureModeCertificate == null)
					{
						continue;
					}
					bool flag = false;
					try
					{
						using (PluginPackage pluginPackage = new PluginPackage(text, FileAccess.Read))
						{
							if (((!_isSecureMode && pluginPackage.ValidateSignatures(_openExchangePublishingCertificate)) || pluginPackage.ValidateSignatures(_secureModeCertificate)) && pluginPackage.ComparePackageContentsTo(directoryName))
							{
								flag = true;
							}
						}
					}
					catch (Exception)
					{
						throw;
					}
					if (ThirdPartyPlugInReferencesAllowed(thirdPartyPluginDescriptor))
					{
						if (flag)
						{
							ValidatedDescriptors.Add(thirdPartyPluginDescriptor);
						}
						else
						{
							InvalidDescriptors.Add(pluginDescriptor);
						}
					}
					else
					{
						InvalidDescriptors.Add(pluginDescriptor);
					}
				}
				else if (pluginDescriptor is FileBasedPluginDescriptor)
				{
					Plugin plugin = new Plugin(null, pluginDescriptor, null, null);
					bool flag2 = true;
					bool flag3 = true;
					foreach (IExtension extension2 in plugin.Extensions)
					{
						Extension extension = extension2 as Extension;
						if (extension != null)
						{
							if (extension.ExtensionTypeName.IndexOf("PublicKeyToken=c28cdb26c445c888") == -1)
							{
								flag3 = false;
							}
							if (extension.ExtensionTypeName.IndexOf("PublicKeyToken=null") != -1)
							{
								flag2 = false;
							}
						}
					}
					if (flag3)
					{
						ValidatedDescriptors.Add(pluginDescriptor);
					}
					else if (flag2)
					{
					}
				}
				else
				{
					ValidatedDescriptors.Add(pluginDescriptor);
				}
			}
		}

		private bool ThirdPartyPlugInReferencesAllowed(IThirdPartyPluginDescriptor thirdPartyFileDescriptor)
		{
			string directoryName = Path.GetDirectoryName(thirdPartyFileDescriptor.ThirdPartyManifestFilePath);
			bool result = true;
			string[] files = Directory.GetFiles(directoryName, "*.dll", SearchOption.AllDirectories);
			foreach (string assemblyFile in files)
			{
				try
				{
					Assembly loadedDll = Assembly.LoadFrom(assemblyFile);
					if (!ValidateDllReferences(loadedDll, thirdPartyFileDescriptor))
					{
						result = false;
					}
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		private bool ValidateDllReferences(Assembly loadedDll, IThirdPartyPluginDescriptor descriptor)
		{
			AssemblyName[] referencedAssemblies = loadedDll.GetReferencedAssemblies();
			bool result = true;
			AssemblyName[] array = referencedAssemblies;
			foreach (AssemblyName referenced in array)
			{
				if (referenced.FullName.IndexOf("PublicKeyToken=c28cdb26c445c888") != -1)
				{
					InvalidSdlAssemblyReference invalidSdlAssemblyReference = null;
					KeyValuePair<string, Version> keyValuePair = XmlPluginConfig.Current.ApiVersions.FirstOrDefault((KeyValuePair<string, Version> v) => referenced.FullName.IndexOf(v.Key + ",") != -1);
					if (keyValuePair.Value == null)
					{
						invalidSdlAssemblyReference = new InvalidSdlAssemblyReference(referenced, SdlAssemblyReferenceError.SdlAssemblyNotPublic);
					}
					else if (referenced.Version.Major > keyValuePair.Value.Major || (referenced.Version.Major == keyValuePair.Value.Major && referenced.Version.Minor > keyValuePair.Value.Minor))
					{
						invalidSdlAssemblyReference = new InvalidSdlAssemblyReference(referenced, SdlAssemblyReferenceError.OlderApiVersionInstalled);
					}
					else if (referenced.Version.Major < keyValuePair.Value.Major)
					{
						invalidSdlAssemblyReference = new InvalidSdlAssemblyReference(referenced, SdlAssemblyReferenceError.NewerApiVersionInstalled);
					}
					if (invalidSdlAssemblyReference != null)
					{
						descriptor.InvalidSdlAssemblyReferences.Add(invalidSdlAssemblyReference);
						result = false;
					}
				}
			}
			return result;
		}

		public IPluginDescriptor[] GetPluginDescriptors()
		{
			if (ValidatedDescriptors == null)
			{
				throw new InvalidOperationException("_validatedDescriptors is null, must call ValidateThirdPartyPluginDescriptors()");
			}
			return ValidatedDescriptors.ToArray();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
