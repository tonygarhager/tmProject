using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public static class AttributeValues
	{
		public const string SegMtype = "seg";

		public const string LocationMarkerMtype = "x-sdl-location";

		public const string CommentMarkerMtype = "x-sdl-comment";

		public const string AddedRevisionMtype = "x-sdl-added";

		public const string DeletedRevisionMtype = "x-sdl-deleted";

		public const string FeedbackRevisionCommentMtype = "x-sdl-feedback-comment";

		public const string FeedbackRevisionAddedMtype = "x-sdl-feedback-added";

		public const string FeedbackRevisionDeletedMtype = "x-sdl-feedback-deleted";

		public const string True = "true";

		public const string False = "false";

		public const string PurposeInformation = "information";

		public const string PurposeMatch = "match";

		public static string Structure => LockTypeFlags.Structure.ToString();

		public static string Externalized => LockTypeFlags.Externalized.ToString();

		public static string Manual => LockTypeFlags.Manual.ToString();
	}
}
