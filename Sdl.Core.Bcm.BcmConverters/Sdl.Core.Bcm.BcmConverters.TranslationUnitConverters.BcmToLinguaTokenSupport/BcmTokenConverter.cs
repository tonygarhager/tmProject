using Sdl.Core.Bcm.BcmModel.Tokenization;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public abstract class BcmTokenConverter<TBcmType, TLinguaType> : IBcmToLinguaConverter where TBcmType : Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token where TLinguaType : Sdl.LanguagePlatform.Core.Tokenization.Token
	{
		public virtual Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaToken(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token bcmToken)
		{
			return ToLinguaTokenSpecific((TBcmType)bcmToken);
		}

		public virtual Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmToken(Sdl.LanguagePlatform.Core.Tokenization.Token linguaToken)
		{
			return ToBcmTokenSpecific((TLinguaType)linguaToken);
		}

		public abstract Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(TBcmType bcmToken);

		public abstract Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(TLinguaType linguaToken);

		protected static Sdl.LanguagePlatform.Core.SegmentRange BcmToLinguaSegmentRange(Sdl.Core.Bcm.BcmModel.Tokenization.SegmentRange span)
		{
			Sdl.LanguagePlatform.Core.SegmentPosition into = new Sdl.LanguagePlatform.Core.SegmentPosition(span.Into.RunIndex, span.Into.PositionInRun);
			Sdl.LanguagePlatform.Core.SegmentPosition from = new Sdl.LanguagePlatform.Core.SegmentPosition(span.From.RunIndex, span.From.PositionInRun);
			return new Sdl.LanguagePlatform.Core.SegmentRange(from, into);
		}

		public static Sdl.Core.Bcm.BcmModel.Tokenization.SegmentRange LinguatoBcmSegmentRange(Sdl.LanguagePlatform.Core.SegmentRange lingua)
		{
			Sdl.Core.Bcm.BcmModel.Tokenization.SegmentPosition from = new Sdl.Core.Bcm.BcmModel.Tokenization.SegmentPosition(lingua.From.Index, lingua.From.Position);
			Sdl.Core.Bcm.BcmModel.Tokenization.SegmentPosition into = new Sdl.Core.Bcm.BcmModel.Tokenization.SegmentPosition(lingua.Into.Index, lingua.Into.Position);
			return new Sdl.Core.Bcm.BcmModel.Tokenization.SegmentRange(from, into);
		}

		protected static Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken LinguatoBcmNumber(Sdl.LanguagePlatform.Core.Tokenization.NumberToken lingua)
		{
			return new Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken(lingua.Text, (Sdl.Core.Bcm.BcmModel.Tokenization.NumericSeparator)lingua.GroupSeparator, (Sdl.Core.Bcm.BcmModel.Tokenization.NumericSeparator)lingua.DecimalSeparator, lingua.AlternateGroupSeparator, lingua.AlternateDecimalSeparator, (Sdl.Core.Bcm.BcmModel.Tokenization.Sign)lingua.Sign, lingua.RawSign, lingua.RawDecimalDigits, lingua.RawFractionalDigits)
			{
				Span = LinguatoBcmSegmentRange(lingua.Span)
			};
		}

		protected static Sdl.LanguagePlatform.Core.Tokenization.NumberToken GetLinguaNumber(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken bcm)
		{
			return new Sdl.LanguagePlatform.Core.Tokenization.NumberToken(bcm.Text, (Sdl.LanguagePlatform.Core.Tokenization.NumericSeparator)bcm.GroupSeparator, (Sdl.LanguagePlatform.Core.Tokenization.NumericSeparator)bcm.DecimalSeparator, bcm.AlternateGroupSeparator, bcm.AlternateDecimalSeparator, (Sdl.LanguagePlatform.Core.Tokenization.Sign)bcm.Sign, bcm.RawSign, bcm.RawDecimalDigits, bcm.RawFractionalDigits)
			{
				Span = BcmToLinguaSegmentRange(bcm.Span)
			};
		}
	}
}
