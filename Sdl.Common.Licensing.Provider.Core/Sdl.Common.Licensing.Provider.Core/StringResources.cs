using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.Common.Licensing.Provider.Core
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class StringResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.Common.Licensing.Provider.Core.StringResources", typeof(StringResources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string ActivationCode => ResourceManager.GetString("ActivationCode", resourceCulture);

		internal static string AlternativeActivationOptionsControl_MainTitle => ResourceManager.GetString("AlternativeActivationOptionsControl_MainTitle", resourceCulture);

		internal static string AlternativeActivationOptionsControl_SubTitle => ResourceManager.GetString("AlternativeActivationOptionsControl_SubTitle", resourceCulture);

		internal static string BorrowLicenseStatusServerControl_BorrowedFromServer => ResourceManager.GetString("BorrowLicenseStatusServerControl_BorrowedFromServer", resourceCulture);

		internal static string BorrowLicenseStatusServerControl_ConfirmReturnLicense => ResourceManager.GetString("BorrowLicenseStatusServerControl_ConfirmReturnLicense", resourceCulture);

		internal static string BorrowLicenseStatusServerControl_ConfirmReturnLicense_Title => ResourceManager.GetString("BorrowLicenseStatusServerControl_ConfirmReturnLicense_Title", resourceCulture);

		internal static string ConnectionSettings_ProxyPortRange => ResourceManager.GetString("ConnectionSettings_ProxyPortRange", resourceCulture);

		internal static string ConnectionSettings_ProxyPortTitle => ResourceManager.GetString("ConnectionSettings_ProxyPortTitle", resourceCulture);

		internal static string ConnectionSettingsControl_Error_InvalidPortNumber => ResourceManager.GetString("ConnectionSettingsControl_Error_InvalidPortNumber", resourceCulture);

		internal static string ConnectionSettingsControl_Error_NoInternetConnection => ResourceManager.GetString("ConnectionSettingsControl_Error_NoInternetConnection", resourceCulture);

		internal static string ConnectionSettingsControl_Error_ObtainingProxySettings => ResourceManager.GetString("ConnectionSettingsControl_Error_ObtainingProxySettings", resourceCulture);

		internal static string ConnectionSettingsControl_FailedToConnectToInternet => ResourceManager.GetString("ConnectionSettingsControl_FailedToConnectToInternet", resourceCulture);

		internal static string ConnectionSettingsControl_InternetConnectionTest_Failed => ResourceManager.GetString("ConnectionSettingsControl_InternetConnectionTest_Failed", resourceCulture);

		internal static string ConnectionSettingsControl_InternetConnectionTest_Successful => ResourceManager.GetString("ConnectionSettingsControl_InternetConnectionTest_Successful", resourceCulture);

		internal static string ConnectionSettingsControl_InternetConnectionTest_Title => ResourceManager.GetString("ConnectionSettingsControl_InternetConnectionTest_Title", resourceCulture);

		internal static string ConnectionSettingsControl_SubTitle => ResourceManager.GetString("ConnectionSettingsControl_SubTitle", resourceCulture);

		internal static string ConnectionSettingsControl_Title => ResourceManager.GetString("ConnectionSettingsControl_Title", resourceCulture);

		internal static string DesktopLicensing_Activation_MessageBox_Title => ResourceManager.GetString("DesktopLicensing_Activation_MessageBox_Title", resourceCulture);

		internal static string DesktopLicensing_Activation_Successful => ResourceManager.GetString("DesktopLicensing_Activation_Successful", resourceCulture);

		internal static string DesktopLicensing_ButtonText_Cancel => ResourceManager.GetString("DesktopLicensing_ButtonText_Cancel", resourceCulture);

		internal static string DesktopLicensing_ButtonText_Confirm => ResourceManager.GetString("DesktopLicensing_ButtonText_Confirm", resourceCulture);

		internal static string DesktopLicensing_Deactivation_Error_UnableToObtainActivationCode => ResourceManager.GetString("DesktopLicensing_Deactivation_Error_UnableToObtainActivationCode", resourceCulture);

		internal static string DesktopLicensing_Error_InvalidActivationCertificate => ResourceManager.GetString("DesktopLicensing_Error_InvalidActivationCertificate", resourceCulture);

		internal static string DesktopLicensing_Error_InvalidActivationCode => ResourceManager.GetString("DesktopLicensing_Error_InvalidActivationCode", resourceCulture);

		internal static string DesktopLicensing_GenericError => ResourceManager.GetString("DesktopLicensing_GenericError", resourceCulture);

		internal static string DesktopLicensing_LicenseServerConfig_WaitPromptLabel => ResourceManager.GetString("DesktopLicensing_LicenseServerConfig_WaitPromptLabel", resourceCulture);

		internal static string DesktopLicensing_MessageBox_Title_Error => ResourceManager.GetString("DesktopLicensing_MessageBox_Title_Error", resourceCulture);

		internal static string DesktopLicensing_MessageBox_Title_Information => ResourceManager.GetString("DesktopLicensing_MessageBox_Title_Information", resourceCulture);

		internal static string DesktopLicensing_MessageBox_Title_Warning => ResourceManager.GetString("DesktopLicensing_MessageBox_Title_Warning", resourceCulture);

		internal static string DesktopLicensing_ReturningBorrowedLicense_Failed => ResourceManager.GetString("DesktopLicensing_ReturningBorrowedLicense_Failed", resourceCulture);

		internal static string DesktopLicensing_ReturningBorrowedLicense_MessageBox_Title => ResourceManager.GetString("DesktopLicensing_ReturningBorrowedLicense_MessageBox_Title", resourceCulture);

		internal static string DesktopLicensing_ReturningBorrowedLicense_Successful => ResourceManager.GetString("DesktopLicensing_ReturningBorrowedLicense_Successful", resourceCulture);

		internal static string DesktopLicensing_ReturningBorrowedLicense_WaitPromptLabel => ResourceManager.GetString("DesktopLicensing_ReturningBorrowedLicense_WaitPromptLabel", resourceCulture);

		internal static string DesktopLicensing_Status_EvaluationExpired => ResourceManager.GetString("DesktopLicensing_Status_EvaluationExpired", resourceCulture);

		internal static string DesktopLicensing_Status_EvaluationMinutesMode => ResourceManager.GetString("DesktopLicensing_Status_EvaluationMinutesMode", resourceCulture);

		internal static string DesktopLicensing_Status_EvaluationMode => ResourceManager.GetString("DesktopLicensing_Status_EvaluationMode", resourceCulture);

		internal static string DesktopLicensing_Status_IndeterminateLicenseStatus => ResourceManager.GetString("DesktopLicensing_Status_IndeterminateLicenseStatus", resourceCulture);

		internal static string DesktopLicensing_Status_Leased => ResourceManager.GetString("DesktopLicensing_Status_Leased", resourceCulture);

		internal static string DesktopLicensing_Status_Leased_LimitedUses => ResourceManager.GetString("DesktopLicensing_Status_Leased_LimitedUses", resourceCulture);

		internal static string DesktopLicensing_Status_LeaseExpired => ResourceManager.GetString("DesktopLicensing_Status_LeaseExpired", resourceCulture);

		internal static string DesktopLicensing_Status_Licensed => ResourceManager.GetString("DesktopLicensing_Status_Licensed", resourceCulture);

		internal static string DesktopLicensing_Status_Licensed_LimitedUses => ResourceManager.GetString("DesktopLicensing_Status_Licensed_LimitedUses", resourceCulture);

		internal static string DesktopLicensing_Status_Licensed_NoUsesLeft => ResourceManager.GetString("DesktopLicensing_Status_Licensed_NoUsesLeft", resourceCulture);

		internal static string DesktopLicensing_Status_LicenseMustBeReauthorized => ResourceManager.GetString("DesktopLicensing_Status_LicenseMustBeReauthorized", resourceCulture);

		internal static string DesktopLicensing_Status_LicenseRemoved => ResourceManager.GetString("DesktopLicensing_Status_LicenseRemoved", resourceCulture);

		internal static string DesktopLicensing_Status_LicenseWasReturnedToServer => ResourceManager.GetString("DesktopLicensing_Status_LicenseWasReturnedToServer", resourceCulture);

		internal static string DesktopLicensing_Status_NotLicensed => ResourceManager.GetString("DesktopLicensing_Status_NotLicensed", resourceCulture);

		internal static string DesktopLicensing_Status_ServerLicense => ResourceManager.GetString("DesktopLicensing_Status_ServerLicense", resourceCulture);

		internal static string Diagnostics_MainTitle => ResourceManager.GetString("Diagnostics_MainTitle", resourceCulture);

		internal static string Diagnostics_SubTitle => ResourceManager.GetString("Diagnostics_SubTitle", resourceCulture);

		internal static string Error_LicenseExpired => ResourceManager.GetString("Error_LicenseExpired", resourceCulture);

		internal static string Error_NoLicenseAvailable => ResourceManager.GetString("Error_NoLicenseAvailable", resourceCulture);

		internal static string ErrorFlexNetKey => ResourceManager.GetString("ErrorFlexNetKey", resourceCulture);

		internal static string LicenseManagerForm_ActivationFailed => ResourceManager.GetString("LicenseManagerForm_ActivationFailed", resourceCulture);

		internal static string LicenseManagerForm_DeactivationFailed => ResourceManager.GetString("LicenseManagerForm_DeactivationFailed", resourceCulture);

		internal static string LicenseManagerForm_DeactivationSuccessful => ResourceManager.GetString("LicenseManagerForm_DeactivationSuccessful", resourceCulture);

		internal static string LicenseServerActivationTitleBar => ResourceManager.GetString("LicenseServerActivationTitleBar", resourceCulture);

		internal static string LicenseServerConfigControl_AdminMessage => ResourceManager.GetString("LicenseServerConfigControl_AdminMessage", resourceCulture);

		internal static string LicenseServerConfigControl_AdminRequiredTooltip => ResourceManager.GetString("LicenseServerConfigControl_AdminRequiredTooltip", resourceCulture);

		internal static string LicenseServerConfigControl_InvalidWithTerminalServices => ResourceManager.GetString("LicenseServerConfigControl_InvalidWithTerminalServices", resourceCulture);

		internal static string LicenseServerConfigControl_MainTitle => ResourceManager.GetString("LicenseServerConfigControl_MainTitle", resourceCulture);

		internal static string LicenseServerConfigControl_Server_Connect_Error => ResourceManager.GetString("LicenseServerConfigControl_Server_Connect_Error", resourceCulture);

		internal static string LicenseServerConfigControl_SubTitle => ResourceManager.GetString("LicenseServerConfigControl_SubTitle", resourceCulture);

		internal static string LicenseStatusControl_Activate => ResourceManager.GetString("LicenseStatusControl_Activate", resourceCulture);

		internal static string LicenseStatusControl_ConfirmDeactivateProduct => ResourceManager.GetString("LicenseStatusControl_ConfirmDeactivateProduct", resourceCulture);

		internal static string LicenseStatusControl_ConfirmDeactivateProduct_Title => ResourceManager.GetString("LicenseStatusControl_ConfirmDeactivateProduct_Title", resourceCulture);

		internal static string LicenseStatusControl_DaysRemaining => ResourceManager.GetString("LicenseStatusControl_DaysRemaining", resourceCulture);

		internal static string LicenseStatusControl_Deactivate => ResourceManager.GetString("LicenseStatusControl_Deactivate", resourceCulture);

		internal static string LicenseStatusControl_EulaPenaltiesWarning1 => ResourceManager.GetString("LicenseStatusControl_EulaPenaltiesWarning1", resourceCulture);

		internal static string LicenseStatusControl_EulaPenaltiesWarning2 => ResourceManager.GetString("LicenseStatusControl_EulaPenaltiesWarning2", resourceCulture);

		internal static string LicenseStatusControl_LicError => ResourceManager.GetString("LicenseStatusControl_LicError", resourceCulture);

		internal static string LicenseStatusControl_LicErrorDetails => ResourceManager.GetString("LicenseStatusControl_LicErrorDetails", resourceCulture);

		internal static string LicenseStatusControl_LicErrorDetailsRetry => ResourceManager.GetString("LicenseStatusControl_LicErrorDetailsRetry", resourceCulture);

		internal static string LicenseStatusControl_MultipleInstancesRunning => ResourceManager.GetString("LicenseStatusControl_MultipleInstancesRunning", resourceCulture);

		internal static string LicenseStatusControl_PreviousVersionWarning1 => ResourceManager.GetString("LicenseStatusControl_PreviousVersionWarning1", resourceCulture);

		internal static string LicenseStatusControl_PreviousVersionWarning2 => ResourceManager.GetString("LicenseStatusControl_PreviousVersionWarning2", resourceCulture);

		internal static string LicenseStatusControl_ProductDeactivation_Title => ResourceManager.GetString("LicenseStatusControl_ProductDeactivation_Title", resourceCulture);

		internal static string LicenseStatusServerControl_BorrowedLicenseHasExpired => ResourceManager.GetString("LicenseStatusServerControl_BorrowedLicenseHasExpired", resourceCulture);

		internal static string LicenseStatusServerControl_CouldNotGetLic => ResourceManager.GetString("LicenseStatusServerControl_CouldNotGetLic", resourceCulture);

		internal static string LicenseStatusServerControl_DaysRemaining => ResourceManager.GetString("LicenseStatusServerControl_DaysRemaining", resourceCulture);

		internal static string LicenseStatusServerControl_Disconnect => ResourceManager.GetString("LicenseStatusServerControl_Disconnect", resourceCulture);

		internal static string LicenseStatusServerControl_Error_Cannot_Connect => ResourceManager.GetString("LicenseStatusServerControl_Error_Cannot_Connect", resourceCulture);

		internal static string LicenseStatusServerControl_Error_General => ResourceManager.GetString("LicenseStatusServerControl_Error_General", resourceCulture);

		internal static string LicenseStatusServerControl_Error_LicenseRevoked => ResourceManager.GetString("LicenseStatusServerControl_Error_LicenseRevoked", resourceCulture);

		internal static string LicenseStatusServerControl_Error_LicenseTamperingDetected => ResourceManager.GetString("LicenseStatusServerControl_Error_LicenseTamperingDetected", resourceCulture);

		internal static string LicenseStatusServerControl_Error_NetworkError_LicenseAlreadyBorrowed => ResourceManager.GetString("LicenseStatusServerControl_Error_NetworkError_LicenseAlreadyBorrowed", resourceCulture);

		internal static string LicenseStatusServerControl_Error_NetworkError_LicenseNotBorrowable => ResourceManager.GetString("LicenseStatusServerControl_Error_NetworkError_LicenseNotBorrowable", resourceCulture);

		internal static string LicenseStatusServerControl_Error_NetworkError_MultipleInstances => ResourceManager.GetString("LicenseStatusServerControl_Error_NetworkError_MultipleInstances", resourceCulture);

		internal static string LicenseStatusServerControl_Error_No_Seats => ResourceManager.GetString("LicenseStatusServerControl_Error_No_Seats", resourceCulture);

		internal static string LicenseStatusServerControl_NoLicenseAvailable => ResourceManager.GetString("LicenseStatusServerControl_NoLicenseAvailable", resourceCulture);

		internal static string LicenseStatusServerControl_Retry => ResourceManager.GetString("LicenseStatusServerControl_Retry", resourceCulture);

		internal static string LicenseStatusServerControl_Return => ResourceManager.GetString("LicenseStatusServerControl_Return", resourceCulture);

		internal static string LicenseTypeNotFound => ResourceManager.GetString("LicenseTypeNotFound", resourceCulture);

		internal static string LicensingDiagnostics_Running => ResourceManager.GetString("LicensingDiagnostics_Running", resourceCulture);

		internal static string LicensingProviderManager_ControlProviderFactoryNotFound => ResourceManager.GetString("LicensingProviderManager_ControlProviderFactoryNotFound", resourceCulture);

		internal static string LicensingProviderManager_FactoryNotFound => ResourceManager.GetString("LicensingProviderManager_FactoryNotFound", resourceCulture);

		internal static string NetworkLicenseExpired => ResourceManager.GetString("NetworkLicenseExpired", resourceCulture);

		internal static string NoConnection => ResourceManager.GetString("NoConnection", resourceCulture);

		internal static string NoConnectionTo => ResourceManager.GetString("NoConnectionTo", resourceCulture);

		internal static string NoNetworkLicenseFound => ResourceManager.GetString("NoNetworkLicenseFound", resourceCulture);

		internal static string OfflineActivationControl_ActivationFailed => ResourceManager.GetString("OfflineActivationControl_ActivationFailed", resourceCulture);

		internal static string OfflineActivationControl_ActivationFailed_General => ResourceManager.GetString("OfflineActivationControl_ActivationFailed_General", resourceCulture);

		internal static string OfflineActivationControl_MainTitle => ResourceManager.GetString("OfflineActivationControl_MainTitle", resourceCulture);

		internal static string OfflineActivationControl_SubTitle => ResourceManager.GetString("OfflineActivationControl_SubTitle", resourceCulture);

		internal static string OfflineDeactivationControl_DeactivatedLocally => ResourceManager.GetString("OfflineDeactivationControl_DeactivatedLocally", resourceCulture);

		internal static string OfflineDeactivationControl_DeactivationFailed => ResourceManager.GetString("OfflineDeactivationControl_DeactivationFailed", resourceCulture);

		internal static string OfflineDeactivationControl_MainTitle => ResourceManager.GetString("OfflineDeactivationControl_MainTitle", resourceCulture);

		internal static string OfflineDeactivationControl_SubTitle => ResourceManager.GetString("OfflineDeactivationControl_SubTitle", resourceCulture);

		internal static string OnlineActionFailWouldYouLikeToCheckConnectionSettings => ResourceManager.GetString("OnlineActionFailWouldYouLikeToCheckConnectionSettings", resourceCulture);

		internal static string OnlineActivationControl_MainTitle => ResourceManager.GetString("OnlineActivationControl_MainTitle", resourceCulture);

		internal static string OnlineActivationControl_SubTitle => ResourceManager.GetString("OnlineActivationControl_SubTitle", resourceCulture);

		internal static string OnlineActivationFailureSubMessage => ResourceManager.GetString("OnlineActivationFailureSubMessage", resourceCulture);

		internal static string OnlineDeactivationControl_MainTitle => ResourceManager.GetString("OnlineDeactivationControl_MainTitle", resourceCulture);

		internal static string OnlineDeactivationControl_SubTitle => ResourceManager.GetString("OnlineDeactivationControl_SubTitle", resourceCulture);

		internal static string OnlineDeactivationFailureSubMessage => ResourceManager.GetString("OnlineDeactivationFailureSubMessage", resourceCulture);

		internal static string PerFeatureNotImplemented => ResourceManager.GetString("PerFeatureNotImplemented", resourceCulture);

		internal static string ProductStatusActivation => ResourceManager.GetString("ProductStatusActivation", resourceCulture);

		internal static string PropertyValueNotDefined => ResourceManager.GetString("PropertyValueNotDefined", resourceCulture);

		internal static string RemoteDesktopMessage => ResourceManager.GetString("RemoteDesktopMessage", resourceCulture);

		internal static string RequireElevationForChangingTheServerName => ResourceManager.GetString("RequireElevationForChangingTheServerName", resourceCulture);

		internal static string ServerDoesNotExist => ResourceManager.GetString("ServerDoesNotExist", resourceCulture);

		internal static string ServerNameNetBiosError => ResourceManager.GetString("ServerNameNetBiosError", resourceCulture);

		internal static string ServerStatusControl_RetrievingLicense => ResourceManager.GetString("ServerStatusControl_RetrievingLicense", resourceCulture);

		internal static string StatusControl_Close => ResourceManager.GetString("StatusControl_Close", resourceCulture);

		internal static string StatusControl_ContinueTrial => ResourceManager.GetString("StatusControl_ContinueTrial", resourceCulture);

		internal static string StatusControl_Ok => ResourceManager.GetString("StatusControl_Ok", resourceCulture);

		internal static string ViewDeactivationCertificateControl_MainTitle => ResourceManager.GetString("ViewDeactivationCertificateControl_MainTitle", resourceCulture);

		internal static string ViewDeactivationCertificateControl_SubTitle => ResourceManager.GetString("ViewDeactivationCertificateControl_SubTitle", resourceCulture);

		internal static string YourActivationCode => ResourceManager.GetString("YourActivationCode", resourceCulture);

		internal StringResources()
		{
		}
	}
}
