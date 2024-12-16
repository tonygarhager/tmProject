namespace Sdl.Core.PluginFramework.Implementation
{
	internal interface IListFilter<T>
	{
		bool ShouldInclude(T item);
	}
}
