using System;

namespace Sdl.Core.Api.DataAccess
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityAttribute : Attribute
	{
		public string Schema
		{
			get;
			set;
		}

		public string Table
		{
			get;
			set;
		}

		public string PrimaryKey
		{
			get;
			set;
		}
	}
}
