namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicenseFeature
	{
		string Id
		{
			get;
		}

		string Name
		{
			get;
		}

		string Value
		{
			get;
		}
	}
}
