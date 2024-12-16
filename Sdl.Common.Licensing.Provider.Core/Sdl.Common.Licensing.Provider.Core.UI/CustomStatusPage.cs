namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class CustomStatusPage
	{
		public ShowCustomStatusPageDelegate ShowPageWhen
		{
			get;
			set;
		}

		public IUIControl StatusPage
		{
			get;
			set;
		}
	}
}
