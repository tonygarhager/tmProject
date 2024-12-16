using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class CustomInfoProperties : ICustomInfoProperties, ICloneable, ISupportsPersistenceId
	{
		private string _NamespaceUri;

		private string _ValueXml;

		[NonSerialized]
		private int _persistenceId;

		public virtual string NamespaceUri
		{
			get
			{
				return _NamespaceUri;
			}
			set
			{
				_NamespaceUri = value;
			}
		}

		public virtual string ValueXml
		{
			get
			{
				return _ValueXml;
			}
			set
			{
				_ValueXml = value;
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

		public CustomInfoProperties()
		{
		}

		protected CustomInfoProperties(CustomInfoProperties other)
		{
			_NamespaceUri = other._NamespaceUri;
			_ValueXml = other._ValueXml;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			CustomInfoProperties customInfoProperties = (CustomInfoProperties)obj;
			if (customInfoProperties._NamespaceUri != _NamespaceUri)
			{
				return false;
			}
			if (customInfoProperties._ValueXml != _ValueXml)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_NamespaceUri != null) ? _NamespaceUri.GetHashCode() : 0) ^ ((_ValueXml != null) ? _ValueXml.GetHashCode() : 0);
		}

		public virtual object Clone()
		{
			return new CustomInfoProperties(this);
		}
	}
}
