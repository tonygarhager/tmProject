using System;

namespace Sdl.Core.Api.DataAccess
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RelationshipAttribute : Attribute
	{
		public RelationshipType Type
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string RelationshipKey
		{
			get;
			set;
		}

		public string OverrideTargetTableKeyColumn
		{
			get;
			set;
		}

		public string OrderBy
		{
			get;
			set;
		}

		public bool Desc
		{
			get;
			set;
		}

		public string LinkTable
		{
			get;
			set;
		}

		public string LinkSourceKey
		{
			get;
			set;
		}

		public string LinkTargetKey
		{
			get;
			set;
		}
	}
}
