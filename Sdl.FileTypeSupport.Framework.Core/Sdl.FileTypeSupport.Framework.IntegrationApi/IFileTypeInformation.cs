using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Text.RegularExpressions;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeInformation : IFileTypeDefinitionAware, IMetaDataContainer
	{
		FileTypeDefinitionId FileTypeDefinitionId
		{
			get;
			set;
		}

		LocalizableString FileTypeName
		{
			get;
			set;
		}

		LocalizableString FileTypeDocumentName
		{
			get;
			set;
		}

		LocalizableString FileTypeDocumentsName
		{
			get;
			set;
		}

		string FileDialogWildcardExpression
		{
			get;
			set;
		}

		Regex Expression
		{
			get;
		}

		string DefaultFileExtension
		{
			get;
			set;
		}

		LocalizableString Description
		{
			get;
			set;
		}

		bool IsBilingualDocumentFileType
		{
			get;
			set;
		}

		bool Enabled
		{
			get;
			set;
		}

		bool Hidden
		{
			get;
			set;
		}

		bool Removed
		{
			get;
			set;
		}

		IconDescriptor Icon
		{
			get;
			set;
		}

		Version FileTypeFrameworkVersion
		{
			get;
			set;
		}

		string[] SilverlightSettingsPageIds
		{
			get;
			set;
		}

		string[] WinFormSettingsPageIds
		{
			get;
			set;
		}
	}
}
