using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class IconDescriptor
	{
		private string _originalLocation;

		private string _lazyContent;

		private Icon _icon;

		public string OriginalLocation => _originalLocation;

		public bool IsEmpty
		{
			get
			{
				if (_originalLocation == null)
				{
					return _icon == null;
				}
				return false;
			}
		}

		public bool IsEmbedded
		{
			get
			{
				if (_originalLocation != null)
				{
					return _originalLocation.StartsWith("assembly://");
				}
				return false;
			}
		}

		public bool IsValid
		{
			get
			{
				if (IsEmpty)
				{
					return true;
				}
				if (_icon != null)
				{
					return true;
				}
				try
				{
					Stream rawContentStream = GetRawContentStream();
					rawContentStream.Close();
					Icon icon = GetIcon(new Size(16, 16));
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public string Content
		{
			get
			{
				if (_lazyContent == null && _originalLocation != null)
				{
					_lazyContent = Convert.ToBase64String(GetRawContentBytes());
				}
				return _lazyContent;
			}
		}

		private bool IsIcon
		{
			get
			{
				if (!string.IsNullOrEmpty(_originalLocation))
				{
					return _originalLocation.EndsWith(".ico", StringComparison.InvariantCultureIgnoreCase);
				}
				return false;
			}
		}

		public IconDescriptor()
		{
		}

		public IconDescriptor(string imageLocation)
		{
			_originalLocation = imageLocation;
		}

		public IconDescriptor(string originalLocation, string content)
		{
			_originalLocation = originalLocation;
			_lazyContent = content;
		}

		public IconDescriptor(Icon icon)
		{
			_icon = icon;
		}

		public static IconDescriptor Create(string imageLocation)
		{
			return new IconDescriptor(imageLocation);
		}

		public static IconDescriptor Create(string location, string content)
		{
			return new IconDescriptor(location, content);
		}

		public Image GetImage()
		{
			return GetImage(Size.Empty);
		}

		public Image GetImage(Size size)
		{
			if (_icon != null)
			{
				return GetImageFromIcon(new Icon(_icon, size));
			}
			if (IsIcon)
			{
				using (Icon icon = size.IsEmpty ? new Icon(GetRawContentStream()) : new Icon(GetRawContentStream(), size))
				{
					return GetImageFromIcon(icon);
				}
			}
			return Image.FromStream(GetRawContentStream());
		}

		private Image GetImageFromIcon(Icon icon)
		{
			try
			{
				return icon.ToBitmap();
			}
			catch (ArgumentOutOfRangeException)
			{
				return Bitmap.FromHicon(icon.Handle);
			}
		}

		public Icon GetIcon()
		{
			return GetIcon(Size.Empty);
		}

		public Icon GetIcon(Size size)
		{
			if (_icon != null)
			{
				return new Icon(_icon, size);
			}
			if (IsIcon)
			{
				return new Icon(GetRawContentStream(), size);
			}
			throw new InvalidOperationException("Cannot convert image to icon.");
		}

		private byte[] GetRawContentBytes()
		{
			using (Stream stream = GetRawContentStream())
			{
				int num = (int)stream.Length;
				byte[] array = new byte[num];
				stream.Read(array, 0, num);
				return array;
			}
		}

		private Stream GetRawContentStream()
		{
			if (_lazyContent != null)
			{
				return new MemoryStream(Convert.FromBase64String(_lazyContent));
			}
			if (_originalLocation.StartsWith("assembly://"))
			{
				return GetEmbeddedContentStream(_originalLocation);
			}
			if (Path.IsPathRooted(_originalLocation))
			{
				return File.OpenRead(_originalLocation);
			}
			throw new ArgumentException(string.Format(Resources.InvalidImageLocation, _originalLocation));
		}

		private Stream GetEmbeddedContentStream(string assemblyLocation)
		{
			if (!assemblyLocation.StartsWith("assembly://"))
			{
				throw new ArgumentException("Incorrect format for embeddedResource parameter");
			}
			int num = assemblyLocation.IndexOf("/", 11);
			if (num < 0)
			{
				throw new ArgumentException("Incorrect format for embeddedResource parameter");
			}
			string text = assemblyLocation.Substring(11, num - 11);
			string text2 = assemblyLocation.Substring(num + 1);
			Stream manifestResourceStream = Assembly.Load(text).GetManifestResourceStream(text2);
			if (manifestResourceStream == null)
			{
				throw new ArgumentException(string.Format(Resources.ResourceNotFound, text2, text));
			}
			return manifestResourceStream;
		}
	}
}
