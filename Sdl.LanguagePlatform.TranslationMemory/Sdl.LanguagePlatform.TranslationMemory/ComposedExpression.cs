using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class ComposedExpression : FilterExpression
	{
		public enum Operator
		{
			Not = 1,
			And,
			Or
		}

		[DataMember]
		public FilterExpression LeftOperand
		{
			get;
			set;
		}

		[DataMember]
		public FilterExpression RightOperand
		{
			get;
			set;
		}

		[DataMember]
		public Operator Op
		{
			get;
			set;
		}

		public ComposedExpression(FilterExpression leftOperand, Operator op, FilterExpression rightOperand)
		{
			_ = 1;
			LeftOperand = leftOperand;
			Op = op;
			RightOperand = rightOperand;
		}

		public override string ToString()
		{
			switch (Op)
			{
			case Operator.Not:
				return "( !" + RightOperand?.ToString() + ")";
			case Operator.And:
				return "(" + LeftOperand?.ToString() + " & " + RightOperand?.ToString() + ")";
			case Operator.Or:
				return "(" + LeftOperand?.ToString() + " | " + RightOperand?.ToString() + ")";
			default:
				return string.Empty;
			}
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			if (Op != Operator.Not)
			{
				if (LeftOperand.Validate(fields, throwIfInvalid))
				{
					return RightOperand.Validate(fields, throwIfInvalid);
				}
				return false;
			}
			if (LeftOperand == null)
			{
				return RightOperand.Validate(fields, throwIfInvalid);
			}
			if (throwIfInvalid)
			{
				throw new ArgumentException("Operand is NOT, but left expression is not null");
			}
			return false;
		}

		public override bool Evaluate(ITypedKeyValueContainer values)
		{
			switch (Op)
			{
			case Operator.Not:
				return !RightOperand.Evaluate(values);
			case Operator.And:
				if (LeftOperand.Evaluate(values))
				{
					return RightOperand.Evaluate(values);
				}
				return false;
			case Operator.Or:
				if (!LeftOperand.Evaluate(values))
				{
					return RightOperand.Evaluate(values);
				}
				return true;
			default:
				throw new ArgumentException("Invalid operator.");
			}
		}
	}
}
