using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	[KnownType(typeof(ComposedExpression))]
	[KnownType(typeof(AtomicExpression))]
	public abstract class FilterExpression
	{
		public abstract bool Evaluate(ITypedKeyValueContainer values);

		public abstract bool Validate(IFieldDefinitions fields, bool throwIfInvalid);
	}
}
