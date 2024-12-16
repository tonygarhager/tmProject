using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua
{
	internal class SegmentTokenVisitor : ISegmentElementVisitor
	{
		private readonly Sdl.Core.Bcm.BcmModel.Segment _container;

		public SegmentTokenVisitor(Sdl.Core.Bcm.BcmModel.Segment container)
		{
			_container = container;
			_container.Tokens = new List<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token>();
		}

		public void VisitTagToken(Sdl.LanguagePlatform.Core.Tokenization.TagToken linguaToken)
		{
			TagBcmTokenConverter tagBcmTokenConverter = new TagBcmTokenConverter();
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token item = tagBcmTokenConverter.ToBcmTokenSpecific(linguaToken);
			_container.Tokens.Add(item);
		}

		public void VisitDateTimeToken(Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken lingua)
		{
			DateTimeBcmTokenConverter dateTimeBcmTokenConverter = new DateTimeBcmTokenConverter();
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token item = dateTimeBcmTokenConverter.ToBcmTokenSpecific(lingua);
			_container.Tokens.Add(item);
		}

		public void VisitNumberToken(Sdl.LanguagePlatform.Core.Tokenization.NumberToken lingua)
		{
			NumberBcmTokenConverter numberBcmTokenConverter = new NumberBcmTokenConverter();
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token item = numberBcmTokenConverter.ToBcmTokenSpecific(lingua);
			_container.Tokens.Add(item);
		}

		public void VisitMeasureToken(Sdl.LanguagePlatform.Core.Tokenization.MeasureToken lingua)
		{
			MeasureBcmTokenConverter measureBcmTokenConverter = new MeasureBcmTokenConverter();
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token item = measureBcmTokenConverter.ToBcmTokenSpecific(lingua);
			_container.Tokens.Add(item);
		}

		public void VisitSimpleToken(SimpleToken lingua)
		{
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token item;
			if (lingua.Type == TokenType.Word)
			{
				WordBcmTokenConverter wordBcmTokenConverter = new WordBcmTokenConverter();
				item = wordBcmTokenConverter.ToBcmTokenSpecific(lingua);
			}
			else
			{
				SimpleBcmTokenConverter simpleBcmTokenConverter = new SimpleBcmTokenConverter(lingua.Type);
				item = simpleBcmTokenConverter.ToBcmTokenSpecific(lingua);
			}
			_container.Tokens.Add(item);
		}

		public void VisitTag(Tag tag)
		{
			throw new NotSupportedException();
		}

		public void VisitText(Text text)
		{
			throw new NotSupportedException();
		}
	}
}
