using System;
using System.ComponentModel;

namespace Sdl.Desktop.IntegrationApi.Extensions.Internal
{
	public abstract class AbstractLocation
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract string Path
		{
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract string InsertBefore
		{
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract string InsertAfter
		{
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract Type TargetAttributeType
		{
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint ZIndex
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsSeparator
		{
			get;
			set;
		}
	}
}
