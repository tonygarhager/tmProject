namespace Sdl.Enterprise2.Platform.Contracts.Communication.Callback
{
	public delegate void CallbackEventHandler<T>(object sender, T eventArgs) where T : CallbackEventArgs;
}
