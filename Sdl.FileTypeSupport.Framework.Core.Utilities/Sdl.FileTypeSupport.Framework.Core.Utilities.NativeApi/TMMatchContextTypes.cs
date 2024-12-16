namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public static class TMMatchContextTypes
	{
		private static string AllContextTypes = "x-tm-table-heading,x-tm-table-cell,x-tm-heading,x-tm-header-footer,x-tm-listitem,x-tm-tag,x-tm-label,x-tm-index-entry,x-tm-length-info";

		public const string TableHeading = "x-tm-table-heading";

		public const string TableCell = "x-tm-table-cell";

		public const string Heading = "x-tm-heading";

		public const string PageHeaderFooter = "x-tm-header-footer";

		public const string ListItem = "x-tm-listitem";

		public const string TagContent = "x-tm-tag";

		public const string Label = "x-tm-label";

		public const string IndexEntry = "x-tm-index-entry";

		public const string LengthInformation = "x-tm-length-info";

		public static bool IsDefined(string contextType)
		{
			return AllContextTypes.Contains(contextType);
		}
	}
}
