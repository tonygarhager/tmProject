namespace Sdl.Core.PluginFramework.Util
{
	public interface ITopologicalSortable
	{
		string Id
		{
			get;
		}

		string InsertBefore
		{
			get;
		}

		string InsertAfter
		{
			get;
		}

		int Priority
		{
			get;
		}
	}
}
