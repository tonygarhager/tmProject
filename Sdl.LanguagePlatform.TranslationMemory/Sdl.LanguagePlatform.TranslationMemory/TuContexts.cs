using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class TuContexts
	{
		[DataMember]
		public HashSet<TuContext> Values
		{
			get;
			set;
		}

		public int Length
		{
			get
			{
				HashSet<TuContext> values = Values;
				if (values == null)
				{
					return 0;
				}
				return values.Count;
			}
		}

		public TuContexts()
		{
			Values = new HashSet<TuContext>();
		}

		public TuContexts(TuContexts other)
		{
			Values = new HashSet<TuContext>(other.Values);
		}

		public bool Add(TuContext newVal)
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

		public void AddRange(TuContexts contexts)
		{
			AddRange(contexts.Values);
		}

		public void AddRange(IEnumerable<TuContext> values)
		{
			if (values != null)
			{
				Values.UnionWith(values);
			}
		}

		public bool Merge(TuContexts contexts)
		{
			return Merge(contexts.Values);
		}

		public bool Assign(TuContexts contexts)
		{
			Clear();
			AddRange(contexts);
			return true;
		}

		public bool Merge(TuContext value)
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

		public bool Merge(IEnumerable<TuContext> values)
		{
			if (values == null)
			{
				return false;
			}
			bool result = false;
			foreach (TuContext value in values)
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

		public bool HasValue(TuContext val)
		{
			return Values.Contains(val);
		}

		public bool HasValues(TuContexts other)
		{
			if (other == null || other.Length == 0)
			{
				return true;
			}
			return other.Values.All(HasValue);
		}

		public bool Equals(TuContexts other)
		{
			if (HasValues(other))
			{
				return other.HasValues(this);
			}
			return false;
		}
	}
}
