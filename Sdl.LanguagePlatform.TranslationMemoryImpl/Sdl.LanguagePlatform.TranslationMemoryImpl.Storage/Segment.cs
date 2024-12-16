using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class Segment
	{
		private List<int> _features;

		private List<int> _concordanceFeatures;

		public long Hash
		{
			get;
			set;
		}

		public long RelaxedHash
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public string PlainText
		{
			get;
			set;
		}

		public byte[] SerializedTags
		{
			get;
			set;
		}

		public List<int> Features
		{
			get
			{
				return _features ?? (_features = new List<int>());
			}
			set
			{
				_features = value;
			}
		}

		public List<int> ConcordanceFeatures
		{
			get
			{
				return _concordanceFeatures ?? (_concordanceFeatures = new List<int>());
			}
			set
			{
				_concordanceFeatures = value;
			}
		}

		public Segment(long hash, long relaxedHash, string text, byte[] serializedTags)
			: this(hash, relaxedHash, text, null, serializedTags)
		{
		}

		public Segment(long hash, long relaxedHash, string text, string plainText, byte[] serializedTags)
		{
			Hash = hash;
			RelaxedHash = relaxedHash;
			Text = text;
			PlainText = plainText;
			SerializedTags = serializedTags;
		}
	}
}
