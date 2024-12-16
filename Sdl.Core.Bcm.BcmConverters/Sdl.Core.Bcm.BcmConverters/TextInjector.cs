using Sdl.FileTypeSupport.Framework.NativeApi;
using System.IO;
using System.Text;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class TextInjector : AbstractNativeFileParser
	{
		public string Text
		{
			get;
			set;
		}

		public Encoding Encoding
		{
			get;
			set;
		}

		protected override bool DuringParsing()
		{
			StreamReader streamReader = new StreamReader(new MemoryStream(Encoding.GetBytes(Text)), Encoding);
			ITextProperties textInfo = PropertiesFactory.CreateTextProperties(streamReader.ReadToEnd());
			Output.Text(textInfo);
			return false;
		}
	}
}
