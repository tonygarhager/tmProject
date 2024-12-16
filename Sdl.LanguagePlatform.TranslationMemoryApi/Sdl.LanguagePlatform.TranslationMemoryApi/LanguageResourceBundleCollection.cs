using Sdl.Core.Api.DataAccess;
using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class LanguageResourceBundleCollection : ObservableCollection<LanguageResourceBundle>
	{
		private IResourceDataAccessor _lazyResourceDataAccessor;

		public LanguageResourceBundle this[CultureInfo language] => this.FirstOrDefault((LanguageResourceBundle b) => b.Language.Equals(language));

		public LanguageResourceBundle this[string languageCode] => this.FirstOrDefault((LanguageResourceBundle b) => b.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));

		public IResourceDataAccessor ResourceDataAccessor
		{
			get
			{
				SaveToEntities();
				if (_lazyResourceDataAccessor == null)
				{
					CompositeResourceDataAccessor compositeResourceDataAccessor = new CompositeResourceDataAccessor(addDefaultAccessor: true);
					compositeResourceDataAccessor.Insert(new LanguageResourceEntityCollectionDataAccessor(Entities));
					_lazyResourceDataAccessor = compositeResourceDataAccessor;
				}
				return _lazyResourceDataAccessor;
			}
		}

		internal EntityCollection<LanguageResourceEntity> Entities
		{
			get;
			private set;
		}

		public LanguageResourceBundleCollection()
		{
			Entities = new EntityCollection<LanguageResourceEntity>();
		}

		internal LanguageResourceBundleCollection(EntityCollection<LanguageResourceEntity> entities)
		{
			Entities = entities;
			List<string> languages = GetLanguages(Entities);
			foreach (string item in languages)
			{
				base.InsertItem(base.Count, new LanguageResourceBundle(item, Entities));
			}
		}

		private static List<string> GetLanguages(EntityCollection<LanguageResourceEntity> entities)
		{
			List<string> list = new List<string>();
			foreach (LanguageResourceEntity entity in entities)
			{
				if (!list.Contains(entity.CultureName, StringComparer.OrdinalIgnoreCase))
				{
					list.Add(entity.CultureName);
				}
			}
			return list;
		}

		protected override void InsertItem(int index, LanguageResourceBundle item)
		{
			item.Entities = Entities;
			base.InsertItem(index, item);
		}

		protected override void ClearItems()
		{
			Entities.Clear();
			base.ClearItems();
		}

		protected override void RemoveItem(int index)
		{
			LanguageResourceBundle languageResourceBundle = base[index];
			languageResourceBundle.RemoveEntities();
			base.RemoveItem(index);
		}

		internal void SaveToEntities()
		{
			foreach (LanguageResourceBundle item in base.Items)
			{
				item.SaveToEntities();
			}
		}

		internal void UpdateEntities(EntityCollection<LanguageResourceEntity> newLanguageResourceEntities)
		{
			Entities = newLanguageResourceEntities;
			List<string> languages = GetLanguages(Entities);
			List<LanguageResourceBundle> list = new List<LanguageResourceBundle>();
			foreach (string item in languages)
			{
				LanguageResourceBundle languageResourceBundle = this[item];
				if (languageResourceBundle == null)
				{
					languageResourceBundle = new LanguageResourceBundle(item, Entities);
				}
				else
				{
					languageResourceBundle.Entities = Entities;
					languageResourceBundle.DiscardChanges();
				}
				list.Add(languageResourceBundle);
			}
			base.Items.Clear();
			foreach (LanguageResourceBundle item2 in list)
			{
				base.Items.Add(item2);
			}
		}
	}
}
