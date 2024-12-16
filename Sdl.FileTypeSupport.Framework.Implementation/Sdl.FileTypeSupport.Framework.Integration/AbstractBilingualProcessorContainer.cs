using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public abstract class AbstractBilingualProcessorContainer : AbstractFileTypeDefinitionComponent, IBilingualProcessorContainer, IBilingualContentHandler
	{
		private IBilingualContentHandler _output;

		private List<IBilingualContentProcessor> _publicBilingualProcessors = new List<IBilingualContentProcessor>();

		private Predicate<IPersistentFileConversionProperties> _fileRestriction;

		private Predicate<IParagraphUnitProperties> _paragraphUnitRestiction;

		private bool _ignoreThisFile;

		protected bool _isInitialized;

		public Predicate<IPersistentFileConversionProperties> FileRestriction
		{
			get
			{
				return _fileRestriction;
			}
			set
			{
				_fileRestriction = value;
			}
		}

		public Predicate<IParagraphUnitProperties> ParagraphUnitRestriction
		{
			get
			{
				return _paragraphUnitRestiction;
			}
			set
			{
				_paragraphUnitRestiction = value;
			}
		}

		protected List<IBilingualContentProcessor> PublicBilingualProcessors
		{
			get
			{
				return _publicBilingualProcessors;
			}
			set
			{
				_publicBilingualProcessors = value;
				ReconnectComponents();
			}
		}

		protected virtual IBilingualContentHandler FirstHandler
		{
			get
			{
				if (_publicBilingualProcessors.Count > 0)
				{
					return _publicBilingualProcessors[0];
				}
				return OutputHandler;
			}
		}

		protected virtual IBilingualContentHandler OutputHandler => Output;

		public List<IBilingualContentProcessor> BilingualProcessors
		{
			get
			{
				return _publicBilingualProcessors;
			}
			set
			{
				_publicBilingualProcessors = value;
			}
		}

		public virtual IBilingualContentHandler Output
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

		public virtual void ReconnectComponents()
		{
			IBilingualContentHandler output = OutputHandler;
			for (int num = _publicBilingualProcessors.Count; num > 0; num--)
			{
				_publicBilingualProcessors[num - 1].Output = output;
				output = _publicBilingualProcessors[num - 1];
			}
		}

		protected virtual List<IBilingualContentProcessor> GetAllIntermediateBilingualProcessors()
		{
			return _publicBilingualProcessors;
		}

		public virtual void AddBilingualProcessor(IBilingualContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			_publicBilingualProcessors.Add(processor);
		}

		public virtual void InsertBilingualProcessor(int index, IBilingualContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			_publicBilingualProcessors.Insert(index, processor);
		}

		public virtual bool RemoveBilingualProcessor(IBilingualContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			if (_publicBilingualProcessors.Remove(processor))
			{
				return true;
			}
			return false;
		}

		public virtual IEnumerable<IBilingualContentProcessor> GetBilingualProcessors()
		{
			return PublicBilingualProcessors;
		}

		protected void EnsureInitialized()
		{
			if (!_isInitialized)
			{
				ReconnectComponents();
				_isInitialized = true;
			}
		}

		public virtual void Initialize(IDocumentProperties documentInfo)
		{
			EnsureInitialized();
			_ignoreThisFile = false;
			FirstHandler?.Initialize(documentInfo);
		}

		public virtual void Complete()
		{
			EnsureInitialized();
			FirstHandler?.Complete();
			DisconnectComponents();
		}

		public virtual void DisconnectComponents()
		{
			for (int num = _publicBilingualProcessors.Count; num > 0; num--)
			{
				_publicBilingualProcessors[num - 1].Output = null;
			}
		}

		public virtual void SetFileProperties(IFileProperties fileInfo)
		{
			EnsureInitialized();
			if (_fileRestriction != null)
			{
				_ignoreThisFile = !_fileRestriction(fileInfo.FileConversionProperties);
			}
			if (!_ignoreThisFile)
			{
				FirstHandler?.SetFileProperties(fileInfo);
			}
		}

		public virtual void FileComplete()
		{
			EnsureInitialized();
			if (_ignoreThisFile)
			{
				_ignoreThisFile = false;
			}
			else
			{
				FirstHandler?.FileComplete();
			}
		}

		public virtual void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			EnsureInitialized();
			if (!_ignoreThisFile && (_paragraphUnitRestiction == null || _paragraphUnitRestiction(paragraphUnit.Properties)))
			{
				FirstHandler?.ProcessParagraphUnit(paragraphUnit);
			}
		}
	}
}
