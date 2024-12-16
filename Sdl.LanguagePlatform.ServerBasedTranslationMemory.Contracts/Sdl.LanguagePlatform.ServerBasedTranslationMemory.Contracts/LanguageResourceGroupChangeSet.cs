using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "LanguageResourceGroupChangeSet")]
	[KnownType(typeof(LanguageResourceUpdate))]
	public class LanguageResourceGroupChangeSet : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private List<LanguageResourceUpdate> _updates = new List<LanguageResourceUpdate>();

		public ExtensionDataObject ExtensionData
		{
			get
			{
				return _extensionDataObject;
			}
			set
			{
				_extensionDataObject = value;
			}
		}

		[DataMember]
		public List<LanguageResourceUpdate> LanguageResourceUpdates
		{
			get
			{
				return _updates;
			}
			set
			{
				_updates = value;
			}
		}

		public bool HasChanges
		{
			get
			{
				if (_updates != null)
				{
					return _updates.Count > 0;
				}
				return false;
			}
		}

		public static LanguageResourceGroupChangeSet GenerateChangeSet(ICollection<LanguageResourceEntity> newLanguageResources)
		{
			return GenerateChangeSet(new EntityCollection<LanguageResourceEntity>(), newLanguageResources);
		}

		public static LanguageResourceGroupChangeSet GenerateChangeSet(ICollection<LanguageResourceEntity> originalLanguageResources, ICollection<LanguageResourceEntity> newLanguageResources)
		{
			LanguageResourceGroupChangeSet languageResourceGroupChangeSet = new LanguageResourceGroupChangeSet();
			foreach (LanguageResourceEntity newLanguageResource in newLanguageResources)
			{
				LanguageResourceEntity languageResourceEntity = Find(originalLanguageResources, newLanguageResource);
				if (languageResourceEntity != null)
				{
					if (newLanguageResource.Data != null && !DataEquals(newLanguageResource.Data, languageResourceEntity.Data))
					{
						LanguageResourceUpdate languageResourceUpdate = new LanguageResourceUpdate();
						languageResourceUpdate.UpdateType = LanguageResourceUpdateType.Update;
						languageResourceUpdate.CultureName = newLanguageResource.CultureName;
						languageResourceUpdate.Type = newLanguageResource.Type.Value;
						languageResourceUpdate.Data = newLanguageResource.Data;
						languageResourceGroupChangeSet.LanguageResourceUpdates.Add(languageResourceUpdate);
					}
				}
				else
				{
					LanguageResourceUpdate languageResourceUpdate2 = new LanguageResourceUpdate();
					languageResourceUpdate2.UpdateType = LanguageResourceUpdateType.Add;
					languageResourceUpdate2.CultureName = newLanguageResource.CultureName;
					languageResourceUpdate2.Type = newLanguageResource.Type.Value;
					languageResourceUpdate2.Data = newLanguageResource.Data;
					languageResourceGroupChangeSet.LanguageResourceUpdates.Add(languageResourceUpdate2);
				}
			}
			foreach (LanguageResourceEntity originalLanguageResource in originalLanguageResources)
			{
				if (Find(newLanguageResources, originalLanguageResource) == null)
				{
					LanguageResourceUpdate languageResourceUpdate3 = new LanguageResourceUpdate();
					languageResourceUpdate3.UpdateType = LanguageResourceUpdateType.Delete;
					languageResourceUpdate3.CultureName = originalLanguageResource.CultureName;
					languageResourceUpdate3.Type = originalLanguageResource.Type.Value;
					languageResourceGroupChangeSet.LanguageResourceUpdates.Add(languageResourceUpdate3);
				}
			}
			return languageResourceGroupChangeSet;
		}

		private static LanguageResourceEntity Find(ICollection<LanguageResourceEntity> entities, LanguageResourceEntity entity)
		{
			return entities.FirstOrDefault((LanguageResourceEntity lr) => lr.CultureName.Equals(entity.CultureName, StringComparison.OrdinalIgnoreCase) && lr.Type.Value == entity.Type.Value);
		}

		private static bool DataEquals(byte[] data1, byte[] data2)
		{
			if (data1.Length != data2.Length)
			{
				return false;
			}
			for (int i = 0; i < data1.Length; i++)
			{
				if (data1[i] != data2[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
