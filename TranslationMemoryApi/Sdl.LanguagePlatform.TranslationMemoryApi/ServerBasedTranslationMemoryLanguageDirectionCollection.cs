using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the collection of language directions of a server-based translation memory (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory" />).
	/// Language directions can be added to and removed from this collection. In order to persist these changes you should save the translation memory itself
	/// (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.Save" />). 
	/// <remarks>
	/// <para>
	/// Saving a translation memory after adding or removing language directions will result in the corresponding physical bilingual translation memories being added to or deleted from the container
	/// which hosts the server-based translation memory.
	/// </para>
	/// <para>
	/// All the language directions in a given translation memory should have a unique source-target language combination. Multiple source language are supported.
	/// </para>
	/// </remarks> 
	/// </summary>
	public class ServerBasedTranslationMemoryLanguageDirectionCollection : ObservableCollection<ServerBasedTranslationMemoryLanguageDirection>
	{
		private ServerBasedTranslationMemory _tm;

		private EntityCollection<LanguageDirectionEntity> _languageDirectionEntities;

		internal ServerBasedTranslationMemoryLanguageDirection this[Guid id] => this.FirstOrDefault((ServerBasedTranslationMemoryLanguageDirection ld) => ld.Entity.UniqueId.Value == id);

		internal ServerBasedTranslationMemoryLanguageDirectionCollection(ServerBasedTranslationMemory tm, EntityCollection<LanguageDirectionEntity> languageDirectionEntities)
		{
			_tm = tm;
			_languageDirectionEntities = languageDirectionEntities;
			int num = 0;
			foreach (LanguageDirectionEntity languageDirectionEntity in languageDirectionEntities)
			{
				base.InsertItem(num++, new ServerBasedTranslationMemoryLanguageDirection(_tm, languageDirectionEntity));
			}
		}

		/// <summary>
		/// Gets the language direction with the specified source and target language.
		/// </summary>
		/// <param name="sourceLanguage">A source language.</param>
		/// <param name="targetLanguage">A target language.</param>
		/// <returns>The language direction matching the specified source and target language; or null if no such language direction exists.</returns>
		public ServerBasedTranslationMemoryLanguageDirection GetLanguageDirection(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			return this.FirstOrDefault((ServerBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguage.Equals(sourceLanguage) && ld.TargetLanguage.Equals(targetLanguage));
		}

		/// <summary>
		/// Gets the language direction with the specified source and target language code.
		/// </summary>
		/// <param name="sourceLanguageCode">A source language code.</param>
		/// <param name="targetLanguageCode">A target language code.</param>
		/// <returns>The language direction matching the specified source and target language; or null if no such language direction exists.</returns>
		public ServerBasedTranslationMemoryLanguageDirection GetLanguageDirection(string sourceLanguageCode, string targetLanguageCode)
		{
			return this.FirstOrDefault((ServerBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguageCode.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase) && ld.TargetLanguageCode.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		protected override void InsertItem(int index, ServerBasedTranslationMemoryLanguageDirection item)
		{
			item.TranslationProvider = _tm;
			_languageDirectionEntities.Add(item.Entity);
			base.InsertItem(index, item);
		}

		/// <summary>
		/// Removes all items from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			using (IEnumerator<ServerBasedTranslationMemoryLanguageDirection> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ServerBasedTranslationMemoryLanguageDirection current = enumerator.Current;
					current.TranslationProvider = null;
				}
			}
			_languageDirectionEntities.Clear();
			base.ClearItems();
		}

		/// <summary>
		/// Removes the item at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		protected override void RemoveItem(int index)
		{
			ServerBasedTranslationMemoryLanguageDirection serverBasedTranslationMemoryLanguageDirection = base[index];
			if (Contains(serverBasedTranslationMemoryLanguageDirection))
			{
				serverBasedTranslationMemoryLanguageDirection.TranslationProvider = null;
				_languageDirectionEntities.Remove(serverBasedTranslationMemoryLanguageDirection.Entity);
				base.RemoveItem(index);
			}
		}

		internal void UpdateEntities(EntityCollection<LanguageDirectionEntity> newLanguageDirectionEntities)
		{
			_languageDirectionEntities = newLanguageDirectionEntities;
			foreach (LanguageDirectionEntity newLanguageDirectionEntity in newLanguageDirectionEntities)
			{
				ServerBasedTranslationMemoryLanguageDirection serverBasedTranslationMemoryLanguageDirection = this[newLanguageDirectionEntity.UniqueId.Value];
				if (serverBasedTranslationMemoryLanguageDirection != null)
				{
					serverBasedTranslationMemoryLanguageDirection.Entity = newLanguageDirectionEntity;
				}
				else
				{
					base.Items.Add(new ServerBasedTranslationMemoryLanguageDirection(_tm, newLanguageDirectionEntity));
				}
			}
			int i;
			for (i = base.Count - 1; i > 0; i--)
			{
				LanguageDirectionEntity entity = newLanguageDirectionEntities.FirstOrDefault((LanguageDirectionEntity ld) => ld.UniqueId.Value == base.Items[i].Entity.UniqueId.Value);
				if (entity == null)
				{
					base.Items.RemoveAt(i);
				}
			}
		}
	}
}