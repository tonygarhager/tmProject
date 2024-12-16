using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class TuIdContexts
	{
		[DataMember]
		public HashSet<string> Values
		{
			get;
			set;
		}

		public int Length
		{
			get
			{
				HashSet<string> values = Values;
				if (values == null)
				{
					return 0;
				}
				return values.Count;
			}
		}

		public TuIdContexts()
		{
			Values = new HashSet<string>();
		}

		public TuIdContexts(TuIdContexts other)
		{
			Values = new HashSet<string>(other.Values);
		}

		public bool Add(string newVal)
		{
			if (newVal == null)
			{
				return false;
			}
			if (Values.Contains(newVal))
			{
				return false;
			}
			Values.Add(newVal);
			return true;
		}

		public void AddRange(TuIdContexts contexts)
		{
			AddRange(contexts.Values);
		}

		public void AddRange(IEnumerable<string> values)
		{
			if (values != null)
			{
				Values.UnionWith(values);
			}
		}

		public bool Merge(TuIdContexts contexts)
		{
			return Merge(contexts.Values);
		}

		public bool Assign(TuIdContexts contexts)
		{
			Clear();
			AddRange(contexts);
			return true;
		}

		public bool Merge(string value)
		{
			if (value == null)
			{
				return false;
			}
			if (Values.Contains(value))
			{
				return false;
			}
			Values.Add(value);
			return true;
		}

		public bool Merge(IEnumerable<string> values)
		{
			if (values == null)
			{
				return false;
			}
			bool result = false;
			foreach (string value in values)
			{
				if (!Values.Contains(value))
				{
					Values.Add(value);
					result = true;
				}
			}
			return result;
		}

		public void Clear()
		{
			Values.Clear();
		}

		public bool HasValue(string val)
		{
			return Values.Contains(val);
		}

		public bool HasValues(TuIdContexts other)
		{
			if (other == null || other.Length == 0)
			{
				return true;
			}
			return other.Values.All(HasValue);
		}

		public bool Equals(TuIdContexts other)
		{
			if (HasValues(other))
			{
				return other.HasValues(this);
			}
			return false;
		}
	}
}
