namespace Sdl.Core.PluginFramework
{
	public class ObjectRegistry<TExtensionAttribute, TExtensionType> where TExtensionAttribute : ExtensionAttribute where TExtensionType : class
	{
		private readonly object _lockObject = new object();

		private readonly IExtensionPoint _extensionPoint;

		public IExtensionPoint ExtensionPoint => _extensionPoint;

		public ObjectRegistry(IPluginRegistry pluginRegistry)
		{
			_extensionPoint = pluginRegistry.GetExtensionPoint<TExtensionAttribute>();
		}

		public virtual TExtensionType[] CreateObjects()
		{
			lock (_lockObject)
			{
				TExtensionType[] array = new TExtensionType[_extensionPoint.Extensions.Count];
				for (int i = 0; i < _extensionPoint.Extensions.Count; i++)
				{
					array[i] = (_extensionPoint.Extensions[i].CreateInstance() as TExtensionType);
				}
				return array;
			}
		}
	}
}
