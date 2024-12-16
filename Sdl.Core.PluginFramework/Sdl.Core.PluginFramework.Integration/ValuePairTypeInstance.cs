using System;

namespace Sdl.Core.PluginFramework.Integration
{
	public sealed class ValuePairTypeInstance
	{
		public Type ResultType
		{
			get;
			private set;
		}

		public object Instance
		{
			get;
			private set;
		}

		internal static ValuePairTypeInstance Create(Type resultType, object instance)
		{
			return new ValuePairTypeInstance(resultType, instance);
		}

		internal static ValuePairTypeInstance Create<TResult>(object instance)
		{
			return Create(typeof(TResult), instance);
		}

		internal ValuePairTypeInstance(Type resultType, object instance)
		{
			ResultType = resultType;
			Instance = instance;
		}
	}
}
