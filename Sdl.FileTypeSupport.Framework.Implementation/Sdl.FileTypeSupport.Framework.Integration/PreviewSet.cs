using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PreviewSet : IPreviewSet
	{
		private PreviewSetId _id;

		private LocalizableString _name;

		private LocalizableString _description;

		private IPreviewType _source;

		private IPreviewType _target;

		private IPreviewType _sideBySide;

		public PreviewSetId Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public LocalizableString Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public LocalizableString Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public PreviewSetKind PreviewSetKind
		{
			get
			{
				PreviewSetKind kind = PreviewSetKind.Undefined;
				UpdateKind(ref kind, _source);
				UpdateKind(ref kind, _sideBySide);
				UpdateKind(ref kind, _target);
				return kind;
			}
		}

		public IPreviewType Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}

		public IPreviewType SideBySide
		{
			get
			{
				return _sideBySide;
			}
			set
			{
				_sideBySide = value;
			}
		}

		public IPreviewType Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}

		public override string ToString()
		{
			return _name.ToString();
		}

		private void UpdateKind(ref PreviewSetKind kind, IPreviewType type)
		{
			if (type is IControlPreviewType)
			{
				switch (kind)
				{
				case PreviewSetKind.ApplicationPreviews:
					kind = PreviewSetKind.Mix;
					break;
				case PreviewSetKind.Undefined:
					kind = PreviewSetKind.ControlPreviews;
					break;
				}
			}
			if (type is IApplicationPreviewType)
			{
				switch (kind)
				{
				case PreviewSetKind.ApplicationPreviews:
				case PreviewSetKind.Mix:
					break;
				case PreviewSetKind.ControlPreviews:
					kind = PreviewSetKind.Mix;
					break;
				case PreviewSetKind.Undefined:
					kind = PreviewSetKind.ApplicationPreviews;
					break;
				}
			}
		}
	}
}
