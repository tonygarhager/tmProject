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
	/// <summary>
	/// Represents a collection of language resource bundles within a translation memory (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.LanguageResourceBundles" />) 
	/// or a language resources template (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate.LanguageResourceBundles" />).
	/// A language resource bundle holds custom language resources for a certain language (abbreviation, ordinal followers, variables and segmentation rules).
	/// </summary>
	/// <remarks>Changes to this collection should be persisted by saving the translation memory or the language resources template
	/// to which the collection belongs.</remarks>
	public class LanguageResourceBundleCollection : ObservableCollection<LanguageResourceBundle>
	{
		private IResourceDataAccessor _lazyResourceDataAccessor;

		/// <summary>
		/// Gets the language resource bundle for the given language, if it exists in this collection.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns>The language resources bundle for the specified language, or null if no such language resource bundle exists.</returns>
		public LanguageResourceBundle this[CultureInfo language] => this.FirstOrDefault((LanguageResourceBundle b) => b.Language.Equals(language));

		/// <summary>
		/// Gets the language resource bundle for the given language code, if it exists in this collection.
		/// </summary>
		/// <param name="languageCode">The language code.</param>
		/// <returns>The language resources bundle for the specified language, or null if no such language resource bundle exists.</returns>
		public LanguageResourceBundle this[string languageCode] => this.FirstOrDefault((LanguageResourceBundle b) => b.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));

		/// <summary>
		/// Gets a reference to the internal resource data accessor used to access the resource storage.
		/// </summary>
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

		/// <summary>
		/// Creates a new, empty language resource bundle collection.
		/// </summary>
		/// <remarks>This constructor should typically only be used by implementers of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate" /> or <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory" />.</remarks>
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

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		protected override void InsertItem(int index, LanguageResourceBundle item)
		{
			item.Entities = Entities;
			base.InsertItem(index, item);
		}

		/// <summary>
		/// Removes all items from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			Entities.Clear();
			base.ClearItems();
		}

		/// <summary>
		/// Removes the item at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
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
