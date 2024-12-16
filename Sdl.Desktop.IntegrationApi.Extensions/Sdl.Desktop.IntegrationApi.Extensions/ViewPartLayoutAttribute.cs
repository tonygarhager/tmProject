using System;
using System.ComponentModel;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ViewPartLayoutAttribute : AbstractLayoutAttribute
	{
		public Type ConfigurationType
		{
			get;
			set;
		}

		public DockType Dock
		{
			get;
			set;
		}

		public GroupStyle GroupStyle
		{
			get;
			set;
		}

		public int Width
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		public bool Visible
		{
			get;
			set;
		}

		public bool Pinned
		{
			get;
			set;
		}

		public int MinWidth
		{
			get;
			set;
		}

		public int MinHeight
		{
			get;
			set;
		}

		public bool CreateNewGroup
		{
			get;
			set;
		}

		public GroupStyle ParentGroupStyle
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public ViewPartLayoutAttribute()
		{
			Width = 100;
			Height = 100;
			MinWidth = 200;
			MinHeight = 150;
			Pinned = true;
			Visible = true;
			GroupStyle = GroupStyle.Tab;
			Dock = DockType.Bottom;
		}

		public ViewPartLayoutAttribute(Type locationByType)
			: this()
		{
			base.LocationByType = locationByType;
		}
	}
}
