namespace Sdl.LanguagePlatform.IO.Streams
{
	public interface IEventReceiver
	{
		void Emit(Event e);
	}
}
