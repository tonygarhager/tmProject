namespace Sdl.FileTypeSupport.Framework.Native
{
	public static class KeyNames
	{
		public const string Prefix = "SDL:";

		public static string Prefixed(string root)
		{
			return "SDL:" + root;
		}
	}
}
