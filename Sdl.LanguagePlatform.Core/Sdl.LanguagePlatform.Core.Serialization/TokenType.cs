namespace Sdl.LanguagePlatform.Core.Serialization
{
	internal enum TokenType
	{
		EndCommentMarker,
		EndLockedContent,
		EndRevisionMarker,
		EndTagPairContent,
		PlaceholderTag,
		StartCommentMarker,
		StartLockedContent,
		StartRevisionMarkerInclude,
		StartRevisionMarkerDelete,
		StartTagPairContent,
		Text
	}
}
