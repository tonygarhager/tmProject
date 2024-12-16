using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class LicensingStatusInformation
	{
		private const string TEAM_EDITION = "tmserver_team";

		private const string ENTERPRISE_EDITION = "tmserver_standard";

		private LicensingInformationEntity _entity;

		private LicensingRestrictionsEntity _restrictions;

		public long CurrentTranslationUnitCount => _entity.CurrentUnitCount;

		public long MaxTranslationUnitCount => _entity.MaxUnitCount;

		public int MaxConcurrentUsers => _entity.MaxConcurrentUsers;

		public int CurrentConcurrentUsers => _entity.CurrentConcurrentUsers;

		public string FeatureName => _entity.FeatureName;

		public bool IsTeamEdition => _entity.FeatureName == "tmserver_team";

		internal LicensingStatusInformation(LicensingRestrictionsEntity restrictions)
		{
			if (restrictions == null)
			{
				throw new ArgumentNullException("restrictions");
			}
			_restrictions = restrictions;
			GetLicensingInformation();
		}

		private void GetLicensingInformation()
		{
			LicensingInformationEntity licensingInformationEntity = new LicensingInformationEntity();
			licensingInformationEntity.CurrentUnitCount = _restrictions.UnitCount.Value;
			licensingInformationEntity.MaxConcurrentUsers = _restrictions.MaxConcurrentUsers;
			licensingInformationEntity.MaxUnitCount = _restrictions.MaxUnitCount;
			licensingInformationEntity.CurrentConcurrentUsers = _restrictions.ConcurrentUsers;
			licensingInformationEntity.FeatureName = _restrictions.FeatureName;
			_entity = licensingInformationEntity;
		}
	}
}
