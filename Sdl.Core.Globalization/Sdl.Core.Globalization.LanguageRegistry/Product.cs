using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Globalization.LanguageRegistry
{
	[Serializable]
	public class Product
	{
		[DataMember]
		public string ProductId
		{
			get;
			set;
		}
	}
}
