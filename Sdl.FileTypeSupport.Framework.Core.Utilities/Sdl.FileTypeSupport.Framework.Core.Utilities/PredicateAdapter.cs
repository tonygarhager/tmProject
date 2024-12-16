using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities
{
	public class PredicateAdapter<AdapteeType, TargetType> where AdapteeType : class
	{
		private Predicate<AdapteeType> _Adaptee;

		public Predicate<AdapteeType> Adaptee
		{
			get
			{
				return _Adaptee;
			}
			set
			{
				_Adaptee = value;
			}
		}

		public Predicate<TargetType> Target => Evaluate;

		public PredicateAdapter(Predicate<AdapteeType> adaptee)
		{
			_Adaptee = adaptee;
		}

		public bool Evaluate(TargetType toEvaluate)
		{
			AdapteeType val = toEvaluate as AdapteeType;
			if (val == null || _Adaptee == null)
			{
				return false;
			}
			return _Adaptee(val);
		}
	}
}
