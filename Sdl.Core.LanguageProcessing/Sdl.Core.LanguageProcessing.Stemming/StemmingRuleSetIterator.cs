using System;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	internal class StemmingRuleSetIterator
	{
		private readonly StemmingRuleSet _set;

		private int _position;

		public StemmingRule Current
		{
			get
			{
				if (_position < 0 || _position >= _set.Count)
				{
					return null;
				}
				return _set[_position];
			}
		}

		public StemmingRuleSetIterator(StemmingRuleSet set)
		{
			_set = (set ?? throw new ArgumentNullException("set"));
			_position = -1;
		}

		public void First(int priority)
		{
			_position = -1;
			Next(priority);
		}

		public void Next(int priority)
		{
			if (_position >= _set.Count)
			{
				_position = -1;
				return;
			}
			_position++;
			if (priority > 0)
			{
				while (_position < _set.Count && _set[_position].Priority < priority)
				{
					_position++;
				}
			}
		}
	}
}
