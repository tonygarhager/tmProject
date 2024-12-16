using System;

namespace Sdl.Core.PluginFramework
{
	public sealed class PluginId
	{
		public string Id
		{
			get;
		}

		public PluginId(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("id");
			}
			Id = id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return ((PluginId)obj).Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
