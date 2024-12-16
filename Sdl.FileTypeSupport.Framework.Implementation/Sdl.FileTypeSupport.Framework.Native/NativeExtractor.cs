using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class NativeExtractor : INativeExtractor
	{
		private INativeFileParser _Parser;

		private INativeExtractionContentHandler _Output;

		private List<INativeExtractionContentProcessor> _Processors = new List<INativeExtractionContentProcessor>();

		private List<INativeExtractionContentProcessor> Processors => _Processors;

		public INativeFileParser Parser
		{
			get
			{
				return _Parser;
			}
			set
			{
				if (_Parser != null)
				{
					_Parser.Output = null;
				}
				_Parser = value;
				ReconnectComponents();
			}
		}

		public INativeExtractionContentHandler Output
		{
			get
			{
				return _Output;
			}
			set
			{
				_Output = value;
				ReconnectComponents();
			}
		}

		public IEnumerable<INativeExtractionContentProcessor> ContentProcessors => _Processors;

		public void ReconnectComponents()
		{
			INativeExtractionContentHandler output = _Output;
			for (int num = _Processors.Count - 1; num >= 0; num--)
			{
				_Processors[num].Output = output;
				output = _Processors[num];
			}
			if (_Parser != null)
			{
				_Parser.Output = output;
			}
		}

		public void AddProcessor(INativeExtractionContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			_Processors.Add(processor);
			ReconnectComponents();
		}

		public void InsertProcessor(int index, INativeExtractionContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			_Processors.Insert(index, processor);
			ReconnectComponents();
		}

		public bool RemoveProcessor(INativeExtractionContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			if (_Processors.Remove(processor))
			{
				processor.Output = null;
				ReconnectComponents();
				return true;
			}
			return false;
		}
	}
}
