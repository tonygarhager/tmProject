using Sdl.Core.Globalization;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public class SniffInfo : IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private bool _IsSupported;

		private Pair<Codepage, DetectionLevel> _detectedEncoding = new Pair<Codepage, DetectionLevel>(new Codepage(), DetectionLevel.Unknown);

		private Pair<Language, DetectionLevel> _detectedSourceLanguage = new Pair<Language, DetectionLevel>(new Language(), DetectionLevel.Unknown);

		private Pair<Language, DetectionLevel> _detectedTargetLanguage = new Pair<Language, DetectionLevel>(new Language(), DetectionLevel.Unknown);

		private EncodingCategory _suggestedTargetEncoding;

		private Dictionary<string, string> _metaData = new Dictionary<string, string>();

		[NonSerialized]
		private int _persistenceId;

		public bool IsSupported
		{
			get
			{
				return _IsSupported;
			}
			set
			{
				_IsSupported = value;
			}
		}

		public Pair<Codepage, DetectionLevel> DetectedEncoding
		{
			get
			{
				return _detectedEncoding;
			}
			set
			{
				_detectedEncoding = value;
			}
		}

		public Pair<Language, DetectionLevel> DetectedSourceLanguage
		{
			get
			{
				return _detectedSourceLanguage;
			}
			set
			{
				_detectedSourceLanguage = value;
			}
		}

		public Pair<Language, DetectionLevel> DetectedTargetLanguage
		{
			get
			{
				return _detectedTargetLanguage;
			}
			set
			{
				_detectedTargetLanguage = value;
			}
		}

		public EncodingCategory SuggestedTargetEncoding
		{
			get
			{
				return _suggestedTargetEncoding;
			}
			set
			{
				_suggestedTargetEncoding = value;
			}
		}

		public IEnumerable<KeyValuePair<string, string>> MetaData
		{
			get
			{
				foreach (KeyValuePair<string, string> metaDatum in _metaData)
				{
					yield return metaDatum;
				}
			}
		}

		public bool HasMetaData => _metaData.Count > 0;

		public int MetaDataCount => _metaData.Count;

		public string this[string key]
		{
			get
			{
				return GetMetaData(key);
			}
			set
			{
				SetMetaData(key, value);
			}
		}

		[XmlIgnore]
		public int PersistenceId
		{
			get
			{
				return _persistenceId;
			}
			set
			{
				_persistenceId = value;
			}
		}

		public SniffInfo()
		{
		}

		protected SniffInfo(SniffInfo other)
		{
			_IsSupported = other._IsSupported;
			if (other._detectedEncoding != null)
			{
				_detectedEncoding = (Pair<Codepage, DetectionLevel>)other._detectedEncoding.Clone();
			}
			if (other._detectedSourceLanguage != null)
			{
				_detectedSourceLanguage = (Pair<Language, DetectionLevel>)other._detectedSourceLanguage.Clone();
			}
			if (other._detectedTargetLanguage != null)
			{
				_detectedTargetLanguage = (Pair<Language, DetectionLevel>)other._detectedTargetLanguage.Clone();
			}
			if (other._metaData.Count != 0)
			{
				foreach (KeyValuePair<string, string> metaDatum in other._metaData)
				{
					_metaData.Add(metaDatum.Key, metaDatum.Value);
				}
			}
			_suggestedTargetEncoding = other._suggestedTargetEncoding;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SniffInfo sniffInfo = (SniffInfo)obj;
			if (!_detectedEncoding.Equals(sniffInfo._detectedEncoding))
			{
				return false;
			}
			if (!_detectedSourceLanguage.Equals(sniffInfo._detectedSourceLanguage))
			{
				return false;
			}
			if (!_detectedTargetLanguage.Equals(sniffInfo._detectedTargetLanguage))
			{
				return false;
			}
			if (_suggestedTargetEncoding != sniffInfo._suggestedTargetEncoding)
			{
				return false;
			}
			if (_IsSupported != sniffInfo._IsSupported)
			{
				return false;
			}
			if (_metaData.Count != sniffInfo._metaData.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> metaDatum in _metaData)
			{
				if (!sniffInfo._metaData.TryGetValue(metaDatum.Key, out string value))
				{
					return false;
				}
				if (metaDatum.Value != value)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (KeyValuePair<string, string> metaDatum in _metaData)
			{
				num ^= ((metaDatum.Key.GetHashCode() << 16) ^ metaDatum.Value.GetHashCode());
			}
			return _IsSupported.GetHashCode() ^ num ^ _detectedEncoding.GetHashCode() ^ _detectedSourceLanguage.GetHashCode() ^ _detectedTargetLanguage.GetHashCode();
		}

		public object Clone()
		{
			return new SniffInfo(this);
		}

		public bool MetaDataContainsKey(string key)
		{
			return _metaData.ContainsKey(key);
		}

		public string GetMetaData(string key)
		{
			if (MetaDataContainsKey(key))
			{
				return _metaData[key];
			}
			return null;
		}

		public void SetMetaData(string key, string value)
		{
			_metaData[key] = value;
		}

		public bool RemoveMetaData(string key)
		{
			return _metaData.Remove(key);
		}

		public void ClearMetaData()
		{
			_metaData.Clear();
		}
	}
}
