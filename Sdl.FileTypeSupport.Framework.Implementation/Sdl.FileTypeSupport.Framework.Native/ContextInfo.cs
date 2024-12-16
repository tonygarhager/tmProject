using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Drawing;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class ContextInfo : AbstractMetaDataContainer, IContextInfo, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private string _Description;

		private string _ContextType;

		private string _DisplayName;

		private string _DisplayCode;

		private Color _DisplayColor;

		private ContextPurpose _Purpose;

		private IFormattingGroup _DefaultFormatting;

		[NonSerialized]
		private int _persistenceId;

		public string Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}

		public string ContextType
		{
			get
			{
				return _ContextType;
			}
			set
			{
				_ContextType = value;
			}
		}

		public string DisplayCode
		{
			get
			{
				return _DisplayCode;
			}
			set
			{
				_DisplayCode = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
			set
			{
				_DisplayName = value;
			}
		}

		public ContextPurpose Purpose
		{
			get
			{
				return _Purpose;
			}
			set
			{
				_Purpose = value;
			}
		}

		public Color DisplayColor
		{
			get
			{
				return _DisplayColor;
			}
			set
			{
				_DisplayColor = value;
			}
		}

		public IFormattingGroup DefaultFormatting
		{
			get
			{
				return _DefaultFormatting;
			}
			set
			{
				_DefaultFormatting = value;
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

		public ContextInfo()
		{
			_ContextType = "sdl:paragraph";
			_DisplayColor = Color.Silver;
		}

		protected ContextInfo(ContextInfo other)
			: base(other)
		{
			_Description = other._Description;
			_ContextType = other._ContextType;
			_DisplayName = other._DisplayName;
			_DisplayCode = other._DisplayCode;
			_Purpose = other._Purpose;
			_DisplayColor = other._DisplayColor;
			if (other._DefaultFormatting != null)
			{
				_DefaultFormatting = (other._DefaultFormatting.Clone() as IFormattingGroup);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			ContextInfo contextInfo = (ContextInfo)obj;
			if (!base.Equals((object)contextInfo))
			{
				return false;
			}
			if (contextInfo._Purpose != _Purpose)
			{
				return false;
			}
			if (contextInfo._Description != _Description)
			{
				return false;
			}
			if (contextInfo._DisplayCode != _DisplayCode)
			{
				return false;
			}
			if (contextInfo._DisplayName != _DisplayName)
			{
				return false;
			}
			if (contextInfo._ContextType != _ContextType)
			{
				return false;
			}
			if (contextInfo._DisplayColor.ToArgb() != _DisplayColor.ToArgb())
			{
				return false;
			}
			if (_DefaultFormatting == null != (contextInfo._DefaultFormatting == null))
			{
				return false;
			}
			if (_DefaultFormatting != null && !_DefaultFormatting.Equals(contextInfo._DefaultFormatting))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ _Purpose.GetHashCode() ^ ((_Description != null) ? _Description.GetHashCode() : 0) ^ ((_DisplayCode != null) ? _DisplayCode.GetHashCode() : 0) ^ ((_DisplayName != null) ? _DisplayName.GetHashCode() : 0) ^ ((_ContextType != null) ? _ContextType.GetHashCode() : 0) ^ _DisplayColor.GetHashCode() ^ ((_DefaultFormatting != null) ? _DefaultFormatting.GetHashCode() : 0);
		}

		public virtual object Clone()
		{
			return new ContextInfo(this);
		}
	}
}
