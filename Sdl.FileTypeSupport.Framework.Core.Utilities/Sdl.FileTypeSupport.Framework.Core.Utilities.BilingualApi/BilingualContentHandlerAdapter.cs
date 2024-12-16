using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class BilingualContentHandlerAdapter : IBilingualContentProcessor, IBilingualContentHandler, IBilingualFileTypeComponent, IBilingualDocumentOutputPropertiesAware, ISharedObjectsAware, ISettingsAware
	{
		private IBilingualContentHandler _output;

		private IBilingualContentHandler _impl;

		public IBilingualContentHandler Impl
		{
			get
			{
				return _impl;
			}
			set
			{
				_impl = value;
			}
		}

		public IBilingualContentHandler Output
		{
			get
			{
				return _output;
			}
			set
			{
				_output = value;
			}
		}

		public IDocumentItemFactory ItemFactory
		{
			get
			{
				return (_impl as IBilingualFileTypeComponent)?.ItemFactory;
			}
			set
			{
				IBilingualFileTypeComponent bilingualFileTypeComponent = _impl as IBilingualFileTypeComponent;
				if (bilingualFileTypeComponent != null)
				{
					bilingualFileTypeComponent.ItemFactory = value;
				}
			}
		}

		public IBilingualContentMessageReporter MessageReporter
		{
			get
			{
				return (_impl as IBilingualFileTypeComponent)?.MessageReporter;
			}
			set
			{
				IBilingualFileTypeComponent bilingualFileTypeComponent = _impl as IBilingualFileTypeComponent;
				if (bilingualFileTypeComponent != null)
				{
					bilingualFileTypeComponent.MessageReporter = value;
				}
			}
		}

		public BilingualContentHandlerAdapter()
		{
		}

		public BilingualContentHandlerAdapter(IBilingualContentHandler impl)
		{
			_impl = impl;
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			if (_impl != null)
			{
				_impl.Initialize(documentInfo);
			}
			if (_output != null)
			{
				_output.Initialize(documentInfo);
			}
		}

		public void Complete()
		{
			if (_impl != null)
			{
				_impl.Complete();
			}
			if (_output != null)
			{
				_output.Complete();
			}
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			if (_impl != null)
			{
				_impl.SetFileProperties(fileInfo);
			}
			if (_output != null)
			{
				_output.SetFileProperties(fileInfo);
			}
		}

		public void FileComplete()
		{
			if (_impl != null)
			{
				_impl.FileComplete();
			}
			if (_output != null)
			{
				_output.FileComplete();
			}
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (_impl != null)
			{
				_impl.ProcessParagraphUnit(paragraphUnit);
			}
			if (_output != null)
			{
				_output.ProcessParagraphUnit(paragraphUnit);
			}
		}

		public void GetProposedFileInfo(IDocumentProperties documentInfo, IOutputFileInfo proposedFileInfo)
		{
			(_impl as IBilingualDocumentOutputPropertiesAware)?.GetProposedFileInfo(documentInfo, proposedFileInfo);
		}

		public void SetOutputProperties(IBilingualDocumentOutputProperties outputProperties)
		{
			(_impl as IBilingualDocumentOutputPropertiesAware)?.SetOutputProperties(outputProperties);
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			(_impl as ISharedObjectsAware)?.SetSharedObjects(sharedObjects);
		}

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{
			(_impl as ISettingsAware)?.InitializeSettings(settingsBundle, configurationId);
		}
	}
}
