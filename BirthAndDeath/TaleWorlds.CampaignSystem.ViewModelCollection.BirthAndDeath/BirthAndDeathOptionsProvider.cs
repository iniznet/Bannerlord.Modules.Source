using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.BirthAndDeath
{
	// Token: 0x02000003 RID: 3
	public class BirthAndDeathOptionsProvider : ICampaignOptionProvider
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
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

		// Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
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
