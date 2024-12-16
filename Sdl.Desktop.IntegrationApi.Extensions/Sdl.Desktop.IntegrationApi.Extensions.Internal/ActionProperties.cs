using System;

namespace Sdl.Desktop.IntegrationApi.Extensions.Internal
{
	[Flags]
	public enum ActionProperties
	{
		None = 0x0,
		Text = 0x1,
		ToolTipText = 0x2,
		Icon = 0x4,
		Shortcuts = 0x8,
		Enabled = 0x10,
		Available = 0x20,
		Style = 0x40,
		Checked = 0x80,
		ActionGroup = 0x100,
		ShowText = 0x200,
		PopupControl = 0x400,
		Size = 0x800,
		All = 0xFFF
	}
}
