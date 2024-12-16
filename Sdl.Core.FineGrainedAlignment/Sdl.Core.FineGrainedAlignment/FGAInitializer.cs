using System;
using System.Reflection;

namespace Sdl.Core.FineGrainedAlignment
{
	public static class FGAInitializer
	{
		private static bool _builtInTypesInitialized;

		private static readonly object _locker = new object();

		public static void Initialize(Assembly a)
		{
			lock (_locker)
			{
				if (!(a == null))
				{
					goto IL_0032;
				}
				if (!_builtInTypesInitialized)
				{
					a = Assembly.GetExecutingAssembly();
					_builtInTypesInitialized = true;
					goto IL_0032;
				}
				goto end_IL_0008;
				IL_0032:
				Type[] types = a.GetTypes();
				Type[] array = types;
				foreach (Type type in array)
				{
					if (type.IsSubclassOf(typeof(TranslationModelId)))
					{
						CallRegister(type);
					}
					if (type.IsSubclassOf(typeof(AlignableCorpusId)))
					{
						CallRegister(type);
					}
					if (type.IsSubclassOf(typeof(AlignerDefinition)))
					{
						CallRegister(type);
					}
				}
				end_IL_0008:;
			}
		}

		private static void CallRegister(Type t)
		{
			MethodInfo method = t.GetMethod("Register", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}
	}
}
