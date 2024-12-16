using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
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

		public ServerBasedTranslationMemoryLanguageDirection GetLanguageDirection(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			return this.FirstOrDefault((ServerBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguage.Equals(sourceLanguage) && ld.TargetLanguage.Equals(targetLanguage));
		}

		public ServerBasedTranslationMemoryLanguageDirection GetLanguageDirection(string sourceLanguageCode, string targetLanguageCode)
		{
			return this.FirstOrDefault((ServerBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguageCode.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase) && ld.TargetLanguageCode.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase));
		}

		protected override void InsertItem(int index, ServerBasedTranslationMemoryLanguageDirection item)
		{
			item.TranslationProvider = _tm;
			_languageDirectionEntities.Add(item.Entity);
			base.InsertItem(index, item);
		}

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
