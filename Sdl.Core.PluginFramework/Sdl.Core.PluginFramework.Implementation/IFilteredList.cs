using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal interface IFilteredList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IDisposable
	{
		void Refresh();
	}
}
