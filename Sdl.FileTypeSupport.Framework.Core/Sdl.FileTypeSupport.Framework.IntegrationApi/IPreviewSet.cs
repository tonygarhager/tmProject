using Sdl.Core.Globalization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IPreviewSet
	{
		PreviewSetId Id
		{
			get;
			set;
		}

		LocalizableString Name
		{
			get;
			set;
		}

		LocalizableString Description
		{
			get;
			set;
		}

		PreviewSetKind PreviewSetKind
		{
			get;
		}

		IPreviewType Source
		{
			get;
			set;
		}

		IPreviewType SideBySide
		{
			get;
			set;
		}

		IPreviewType Target
		{
			get;
			set;
		}
	}
}
