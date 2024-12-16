using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class DocumentProperties : IDocumentProperties, ICloneable, ISupportsPersistenceId
	{
		[NonSerialized]
		private IRepetitionsTable _repetitions = new RepetitionsTable();

		[NonSerialized]
		private int _persistenceId;

		private Language _sourceLanguage = new Language();

		private Language _targetLanguage = new Language();

		[NonSerialized]
		private SourceCount _sourceCount;

		private string _lastOpenedAsPath;

		private string _lastSavedAsPath;

		public IRepetitionsTable Repetitions
		{
			get
			{
				return _repetitions;
			}
			set
			{
				_repetitions = value;
			}
		}

		public Language SourceLanguage
		{
			get
			{
				return _sourceLanguage;
			}
			set
			{
				_sourceLanguage = value;
			}
		}

		public Language TargetLanguage
		{
			get
			{
				return _targetLanguage;
			}
			set
			{
				_targetLanguage = value;
			}
		}

		public SourceCount SourceCount
		{
			get
			{
				return _sourceCount;
			}
			set
			{
				_sourceCount = value;
			}
		}

		public string LastOpenedAsPath
		{
			get
			{
				return _lastOpenedAsPath;
			}
			set
			{
				_lastOpenedAsPath = value;
			}
		}

		public string LastSavedAsPath
		{
			get
			{
				return _lastSavedAsPath;
			}
			set
			{
				_lastSavedAsPath = value;
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

		public DocumentProperties()
		{
		}

		protected DocumentProperties(DocumentProperties other)
		{
			if (other._sourceLanguage != null)
			{
				_sourceLanguage = (Language)other._sourceLanguage.Clone();
			}
			else
			{
				_sourceLanguage = null;
			}
			if (other._targetLanguage != null)
			{
				_targetLanguage = (Language)other._targetLanguage.Clone();
			}
			else
			{
				_targetLanguage = null;
			}
			if (other._repetitions != null)
			{
				_repetitions = (IRepetitionsTable)other._repetitions.Clone();
			}
			else
			{
				_repetitions = null;
			}
			if (other._sourceCount != null)
			{
				_sourceCount = (SourceCount)other._sourceCount.Clone();
			}
			_lastOpenedAsPath = other._lastOpenedAsPath;
			_lastSavedAsPath = other._lastSavedAsPath;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			DocumentProperties documentProperties = (DocumentProperties)obj;
			if (documentProperties._sourceLanguage == null != (_sourceLanguage == null))
			{
				return false;
			}
			if (_sourceLanguage != null && !documentProperties._sourceLanguage.Equals(_sourceLanguage))
			{
				return false;
			}
			if (documentProperties._targetLanguage == null != (_targetLanguage == null))
			{
				return false;
			}
			if (_targetLanguage != null && !documentProperties._targetLanguage.Equals(_targetLanguage))
			{
				return false;
			}
			if (documentProperties._repetitions == null != (_repetitions == null))
			{
				return false;
			}
			if (_repetitions != null && !_repetitions.Equals(documentProperties._repetitions))
			{
				return false;
			}
			if (documentProperties._sourceCount == null != (_sourceCount == null))
			{
				return false;
			}
			if (_sourceCount != null && !_sourceCount.Equals(documentProperties._sourceCount))
			{
				return false;
			}
			if (_lastOpenedAsPath != documentProperties._lastOpenedAsPath)
			{
				return false;
			}
			if (_lastSavedAsPath != documentProperties._lastSavedAsPath)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_sourceLanguage != null) ? _sourceLanguage.GetHashCode() : 0) ^ ((_targetLanguage != null) ? _targetLanguage.GetHashCode() : 0) ^ ((_repetitions != null) ? _repetitions.GetHashCode() : 0) ^ ((SourceCount != null) ? _sourceCount.GetHashCode() : 0) ^ ((_lastOpenedAsPath != null) ? _lastOpenedAsPath.GetHashCode() : 0) ^ ((_lastSavedAsPath != null) ? _lastSavedAsPath.GetHashCode() : 0);
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (_sourceLanguage != null)
			{
				text += _sourceLanguage.ToString();
			}
			if (_targetLanguage != null)
			{
				text = text + " => " + _targetLanguage.ToString();
			}
			if (!string.IsNullOrEmpty(_lastSavedAsPath))
			{
				text = text + ": " + _lastSavedAsPath;
			}
			else if (!string.IsNullOrEmpty(_lastOpenedAsPath))
			{
				text = text + ": " + _lastOpenedAsPath;
			}
			return text;
		}

		public object Clone()
		{
			return new DocumentProperties(this);
		}
	}
}
