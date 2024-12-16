using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	public class ChangeContextContentItem : AbstractContentItem
	{
		private IContextProperties _Properties;

		public IContextProperties Properties
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

		public ChangeContextContentItem(IContextProperties properties)
		{
			_Properties = properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			ChangeContextContentItem changeContextContentItem = (ChangeContextContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(changeContextContentItem._Properties);
			}
			return _Properties == changeContextContentItem._Properties;
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
			output.ChangeContext(_Properties);
		}
	}
}
