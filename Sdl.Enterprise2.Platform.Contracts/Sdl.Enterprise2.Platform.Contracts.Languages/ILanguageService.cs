using Sdl.Core.Globalization;
using Sdl.Enterprise2.Platform.Contracts.Communication;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.Languages
{
	[ServiceContract(Name = "PlatformLanguageService", Namespace = "http://sdl.com/languages/2010")]
	public interface ILanguageService
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Language[] GetAvailableLanguages();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		string GetLanguageDisplayName(string isoAbbreviation);
	}
}
