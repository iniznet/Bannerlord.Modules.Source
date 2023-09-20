using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.BirthAndDeath
{
	public class BirthAndDeathOptionsProvider : ICampaignOptionProvider
	{
		public IEnumerable<ICampaignOptionData> GetGameplayCampaignOptions()
		{
			yield return new BooleanCampaignOptionData("IsLifeDeathCycleEnabled", 890, 2, delegate
			{
				if (!CampaignOptions.IsLifeDeathCycleDisabled)
				{
					return 1f;
				}
				return 0f;
			}, delegate(float value)
			{
				CampaignOptions.IsLifeDeathCycleDisabled = value == 0f;
			}, null, false, null, null);
			yield break;
		}

		public IEnumerable<ICampaignOptionData> GetCharacterCreationCampaignOptions()
		{
			yield return new BooleanCampaignOptionData("IsLifeDeathCycleEnabled", 890, 1, delegate
			{
				if (!CampaignOptions.IsLifeDeathCycleDisabled)
				{
					return 1f;
				}
				return 0f;
			}, delegate(float value)
			{
				CampaignOptions.IsLifeDeathCycleDisabled = value == 0f;
			}, null, false, null, null);
			yield break;
		}
	}
}
