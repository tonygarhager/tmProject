using Sdl.LanguagePlatform.Core.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.Core.LanguageProcessing.Resources
{
	public class CompositeResourceDataAccessor : IResourceDataAccessor
	{
		private readonly List<IResourceDataAccessor> _individualAccessors;

		public int Count => _individualAccessors.Count;

		public IResourceDataAccessor this[int index] => _individualAccessors[index];

		public CompositeResourceDataAccessor(bool addDefaultAccessor)
		{
			_individualAccessors = new List<IResourceDataAccessor>();
			if (addDefaultAccessor)
			{
				AddDefaultAccessor();
			}
		}

		public void Insert(int index, IResourceDataAccessor racc)
		{
			_individualAccessors.Insert(index, racc);
		}

		public void Insert(IResourceDataAccessor racc)
		{
			_individualAccessors.Insert(0, racc);
		}

		public void AddDefaultAccessor()
		{
			_individualAccessors.Add(new ResourceFileResourceAccessor());
		}

		public void Add(IResourceDataAccessor racc)
		{
			_individualAccessors.Add(racc);
		}

		public ResourceStatus GetResourceStatus(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			foreach (IResourceDataAccessor individualAccessor in _individualAccessors)
			{
				ResourceStatus resourceStatus = individualAccessor.GetResourceStatus(culture, t, fallback);
				if (resourceStatus != ResourceStatus.NotAvailable)
				{
					return resourceStatus;
				}
			}
			return ResourceStatus.NotAvailable;
		}

		public Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			foreach (IResourceDataAccessor individualAccessor in _individualAccessors)
			{
				ResourceStatus resourceStatus = individualAccessor.GetResourceStatus(culture, t, fallback);
				if (resourceStatus != ResourceStatus.NotAvailable)
				{
					return individualAccessor.ReadResourceData(culture, t, fallback);
				}
			}
			return null;
		}

		public byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			using (Stream stream = ReadResourceData(culture, t, fallback))
			{
				return ResourceStorage.StreamToByteArray(stream);
			}
		}

		public List<CultureInfo> GetSupportedCultures(LanguageResourceType t)
		{
			List<CultureInfo> list = new List<CultureInfo>();
			foreach (IResourceDataAccessor individualAccessor in _individualAccessors)
			{
				foreach (CultureInfo supportedCulture in individualAccessor.GetSupportedCultures(t))
				{
					if (!list.Contains(supportedCulture))
					{
						list.Add(supportedCulture);
					}
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}
	}
}
