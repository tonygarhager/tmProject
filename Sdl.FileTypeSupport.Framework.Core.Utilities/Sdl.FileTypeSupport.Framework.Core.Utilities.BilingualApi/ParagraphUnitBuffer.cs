using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class ParagraphUnitBuffer
	{
		private bool _isHolding;

		private List<IParagraphUnit> _bufferedParagraphUnits = new List<IParagraphUnit>();

		private IBilingualContentHandler _output;

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

		public List<IParagraphUnit> BufferedParagraphUnits => _bufferedParagraphUnits;

		public bool IsHolding => _isHolding;

		public bool IsEmpty => _bufferedParagraphUnits.Count == 0;

		public ParagraphUnitBuffer()
		{
		}

		public ParagraphUnitBuffer(IBilingualContentHandler output)
		{
			_output = output;
		}

		public void Hold()
		{
			_isHolding = true;
		}

		public void Release()
		{
			foreach (IParagraphUnit bufferedParagraphUnit in _bufferedParagraphUnits)
			{
				OutputParagraphUnit(bufferedParagraphUnit);
			}
			_bufferedParagraphUnits.Clear();
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
				_bufferedParagraphUnits.Add(pu);
			}
			else
			{
				OutputParagraphUnit(pu);
			}
		}
	}
}
