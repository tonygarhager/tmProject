using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class LockedContainer : MarkupDataContainer, ILockedContainer, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ICloneable, ISupportsPersistenceId
	{
		private ILockedContent _LockedContent;

		[NonSerialized]
		private int _persistenceId;

		[NonSerialized]
		private int _uniqueId;

		public ILockedContent LockedContent
		{
			get
			{
				return _LockedContent;
			}
			set
			{
				_LockedContent = value;
			}
		}

		public override bool CanBeSplit => false;

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

		[XmlIgnore]
		int ISupportsUniqueId.UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				_uniqueId = value;
			}
		}

		public LockedContainer()
		{
		}

		protected LockedContainer(LockedContainer other)
		{
			_PublicContainer = this;
			foreach (IAbstractMarkupData item in other)
			{
				Add((IAbstractMarkupData)item.Clone());
			}
		}

		public override object Clone()
		{
			return new LockedContainer(this);
		}
	}
}
