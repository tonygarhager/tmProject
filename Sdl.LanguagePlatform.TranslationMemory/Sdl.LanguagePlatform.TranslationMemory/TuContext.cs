using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class TuContext
	{
		[DataMember]
		public long Context1
		{
			get;
			set;
		}

		[DataMember]
		public long Context2
		{
			get;
			set;
		}

		[DataMember]
		public Segment Segment1
		{
			get;
			set;
		}

		[DataMember]
		public Segment Segment2
		{
			get;
			set;
		}

		[DataMember]
		[Obsolete]
		public long LeftSource
		{
			get
			{
				return Context1;
			}
			set
			{
				Context1 = value;
			}
		}

		[DataMember]
		[Obsolete]
		public long LeftTarget
		{
			get
			{
				return Context2;
			}
			set
			{
				Context2 = value;
			}
		}

		public TuContext()
		{
		}

		public TuContext(long context1, long context2)
		{
			Context1 = context1;
			Context2 = context2;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			TuContext tuContext = obj as TuContext;
			if (tuContext == null)
			{
				return false;
			}
			if (Context1 != tuContext.Context1 || Context2 != tuContext.Context2)
			{
				return false;
			}
			if (SegmentsMatch(Segment1, tuContext.Segment1))
			{
				return SegmentsMatch(Segment2, tuContext.Segment2);
			}
			return false;
		}

		private static bool SegmentsMatch(Segment s1, Segment s2)
		{
			if (s1 == null && s2 == null)
			{
				return true;
			}
			if (s1 != null && s2 != null)
			{
				return s1.Equals(s2);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Context1.GetHashCode() + Context2.GetHashCode();
		}
	}
}
