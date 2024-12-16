using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class SupportsPersistenceId : ISupportsPersistenceId
	{
		[NonSerialized]
		private int _persistenceId;

		[XmlIgnore]
		public virtual int PersistenceId
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
	}
}
