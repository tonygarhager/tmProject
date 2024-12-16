using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class CloudBasedTranslationMemoryLanguageDirectionCollection : ObservableCollection<CloudBasedTranslationMemoryLanguageDirection>
	{
		private CloudBasedTranslationMemory _tm;

		private EntityCollection<LanguageDirectionEntity> _languageDirectionEntities;

		internal CloudBasedTranslationMemoryLanguageDirection this[Guid id] => this.FirstOrDefault((CloudBasedTranslationMemoryLanguageDirection ld) => ld.Entity.UniqueId.Value == id);

		internal CloudBasedTranslationMemoryLanguageDirectionCollection(CloudBasedTranslationMemory tm, EntityCollection<LanguageDirectionEntity> languageDirectionEntities)
		{
			_tm = tm;
			_languageDirectionEntities = languageDirectionEntities;
			int num = 0;
			foreach (LanguageDirectionEntity languageDirectionEntity in languageDirectionEntities)
			{
				base.InsertItem(num++, new CloudBasedTranslationMemoryLanguageDirection(_tm, languageDirectionEntity));
			}
		}

		public CloudBasedTranslationMemoryLanguageDirection GetLanguageDirection(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			return this.FirstOrDefault((CloudBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguage.Equals(sourceLanguage) && ld.TargetLanguage.Equals(targetLanguage));
		}

		public CloudBasedTranslationMemoryLanguageDirection GetLanguageDirection(string sourceLanguageCode, string targetLanguageCode)
		{
			return this.FirstOrDefault((CloudBasedTranslationMemoryLanguageDirection ld) => ld.SourceLanguageCode.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase) && ld.TargetLanguageCode.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase));
		}

		protected override void InsertItem(int index, CloudBasedTranslationMemoryLanguageDirection item)
		{
			item.TranslationProvider = _tm;
			_languageDirectionEntities.Add(item.Entity);
			base.InsertItem(index, item);
		}

		protected override void ClearItems()
		{
			using (IEnumerator<CloudBasedTranslationMemoryLanguageDirection> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CloudBasedTranslationMemoryLanguageDirection current = enumerator.Current;
					current.TranslationProvider = null;
				}
			}
			_languageDirectionEntities.Clear();
			base.ClearItems();
		}

		protected override void RemoveItem(int index)
		{
			CloudBasedTranslationMemoryLanguageDirection cloudBasedTranslationMemoryLanguageDirection = base[index];
			if (Contains(cloudBasedTranslationMemoryLanguageDirection))
			{
				cloudBasedTranslationMemoryLanguageDirection.TranslationProvider = null;
				_languageDirectionEntities.Remove(cloudBasedTranslationMemoryLanguageDirection.Entity);
				base.RemoveItem(index);
			}
		}

		internal void UpdateEntities(EntityCollection<LanguageDirectionEntity> newLanguageDirectionEntities)
		{
			_languageDirectionEntities = newLanguageDirectionEntities;
			foreach (LanguageDirectionEntity newLanguageDirectionEntity in newLanguageDirectionEntities)
			{
				CloudBasedTranslationMemoryLanguageDirection cloudBasedTranslationMemoryLanguageDirection = this[newLanguageDirectionEntity.UniqueId.Value];
				if (cloudBasedTranslationMemoryLanguageDirection != null)
				{
					cloudBasedTranslationMemoryLanguageDirection.Entity = newLanguageDirectionEntity;
				}
				else
				{
					base.Items.Add(new CloudBasedTranslationMemoryLanguageDirection(_tm, newLanguageDirectionEntity));
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
