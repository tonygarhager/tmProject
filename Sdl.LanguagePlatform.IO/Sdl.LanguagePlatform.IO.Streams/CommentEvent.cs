namespace Sdl.LanguagePlatform.IO.Streams
{
	public class CommentEvent : Event
	{
		private string _Message;

		public string Message
		{
			get
			{
				return _Message;
			}
			set
			{
				_Message = value;
			}
		}

		public CommentEvent(string msg)
		{
			_Message = msg;
		}
	}
}
