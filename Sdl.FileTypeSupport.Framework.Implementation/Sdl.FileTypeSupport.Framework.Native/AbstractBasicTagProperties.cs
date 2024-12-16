using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public abstract class AbstractBasicTagProperties : AbstractMetaDataContainer, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private string _tagContent;

		private string _displayText;

		private bool _isSoftBreak = true;

		private bool _isWordStop = true;

		private bool _canHide;

		[NonSerialized]
		private int _persistenceId;

		public string TagContent
		{
			get
			{
				return _tagContent;
			}
			set
			{
				_tagContent = value;
			}
		}

		public string DisplayText
		{
			get
			{
				return _displayText;
			}
			set
			{
				_displayText = value;
			}
		}

		public bool IsWordStop
		{
			get
			{
				return _isWordStop;
			}
			set
			{
				_isWordStop = value;
			}
		}

		public bool IsSoftBreak
		{
			get
			{
				return _isSoftBreak;
			}
			set
			{
				_isSoftBreak = value;
			}
		}

		public bool CanHide
		{
			get
			{
				return _canHide;
			}
			set
			{
				_canHide = value;
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

		protected AbstractBasicTagProperties()
		{
		}

		protected AbstractBasicTagProperties(AbstractBasicTagProperties other)
			: base(other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			_displayText = other._displayText;
			_tagContent = other._tagContent;
			_canHide = other._canHide;
			_isSoftBreak = other._isSoftBreak;
			_isWordStop = other._isWordStop;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractBasicTagProperties abstractBasicTagProperties = (AbstractBasicTagProperties)obj;
			if (!base.Equals((object)abstractBasicTagProperties))
			{
				return false;
			}
			if (abstractBasicTagProperties.DisplayText != DisplayText)
			{
				return false;
			}
			if (abstractBasicTagProperties.TagContent != TagContent)
			{
				return false;
			}
			if (abstractBasicTagProperties._isSoftBreak != _isSoftBreak)
			{
				return false;
			}
			if (abstractBasicTagProperties._isWordStop != _isWordStop)
			{
				return false;
			}
			if (abstractBasicTagProperties._canHide != _canHide)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			return hashCode ^ ((_displayText != null) ? _displayText.GetHashCode() : 0) ^ ((_tagContent != null) ? _tagContent.GetHashCode() : 0) ^ _canHide.GetHashCode() ^ _isSoftBreak.GetHashCode() ^ _isWordStop.GetHashCode();
		}

		public override string ToString()
		{
			return _tagContent;
		}

		public abstract object Clone();
	}
}
