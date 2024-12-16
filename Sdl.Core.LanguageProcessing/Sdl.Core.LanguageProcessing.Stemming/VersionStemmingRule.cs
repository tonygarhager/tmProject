namespace Sdl.Core.LanguageProcessing.Stemming
{
	internal class VersionStemmingRule : StemmingRule
	{
		internal int Version
		{
			get;
		}

		internal VersionStemmingRule(int version)
		{
			base.Action = StemAction.Version;
			Version = version;
		}
	}
}
