using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class NativeExtractionMarkInserter : LineNumberTracker, INativeContentCycleAware
	{
		private List<Pair<LocationMarkerId, NativeTextLocation>> _markLocations;

		public List<Pair<LocationMarkerId, NativeTextLocation>> MarkLocations
		{
			get
			{
				return _markLocations;
			}
			set
			{
				_markLocations = value;
			}
		}

		public override void Text(ITextProperties textInfo)
		{
			if (_markLocations.Count > 0)
			{
				NativeTextLocation currentLocation = GetCurrentLocation();
				Pair<int, int> pair = ParseText(textInfo.Text);
				int offset = base.Offset + pair.Second;
				if (pair.First > 0)
				{
					offset = pair.Second;
				}
				NativeTextLocation other = new NativeTextLocation(base.Line + pair.First, offset);
				List<Pair<int, int>> list = new List<Pair<int, int>>();
				foreach (Pair<LocationMarkerId, NativeTextLocation> markLocation in _markLocations)
				{
					if (markLocation.Second.IsAfter(currentLocation))
					{
						if (!markLocation.Second.IsBefore(other))
						{
							break;
						}
						if (list.Count <= 0 || !list[list.Count - 1].Equals(markLocation.Second))
						{
							Pair<int, int> pair2 = new Pair<int, int>(markLocation.Second.Line - currentLocation.Line, markLocation.Second.Offset - 1);
							if (markLocation.Second.Line == currentLocation.Line)
							{
								pair2.Second = markLocation.Second.Offset - currentLocation.Offset;
							}
							list.Add(pair2);
						}
					}
				}
				if (list.Count > 0)
				{
					SplitText(textInfo.Text, list);
					return;
				}
			}
			base.Text(textInfo);
		}

		private void SplitText(string text, List<Pair<int, int>> splitLocations)
		{
			List<int> list = new List<int>();
			int num = 0;
			int num2 = 0;
			bool flag = base.LastCharacterWasCR;
			for (int i = 0; i < text.Length; i++)
			{
				Pair<int, int> pair = splitLocations[0];
				if (pair.First == num && pair.Second == num2 && (!flag || text[i] != '\n'))
				{
					list.Add(i);
					splitLocations.RemoveAt(0);
					if (splitLocations.Count == 0)
					{
						break;
					}
				}
				switch (text[i])
				{
				case '\r':
					num++;
					num2 = 0;
					flag = true;
					break;
				case '\n':
					if (!flag)
					{
						num++;
						num2 = 0;
					}
					flag = false;
					break;
				default:
					num2++;
					flag = false;
					break;
				}
			}
			int num3 = 0;
			foreach (int item in list)
			{
				ITextProperties textInfo = PropertiesFactory.CreateTextProperties(text.Substring(num3, item - num3));
				base.Text(textInfo);
				num3 = item;
			}
			ITextProperties textInfo2 = PropertiesFactory.CreateTextProperties(text.Substring(num3));
			base.Text(textInfo2);
		}

		protected override void IncrementLineAndOffsetNumbers(string text)
		{
			if (_markLocations.Count > 0)
			{
				Pair<LocationMarkerId, NativeTextLocation> pair = _markLocations[0];
				NativeTextLocation currentLocation = GetCurrentLocation();
				while (!pair.Second.IsAfter(currentLocation))
				{
					LocationMark(pair.First);
					_markLocations.RemoveAt(0);
					if (_markLocations.Count == 0)
					{
						break;
					}
					pair = _markLocations[0];
				}
			}
			base.IncrementLineAndOffsetNumbers(text);
		}

		public void SetFileProperties(IFileProperties properties)
		{
		}

		public void StartOfInput()
		{
			_markLocations.Sort((Pair<LocationMarkerId, NativeTextLocation> first, Pair<LocationMarkerId, NativeTextLocation> second) => first.Second.CompareTo(second.Second));
		}

		public void EndOfInput()
		{
			foreach (Pair<LocationMarkerId, NativeTextLocation> markLocation in _markLocations)
			{
				LocationMark(markLocation.First);
			}
			_markLocations.Clear();
		}
	}
}
