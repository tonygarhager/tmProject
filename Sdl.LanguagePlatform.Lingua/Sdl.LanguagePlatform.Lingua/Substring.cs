using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Lingua
{
	[DataContract]
	public struct Substring
	{
		[DataMember]
		public int Start;

		[DataMember]
		public int Length;

		public Substring(int start, int length)
		{
			Start = start;
			Length = length;
		}
	}
}
