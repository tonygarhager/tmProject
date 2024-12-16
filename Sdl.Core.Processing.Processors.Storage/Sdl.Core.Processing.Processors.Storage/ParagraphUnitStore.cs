using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sdl.Core.Processing.Processors.Storage
{
	public class ParagraphUnitStore : IDisposable, IEnumerable<IParagraphUnit>, IEnumerable
	{
		[Serializable]
		private struct PositionAndLength
		{
			public readonly long Position;

			public readonly long Length;

			public PositionAndLength(long position, long length)
			{
				Position = position;
				Length = length;
			}
		}

		private FileStream _fs;

		private readonly BinaryFormatter _bf = new BinaryFormatter();

		private List<PositionAndLength> _index;

		private TempFileManager _fileManager;

		private string _storeFileName;

		private IParagraphUnit _firstPu;

		public int Count
		{
			get;
			private set;
		}

		public int NonStructureParagraphUnitCount
		{
			get;
			private set;
		}

		public int SegmentPairCount
		{
			get;
			private set;
		}

		public IParagraphUnit this[int index]
		{
			get
			{
				if (index == 0)
				{
					return _firstPu;
				}
				long length = _index[index].Length;
				byte[] buffer = new byte[length];
				_fs.Seek(_index[index].Position, SeekOrigin.Begin);
				_fs.Read(buffer, 0, (int)length);
				return Deserialise(buffer);
			}
		}

		public ParagraphUnitStore()
		{
			Initialise();
		}

		public ParagraphUnitStore(IEnumerable<IParagraphUnit> pus)
		{
			Initialise();
			foreach (IParagraphUnit pu in pus)
			{
				Add(pu);
			}
		}

		public void Initialise()
		{
			_fileManager = new TempFileManager();
			_storeFileName = _fileManager.FilePath;
			_fs = File.Open(_storeFileName, FileMode.CreateNew, FileAccess.ReadWrite);
			_index = new List<PositionAndLength>();
		}

		public void Add(IParagraphUnit pu)
		{
			IncrementCounts(pu);
			if (_index.Count == 0)
			{
				_firstPu = pu;
			}
			byte[] array = Serialise(pu);
			_index.Add(new PositionAndLength(_fs.Position, array.Length));
			_fs.Write(array, 0, array.Length);
			Count++;
		}

		private void IncrementCounts(IParagraphUnit pu)
		{
			if (!pu.IsStructure)
			{
				int num = ++NonStructureParagraphUnitCount;
				foreach (ISegmentPair segmentPair in pu.SegmentPairs)
				{
					if (segmentPair?.Source != null)
					{
						num = ++SegmentPairCount;
					}
				}
			}
		}

		internal byte[] Serialise(IParagraphUnit pu)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				_bf.Serialize(memoryStream, pu);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream.ToArray();
			}
		}

		internal IParagraphUnit Deserialise(byte[] buffer)
		{
			MemoryStream serializationStream = new MemoryStream(buffer);
			return (ParagraphUnit)_bf.Deserialize(serializationStream);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _index.Count; i++)
			{
				yield return this[i];
			}
		}

		public void Dispose()
		{
			if (_fs != null)
			{
				_fs.Close();
				_fs.Dispose();
			}
		}

		public IEnumerator<IParagraphUnit> GetEnumerator()
		{
			for (int i = 0; i < _index.Count; i++)
			{
				yield return this[i];
			}
		}
	}
}
