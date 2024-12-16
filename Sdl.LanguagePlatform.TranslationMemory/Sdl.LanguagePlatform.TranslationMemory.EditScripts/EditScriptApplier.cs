using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	public class EditScriptApplier
	{
		public static bool Apply(EditScript script, TranslationUnit tu)
		{
			if (script == null)
			{
				throw new ArgumentNullException("script");
			}
			if (tu == null)
			{
				throw new ArgumentNullException("tu");
			}
			bool flag = false;
			if (script.Filter != null && !script.Filter.Evaluate(tu))
			{
				return false;
			}
			bool flag2 = false;
			foreach (EditAction action in script.Actions)
			{
				flag |= action.Apply(tu);
				if (action is EditActionDeleteTags || action is EditActionSearchReplace)
				{
					flag2 = true;
				}
				if (flag && script.Continuation == Continuation.StopAfterFirst)
				{
					break;
				}
			}
			if ((flag && tu.Contexts != null) & flag2)
			{
				tu.Contexts.Clear();
			}
			return flag;
		}

		public static List<bool> Apply(EditScript script, IList<TranslationUnit> tus)
		{
			return Apply(script, tus, null);
		}

		public static List<bool> Apply(EditScript script, IList<TranslationUnit> tus, FilterExpression filter)
		{
			if (script == null)
			{
				throw new ArgumentNullException("script");
			}
			if (tus == null)
			{
				throw new ArgumentNullException("tus");
			}
			List<bool> list = new List<bool>();
			foreach (TranslationUnit tu in tus)
			{
				bool item = false;
				if (tu != null && (filter == null || filter.Evaluate(tu)))
				{
					item = Apply(script, tu);
				}
				list.Add(item);
			}
			return list;
		}
	}
}
