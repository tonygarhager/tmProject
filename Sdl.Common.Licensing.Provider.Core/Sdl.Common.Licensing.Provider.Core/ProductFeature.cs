namespace Sdl.Common.Licensing.Provider.Core
{
	public class ProductFeature
	{
		public int Id
		{
			get;
		}

		public string Name
		{
			get;
		}

		public string Version
		{
			get;
		}

		public GetFeatureValueDelegate GetFeatureValue
		{
			get;
		}

		public ProductFeature(int id, string name)
			: this(id, name, null, null)
		{
		}

		public ProductFeature(int id, string name, string version)
			: this(id, name, version, null)
		{
		}

		public ProductFeature(int id, string name, GetFeatureValueDelegate getFeatureValueDelegate)
			: this(id, name, null, getFeatureValueDelegate)
		{
		}

		public ProductFeature(int id, string name, string version, GetFeatureValueDelegate getFeatureValueDelegate)
		{
			Id = id;
			Name = name;
			Version = version;
			GetFeatureValue = getFeatureValueDelegate;
		}
	}
}
