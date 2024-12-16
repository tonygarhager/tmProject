using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	public class StemmingRuleSetReader : IDisposable
	{
		private TextReader _reader;

		private bool _ownStream;

		public StemmingRuleSetReader(TextReader reader)
		{
			_reader = reader;
			_ownStream = false;
		}

		public StemmingRuleSetReader(Stream s)
		{
			_reader = new StreamReader(s);
			_ownStream = true;
		}

		public StemmingRuleSetReader(string path)
		{
			_reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			_ownStream = false;
		}

		public StemmingRuleSet Read(CultureInfo culture)
		{
			StemmingRuleSet stemmingRuleSet = new StemmingRuleSet(culture);
			StemmingRuleParser stemmingRuleParser = new StemmingRuleParser(stemmingRuleSet);
			string text;
			while ((text = _reader.ReadLine()) != null)
			{
				text = text.Trim();
				if (!text.StartsWith("#") && text.Length != 0)
				{
					stemmingRuleParser.Add(text);
				}
			}
			Close();
			return stemmingRuleSet;
		}

		private void Close()
		{
			if (_ownStream && _reader != null)
			{
				_reader.Close();
				_reader.Dispose();
			}
			_reader = null;
			_ownStream = false;
		}

		public void Dispose()
		{
			Close();
		}

		public static StemmingRuleSet Read(string path, CultureInfo culture)
		{
			using (StemmingRuleSetReader stemmingRuleSetReader = new StemmingRuleSetReader(path))
			{
				return stemmingRuleSetReader.Read(culture);
			}
		}
	}
}
