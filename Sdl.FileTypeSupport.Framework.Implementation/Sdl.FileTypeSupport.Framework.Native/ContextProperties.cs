using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class ContextProperties : IContextProperties, ICloneable, ISupportsPersistenceId
	{
		private List<IContextInfo> _Contexts = new List<IContextInfo>();

		private IStructureInfo _StructureInfo;

		[NonSerialized]
		private int _persistenceId;

		public IList<IContextInfo> Contexts => _Contexts;

		public IStructureInfo StructureInfo
		{
			get
			{
				return _StructureInfo;
			}
			set
			{
				_StructureInfo = value;
			}
		}

		public IFormattingGroup EffectiveDefaultFormatting
		{
			get
			{
				IFormattingGroup formattingGroup = null;
				foreach (IContextInfo context in _Contexts)
				{
					if (context.DefaultFormatting != null)
					{
						if (formattingGroup == null)
						{
							formattingGroup = new FormattingGroup();
						}
						formattingGroup.UnderrideWith(context.DefaultFormatting);
					}
				}
				return formattingGroup;
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

		public ContextProperties()
		{
		}

		protected ContextProperties(ContextProperties other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (IContextInfo context in other._Contexts)
			{
				_Contexts.Add((IContextInfo)context.Clone());
			}
			_StructureInfo = other._StructureInfo;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			ContextProperties contextProperties = (ContextProperties)obj;
			if (contextProperties._Contexts.Count != _Contexts.Count)
			{
				return false;
			}
			for (int i = 0; i < _Contexts.Count; i++)
			{
				if (!contextProperties._Contexts[i].Equals(_Contexts[i]))
				{
					return false;
				}
			}
			if (_StructureInfo == null != (contextProperties._StructureInfo == null))
			{
				return false;
			}
			if (_StructureInfo != null && !_StructureInfo.Equals(contextProperties._StructureInfo))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (IContextInfo context in _Contexts)
			{
				num ^= context.GetHashCode();
			}
			return num ^ ((_StructureInfo != null) ? _StructureInfo.GetHashCode() : 0);
		}

		public object Clone()
		{
			return new ContextProperties(this);
		}
	}
}
