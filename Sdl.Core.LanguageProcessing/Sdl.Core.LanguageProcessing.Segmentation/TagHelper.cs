using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public static class TagHelper
	{
		public static ITagPair TagPairAtLocation(this Location location)
		{
			return location?.ItemAtLocation.AsTagPair();
		}

		public static ITagPair AsTagPair(this IAbstractMarkupData markup)
		{
			return markup as ITagPair;
		}

		public static IRevisionMarker RevisionMarkerAtLocation(this Location location)
		{
			return location?.ItemAtLocation.AsRevisionMarker();
		}

		public static IRevisionMarker AsRevisionMarker(this IAbstractMarkupData markup)
		{
			return markup as IRevisionMarker;
		}

		public static ICommentMarker AsCommentMarker(this IAbstractMarkupData markup)
		{
			return markup as ICommentMarker;
		}

		public static ILockedContent LockedContentAtLocation(this Location location)
		{
			if (location == null)
			{
				return null;
			}
			return location.ItemAtLocation.AsLockedContent();
		}

		private static ILockedContent AsLockedContent(this IAbstractMarkupData markup)
		{
			return markup as ILockedContent;
		}
	}
}
