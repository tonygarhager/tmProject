namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// An properties class that can be used as a properties holder for tempoary files created in order to perform a preview.
	/// </summary>
	public class TempFileProperties
	{
		private bool _readOnly;

		private bool _locked;

		private bool _singlePreview;

		private string[] _extensionMaps = new string[0];

		/// <summary>
		/// Set if preview file ReadOnly attribute should be set
		/// </summary>
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}

		/// <summary>
		/// Set if preview file should be locked to prevent deleting/writing by another process.
		/// </summary>
		public bool Locked
		{
			get
			{
				return _locked;
			}
			set
			{
				_locked = value;
			}
		}

		/// <summary>
		/// Set if only one preview is allowed for each document (FileId).
		/// </summary>
		public bool SinglePreview
		{
			get
			{
				return _singlePreview;
			}
			set
			{
				_singlePreview = value;
			}
		}

		/// <summary>
		/// A list of filter extensions that need to be mapped to other extensions before previewing.
		/// For example mappping .csv to .csv.txt files so they can be previewed in an IE browser control.
		/// </summary>
		public string[] ExtensionMaps
		{
			get
			{
				return _extensionMaps;
			}
			set
			{
				_extensionMaps = value;
			}
		}
	}
}
