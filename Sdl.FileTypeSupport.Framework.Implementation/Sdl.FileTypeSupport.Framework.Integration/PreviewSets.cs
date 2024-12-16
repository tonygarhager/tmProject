using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PreviewSets : List<IPreviewSet>, IPreviewSets, ICollection<IPreviewSet>, IEnumerable<IPreviewSet>, IEnumerable
	{
		private PreviewSetId? _defaultControlPreview;

		private PreviewSetId? _defaultApplicationPreview;

		public PreviewSetId? DefaultControlPreview
		{
			get
			{
				return _defaultControlPreview;
			}
			set
			{
				_defaultControlPreview = value;
			}
		}

		public PreviewSetId? DefaultApplicationPreview
		{
			get
			{
				return _defaultApplicationPreview;
			}
			set
			{
				_defaultApplicationPreview = value;
			}
		}

		public PreviewSets()
		{
		}

		public PreviewSets(params PreviewSet[] previewSets)
		{
			CreatePreviewSets(previewSets);
		}

		private void CreatePreviewSets(IList<PreviewSet> previewSets)
		{
			foreach (PreviewSet previewSet in previewSets)
			{
				Add(previewSet);
			}
		}

		public IPreviewSet FindFromId(PreviewSetId id)
		{
			return Find((IPreviewSet type) => type.Id == id);
		}

		public IEnumerable<IPreviewSet> GetControlPreviews()
		{
			return this.Where((IPreviewSet previewSet) => previewSet.PreviewSetKind == PreviewSetKind.ControlPreviews);
		}

		public IEnumerable<IPreviewSet> GetApplicationPreviews()
		{
			return this.Where((IPreviewSet previewSet) => previewSet.PreviewSetKind == PreviewSetKind.ApplicationPreviews);
		}
	}
}
