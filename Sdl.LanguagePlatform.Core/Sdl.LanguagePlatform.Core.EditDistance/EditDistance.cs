using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.EditDistance
{
	[DataContract]
	public class EditDistance
	{
		public EditDistanceItem this[int index] => Items[index];

		public double Score
		{
			get
			{
				double num = SourceObjectCount + TargetObjectCount;
				num -= (double)Items.Count((EditDistanceItem item) => item.Operation == EditOperation.Insert && item.Resolution == EditDistanceResolution.Deletion);
				if (num <= 0.0)
				{
					return 0.0;
				}
				double num2 = (num - 2.0 * Distance) / num;
				if (num2 <= 0.0)
				{
					return 0.0;
				}
				if (!(num2 >= 1.0))
				{
					return num2;
				}
				return 1.0;
			}
		}

		[DataMember]
		public double Distance
		{
			get;
			set;
		}

		[DataMember]
		public List<EditDistanceItem> Items
		{
			get;
			set;
		}

		[DataMember]
		public int SourceObjectCount
		{
			get;
			private set;
		}

		[DataMember]
		public int TargetObjectCount
		{
			get;
			private set;
		}

		public EditDistance()
		{
			Distance = 0.0;
			Items = new List<EditDistanceItem>();
		}

		public EditDistance(int sourceObjectCount, int targetObjectCount, double distance)
		{
			SourceObjectCount = sourceObjectCount;
			TargetObjectCount = targetObjectCount;
			Distance = distance;
			Items = new List<EditDistanceItem>();
		}

		public void SetSourceAt(int index, int offset)
		{
			EditDistanceItem value = Items[index];
			value.Source = offset;
			Items[index] = value;
		}

		public void SetTargetAt(int index, int offset)
		{
			EditDistanceItem value = Items[index];
			value.Target = offset;
			Items[index] = value;
		}

		public void SetResolutionAt(int index, EditDistanceResolution resolution)
		{
			EditDistanceItem value = Items[index];
			value.Resolution = resolution;
			Items[index] = value;
		}

		public int FindSourceItemIndex(int sourceTokenOffset)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].Source == sourceTokenOffset && Items[i].Operation != EditOperation.Insert)
				{
					return i;
				}
			}
			return -1;
		}

		public int FindTargetItemIndex(int targetTokenOffset)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].Target == targetTokenOffset && Items[i].Operation != EditOperation.Delete)
				{
					return i;
				}
			}
			return -1;
		}

		public void Sort()
		{
			Items.Sort(delegate(EditDistanceItem x, EditDistanceItem y)
			{
				int num = 0;
				if (x.Operation != EditOperation.Insert && y.Operation != EditOperation.Insert)
				{
					num = x.Source - y.Source;
				}
				if (num == 0 && x.Operation != EditOperation.Delete && y.Operation != EditOperation.Delete)
				{
					num = x.Target - y.Target;
				}
				return num;
			});
		}

		public void Add(EditDistanceItem item)
		{
			Items.Add(item);
		}

		public void AddAtStart(EditDistanceItem item)
		{
			Items.Insert(0, item);
		}

		public void Dump(TextWriter wtr, string msg)
		{
			wtr.WriteLine();
			wtr.WriteLine("Edit distance ({0})", msg);
			wtr.WriteLine("--------------------------------------------------------------------");
			wtr.WriteLine("{0} source objects, {1} target objects, {2} items in ED, distance = {3}", SourceObjectCount, TargetObjectCount, Items.Count, Distance);
			wtr.WriteLine();
			for (int i = 0; i < Items.Count; i++)
			{
				EditDistanceItem editDistanceItem = Items[i];
				wtr.Write("\t{0}:\t({1}, {2}): {3} (c={4})", i, editDistanceItem.Source, editDistanceItem.Target, editDistanceItem.Operation, editDistanceItem.Costs);
				if (editDistanceItem.Operation == EditOperation.Move)
				{
					wtr.Write("\t(mst={0}, mts={1})", editDistanceItem.MoveSourceTarget, editDistanceItem.MoveTargetSource);
				}
				if (editDistanceItem.Resolution != 0)
				{
					wtr.Write("\tres={0}", editDistanceItem.Resolution);
				}
				wtr.WriteLine();
			}
			wtr.WriteLine();
			wtr.WriteLine();
		}
	}
}
