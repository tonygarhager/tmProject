using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents licensing status information for a translation provider server.
	/// </summary>
	public class LicensingStatusInformation
	{
		private const string TEAM_EDITION = "tmserver_team";

		private const string ENTERPRISE_EDITION = "tmserver_standard";

		private LicensingInformationEntity _entity;

		private LicensingRestrictionsEntity _restrictions;

		/// <summary>
		/// Gets the current total number of translation units across all translation memories hosted on the server.
		/// </summary>
		public long CurrentTranslationUnitCount => _entity.CurrentUnitCount;

		/// <summary>
		/// Gets the maximum number of translation units allowed by the current license.
		/// </summary>
		public long MaxTranslationUnitCount => _entity.MaxUnitCount;

		/// <summary>
		/// Gets the maximum number of concurrent users allowed by the current license.
		/// </summary>
		public int MaxConcurrentUsers => _entity.MaxConcurrentUsers;

		/// <summary>
		/// Gets the current number of concurrent users logged in.
		/// </summary>
		public int CurrentConcurrentUsers => _entity.CurrentConcurrentUsers;

		/// <summary>
		/// Gets the feature name 
		/// </summary>
		public string FeatureName => _entity.FeatureName;

		/// <summary>
		/// Is true if team edition license
		/// </summary>
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
