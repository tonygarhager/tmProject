using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class NativeSubContentGenerator : AbstractNativeGenerationContentProcessor, INativeExtractionContentHandler, IAbstractNativeContentHandler, INativeSubContentGenerator
	{
		private INativeFileWriter _Writer;

		private List<INativeGenerationContentProcessor> _Processors = new List<INativeGenerationContentProcessor>();

		private List<INativeGenerationContentProcessor> Processors => _Processors;

		public INativeGenerationContentHandler Input => this;

		public INativeFileWriter Writer
		{
			get
			{
				return _Writer;
			}
			set
			{
				_Writer = value;
				ReconnectComponents();
			}
		}

		public IEnumerable<INativeGenerationContentProcessor> ContentProcessors => _Processors;

		public void ReconnectComponents()
		{
			INativeGenerationContentHandler output = _Writer;
			for (int num = _Processors.Count - 1; num >= 0; num--)
			{
				_Processors[num].Output = output;
				output = _Processors[num];
			}
			Output = output;
		}

		public void AddProcessor(INativeGenerationContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			_Processors.Add(processor);
			ReconnectComponents();
		}

		public void InsertProcessor(int index, INativeGenerationContentProcessor processor)
		{
			if (processor == null)
			{
				throw new ArgumentNullException("processor");
			}
			_Processors.Insert(index, processor);
			ReconnectComponents();
		}

		public bool RemoveProcessor(INativeGenerationContentProcessor processor)
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

		public INativeGenerationContentProcessor[] GetContentProcessors()
		{
			return _Processors.ToArray();
		}
	}
}
