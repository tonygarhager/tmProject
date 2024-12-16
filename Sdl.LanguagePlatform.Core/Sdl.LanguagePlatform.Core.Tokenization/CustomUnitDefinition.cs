using System;
using System.Linq;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class CustomUnitDefinition : ICloneable
	{
		public Unit Unit
		{
			get;
			set;
		} = Unit.NoUnit;


		public string CategoryName
		{
			get;
			set;
		}

		public object Clone()
		{
			return new CustomUnitDefinition
			{
				CategoryName = ((CategoryName == null) ? null : new string(CategoryName.ToArray())),
				Unit = Unit
			};
		}
	}
}
