using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class TextContentItem : AbstractContentItem
	{
		private ITextProperties _Properties;

		public ITextProperties Properties
		{
			get
			{
				return _Properties;
			}
			set
			{
				_Properties = value;
			}
		}

		public TextContentItem(ITextProperties properties)
		{
			_Properties = properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			TextContentItem textContentItem = (TextContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(textContentItem._Properties);
			}
			return _Properties == textContentItem._Properties;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (_Properties == null)
			{
				return base.ToString();
			}
			return _Properties.ToString();
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Text(_Properties);
		}
	}
}
