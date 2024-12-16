using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class TranslationUnitFilteringStrategyFactory
	{
		private static readonly Regex ClientOnlyFieldsRegexDataVersion0 = new Regex("^(src)|(trg)|(sourceplainlength)|(targetplainlength)|(sourcetagcount)|(sourcetagcount)|(x\\-origin)|(x\\-format)|(x\\-confirmationlevel)|(structurecontext)|(textcontext)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex ClientOnlyFieldsRegexDataVersion1 = new Regex("^(sourcetagcount)|(sourcetagcount)|(x\\-origin)|(x\\-format)|(x\\-confirmationlevel)|(structurecontext)|(textcontext)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static bool FilterHasClientOnlyLogic(FilterExpression filters, int tmDataVersion)
		{
			AtomicExpression atomicExpression = filters as AtomicExpression;
			if (atomicExpression == null)
			{
				ComposedExpression composedExpression = filters as ComposedExpression;
				if (composedExpression != null)
				{
					if (composedExpression.LeftOperand != null && FilterHasClientOnlyLogic(composedExpression.LeftOperand, tmDataVersion))
					{
						return true;
					}
					if (composedExpression.RightOperand != null && FilterHasClientOnlyLogic(composedExpression.RightOperand, tmDataVersion))
					{
						return true;
					}
				}
				return false;
			}
			if (atomicExpression.Op == AtomicExpression.Operator.Matches || atomicExpression.Op == AtomicExpression.Operator.MatchesNot)
			{
				return true;
			}
			AtomicExpression atomicExpression2 = atomicExpression;
			if (tmDataVersion == 0)
			{
				if (ClientOnlyFieldsRegexDataVersion0.IsMatch(atomicExpression2.Value.Name))
				{
					return true;
				}
			}
			else if (ClientOnlyFieldsRegexDataVersion1.IsMatch(atomicExpression2.Value.Name))
			{
				return true;
			}
			return false;
		}

		public static ITranslationUnitFilteringStrategy GetBestStrategyForFilter(FilterExpression filters, CallContext context, int tmDataVersion)
		{
			if (context.Storage is SqlStorage)
			{
				if (!FilterHasClientOnlyLogic(filters, tmDataVersion))
				{
					return new TranslationUnitServerFilteringStrategy(context);
				}
				return new TranslationUnitClientFilteringStrategy(context, fileBased: false);
			}
			return new TranslationUnitClientFilteringStrategy(context, fileBased: true);
		}

		public static bool IsFilterRunnableOnSql(FilterExpression filters, int tmDataVersion)
		{
			if (filters == null)
			{
				return false;
			}
			return !FilterHasClientOnlyLogic(filters, tmDataVersion);
		}
	}
}
