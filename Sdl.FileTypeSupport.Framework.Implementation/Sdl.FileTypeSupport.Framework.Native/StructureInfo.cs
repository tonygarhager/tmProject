using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class StructureInfo : IStructureInfo, ICloneable, ISupportsPersistenceId
	{
		private IContextInfo _ContextInfo;

		private bool _MustUseDisplayName;

		private IStructureInfo _ParentStructure;

		private string _Id;

		[NonSerialized]
		private int _persistenceId;

		public IContextInfo ContextInfo
		{
			get
			{
				return _ContextInfo;
			}
			set
			{
				_ContextInfo = value;
			}
		}

		public bool MustUseDisplayName
		{
			get
			{
				return _MustUseDisplayName;
			}
			set
			{
				_MustUseDisplayName = value;
			}
		}

		public IStructureInfo ParentStructure
		{
			get
			{
				return _ParentStructure;
			}
			set
			{
				_ParentStructure = value;
			}
		}

		public string Id => _Id;

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

		public StructureInfo()
		{
			_Id = Guid.NewGuid().ToString();
		}

		public StructureInfo(IContextInfo contextInfo, bool mustUseDisplayName, IStructureInfo parentStructure)
			: this()
		{
			_ContextInfo = contextInfo;
			_MustUseDisplayName = mustUseDisplayName;
			_ParentStructure = parentStructure;
		}

		public StructureInfo(StructureInfo other)
		{
			_ContextInfo = other._ContextInfo;
			_MustUseDisplayName = other._MustUseDisplayName;
			_ParentStructure = other._ParentStructure;
			_Id = other._Id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			StructureInfo structureInfo = (StructureInfo)obj;
			if (_Id != structureInfo._Id)
			{
				return false;
			}
			if (_ContextInfo == null != (structureInfo._ContextInfo == null))
			{
				return false;
			}
			if (_ContextInfo != null && !_ContextInfo.Equals(structureInfo._ContextInfo))
			{
				return false;
			}
			if (_MustUseDisplayName != structureInfo._MustUseDisplayName)
			{
				return false;
			}
			if (_ParentStructure == null != (structureInfo._ParentStructure == null))
			{
				return false;
			}
			if (_ParentStructure != null && !_ParentStructure.Equals(structureInfo._ParentStructure))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _Id.GetHashCode();
		}

		public object Clone()
		{
			return new StructureInfo(this);
		}
	}
}
