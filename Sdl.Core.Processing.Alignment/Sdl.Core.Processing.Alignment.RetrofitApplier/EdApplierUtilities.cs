using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.IO;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class EdApplierUtilities
	{
		public static IAbstractMarkupDataContainer CreateTagPairFromFakeToken(EdApplierContext context, BilingualTagPairToken tagPairToken)
		{
			if (context.Map.UpdateTag.Count > 0 && tagPairToken != null)
			{
				_ = tagPairToken.Anchor;
				if (context.Map.UpdateTag.ContainsKey(tagPairToken.Anchor))
				{
					TagPair tagPair = context.Map.UpdateTag[tagPairToken.Anchor].Clone() as TagPair;
					while (tagPair != null && tagPair.Count > 0)
					{
						tagPair[0].RemoveFromParent();
					}
					return tagPair;
				}
			}
			if (context.Map.OriginalTag.Count > 0 && tagPairToken != null)
			{
				_ = tagPairToken.Anchor;
				if (context.Map.OriginalTag.ContainsKey(tagPairToken.Anchor))
				{
					TagPair tagPair2 = context.Map.OriginalTag[tagPairToken.Anchor] as TagPair;
					while (tagPair2 != null && tagPair2.Count > 0)
					{
						tagPair2[0].RemoveFromParent();
					}
					return tagPair2;
				}
			}
			if (context.Map.UpdateComment.Count > 0 && tagPairToken != null)
			{
				_ = tagPairToken.Anchor;
				if (context.Map.UpdateComment.ContainsKey(tagPairToken.Anchor))
				{
					CommentMarker commentMarker = context.Map.UpdateTag[tagPairToken.Anchor].Clone() as CommentMarker;
					while (commentMarker != null && commentMarker.Count > 0)
					{
						commentMarker[0].RemoveFromParent();
					}
					return commentMarker;
				}
			}
			if (context.Map.OriginalComment.Count > 0 && tagPairToken != null)
			{
				_ = tagPairToken.Anchor;
				if (context.Map.OriginalComment.ContainsKey(tagPairToken.Anchor))
				{
					CommentMarker commentMarker2 = context.Map.OriginalComment[tagPairToken.Anchor] as CommentMarker;
					while (commentMarker2 != null && commentMarker2.Count > 0)
					{
						commentMarker2[0].RemoveFromParent();
					}
					return commentMarker2;
				}
			}
			throw new Exception("CreateTagPairFromFakeToken - tagPair must be either part of source or Target");
		}

		public static bool IsTokenTagPair(Token linguaToken)
		{
			TagToken tagToken = linguaToken as TagToken;
			if (tagToken != null)
			{
				if (tagToken.Tag.Type != TagType.Start)
				{
					return tagToken.Tag.Type == TagType.End;
				}
				return true;
			}
			return false;
		}

		public static bool IsTokenPlaceholder(Token linguaToken, out TagToken tagToken)
		{
			tagToken = (linguaToken as TagToken);
			if (tagToken != null)
			{
				return tagToken.Tag.Type == TagType.Standalone;
			}
			return false;
		}

		public static IAbstractMarkupData CreateBilingualToken(EdApplierContext context, Token linguaToken)
		{
			TagToken tagToken = linguaToken as TagToken;
			if (tagToken != null)
			{
				if (tagToken.Tag.Type == TagType.Standalone)
				{
					context.Map.UpdatePh[tagToken.Tag.Anchor].RemoveFromParent();
					return context.Map.UpdatePh[tagToken.Tag.Anchor];
				}
				return CreateFakeTokenFromLinguaTagToken(context, tagToken);
			}
			return context.ItemFactory.CreateText(context.ItemFactory.PropertiesFactory.CreateTextProperties(linguaToken.Text));
		}

		public static IAbstractMarkupData CreateUpdateBcmToken(EdApplierContext context, Token linguaToken)
		{
			TagToken tagToken = linguaToken as TagToken;
			if (tagToken != null)
			{
				if (tagToken.Tag.Type == TagType.Standalone)
				{
					context.Map.UpdatePh[tagToken.Tag.Anchor].RemoveFromParent();
					return context.Map.UpdatePh[tagToken.Tag.Anchor];
				}
				return CreateFakeTokenFromLinguaTagToken(context, tagToken);
			}
			return context.ItemFactory.CreateText(context.ItemFactory.PropertiesFactory.CreateTextProperties(linguaToken.Text));
		}

		private static BilingualTagPairToken CreateFakeTokenFromLinguaTagToken(EdApplierContext context, TagToken tagPairToken)
		{
			return new BilingualTagPairToken(tagPairToken, context.Map.UpdateTag[tagPairToken.Tag.Anchor]);
		}

		public static void LogSegment(bool enableLogs, string logAction, ISegment updated, ISegment applied, ISegment original, string dirLog, string segmentFileLogName, string error = "")
		{
			if (enableLogs)
			{
				if (!Directory.Exists(dirLog))
				{
					Directory.CreateDirectory(dirLog);
				}
				File.AppendAllText(dirLog + "\\" + segmentFileLogName, logAction + Environment.NewLine);
				BilingualSerializationFlags bilingualSerializationFlags = new BilingualSerializationFlags();
				bilingualSerializationFlags.TagID = false;
				BilingualSerializerVisitor bilingualSerializerVisitor = new BilingualSerializerVisitor(bilingualSerializationFlags);
				original.AcceptVisitor(bilingualSerializerVisitor);
				string text = bilingualSerializerVisitor.ContentString.ToString();
				bilingualSerializerVisitor.ContentString.Clear();
				updated?.AcceptVisitor(bilingualSerializerVisitor);
				string text2 = bilingualSerializerVisitor.ContentString.ToString();
				bilingualSerializerVisitor.ContentString.Clear();
				applied?.AcceptVisitor(bilingualSerializerVisitor);
				string text3 = bilingualSerializerVisitor.ContentString.ToString();
				string str = "[original] => " + text + Environment.NewLine + "[updated ] => " + text2 + Environment.NewLine + "[applied ] => " + text3 + Environment.NewLine;
				if (error != string.Empty)
				{
					str = str + " [Error ] => " + error + Environment.NewLine;
				}
				File.AppendAllText(dirLog + "\\" + segmentFileLogName, str + Environment.NewLine);
			}
		}

		public static void MoveAllcontainerContent(RevisionMarker source, RevisionMarker target)
		{
			while (source.Count > 0)
			{
				IAbstractMarkupData abstractMarkupData = source[0];
				abstractMarkupData.RemoveFromParent();
				target.Add(abstractMarkupData);
			}
		}
	}
}
