using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IPreviewSets : ICollection<IPreviewSet>, IEnumerable<IPreviewSet>, IEnumerable
	{
		PreviewSetId? DefaultControlPreview
		{
			get;
			set;
		}

		PreviewSetId? DefaultApplicationPreview
		{
			get;
			set;
		}

		IPreviewSet FindFromId(PreviewSetId id);

		IEnumerable<IPreviewSet> GetControlPreviews();

		IEnumerable<IPreviewSet> GetApplicationPreviews();
	}
}
