using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class BufferedBilingualContentHandler : IBilingualContentHandler
	{
		internal abstract class BufferEntry
		{
		}

		internal class BufferEntryParagraphUnit : BufferEntry
		{
			public IParagraphUnit ParagraphUnit
			{
				get;
				set;
			}
		}

		internal class BufferEntryInitialize : BufferEntry
		{
			public IDocumentProperties DocInfo
			{
				get;
				set;
			}
		}

		internal class BufferEntryComplete : BufferEntry
		{
		}

		internal class BufferEntryStartFile : BufferEntry
		{
			public IFileProperties FileInfo
			{
				get;
				set;
			}
		}

		internal class BufferEntryEndFile : BufferEntry
		{
		}

		private bool _isHolding;

		private List<BufferEntry> _bufferedEntries = new List<BufferEntry>();

		private IBilingualContentHandler _output;

		public IBilingualContentHandler Output
		{
			get
			{
				if (_isHolding)
				{
					return this;
				}
				return _output;
			}
			set
			{
				_output = value;
			}
		}

		public List<IParagraphUnit> BufferedParagraphUnits
		{
			get
			{
				List<IParagraphUnit> list = new List<IParagraphUnit>();
				foreach (BufferEntry bufferedEntry in _bufferedEntries)
				{
					if (bufferedEntry is BufferEntryParagraphUnit)
					{
						list.Add(((BufferEntryParagraphUnit)bufferedEntry).ParagraphUnit);
					}
				}
				return list;
			}
		}

		public bool IsHolding => _isHolding;

		public bool IsEmpty => BufferedParagraphUnits.Count == 0;

		public BufferedBilingualContentHandler()
		{
		}

		public BufferedBilingualContentHandler(IBilingualContentHandler output)
		{
			_output = output;
		}

		public void Hold()
		{
			_isHolding = true;
		}

		public void Release()
		{
			foreach (BufferEntry bufferedEntry in _bufferedEntries)
			{
				if (bufferedEntry is BufferEntryParagraphUnit)
				{
					OutputParagraphUnit(((BufferEntryParagraphUnit)bufferedEntry).ParagraphUnit);
				}
				if (_output != null)
				{
					if (bufferedEntry is BufferEntryInitialize)
					{
						IDocumentProperties docInfo = ((BufferEntryInitialize)bufferedEntry).DocInfo;
						_output.Initialize(docInfo);
					}
					if (bufferedEntry is BufferEntryStartFile)
					{
						IFileProperties fileInfo = ((BufferEntryStartFile)bufferedEntry).FileInfo;
						_output.SetFileProperties(fileInfo);
					}
					if (bufferedEntry is BufferEntryEndFile)
					{
						_output.FileComplete();
					}
					if (bufferedEntry is BufferEntryComplete)
					{
						_output.Complete();
					}
				}
			}
			_bufferedEntries.Clear();
			_isHolding = false;
		}

		private void OutputParagraphUnit(IParagraphUnit pu)
		{
			if (_output != null)
			{
				_output.ProcessParagraphUnit(pu);
			}
		}

		public void ProcessParagraphUnit(IParagraphUnit pu)
		{
			if (pu == null)
			{
				throw new ArgumentNullException("pu");
			}
			if (_isHolding)
			{
				_bufferedEntries.Add(new BufferEntryParagraphUnit
				{
					ParagraphUnit = pu
				});
			}
			else
			{
				OutputParagraphUnit(pu);
			}
		}

		public void Complete()
		{
			if (_isHolding)
			{
				_bufferedEntries.Add(new BufferEntryComplete());
			}
			else
			{
				_output.Complete();
			}
		}

		public void FileComplete()
		{
			if (_isHolding)
			{
				_bufferedEntries.Add(new BufferEntryEndFile());
			}
			else
			{
				_output.FileComplete();
			}
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			if (_isHolding)
			{
				_bufferedEntries.Add(new BufferEntryInitialize
				{
					DocInfo = documentInfo
				});
			}
			else
			{
				_output.Initialize(documentInfo);
			}
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			if (_isHolding)
			{
				_bufferedEntries.Add(new BufferEntryStartFile
				{
					FileInfo = fileInfo
				});
			}
			else
			{
				_output.SetFileProperties(fileInfo);
			}
		}
	}
}
