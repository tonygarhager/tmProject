using System;

namespace Sdl.Core.Api.DataAccess
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property)]
	public class EntityColumnAttribute : Attribute
	{
		public string ColumnName
		{
			get;
			set;
		}
	}
}
