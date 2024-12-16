using System;

namespace Sdl.Core.Processing.Alignment.Api
{
	internal class ProgressReporter<T, TP> where T : EventArgs, new()
	{
		public EventHandler<T> Handler;

		private readonly object _source;

		private readonly object[] _info;

		private int _previousProgress;

		public ProgressReporter(object source, params object[] info)
		{
			_source = source;
			_info = info;
			_previousProgress = -1;
		}

		public void Report(TP phase, int progress)
		{
			if (Handler == null)
			{
				return;
			}
			if (progress < 0)
			{
				progress = 0;
			}
			if (progress > 100)
			{
				progress = 100;
			}
			if (_previousProgress < progress)
			{
				_previousProgress = progress;
				if (_info != null && _info.Length != 0)
				{
					Handler(_source, (T)Activator.CreateInstance(typeof(T), phase, (byte)progress, _info));
				}
				else
				{
					Handler(_source, (T)Activator.CreateInstance(typeof(T), phase, (byte)progress));
				}
			}
		}
	}
}
