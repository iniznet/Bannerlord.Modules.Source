using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000011 RID: 17
	public interface ICampaignOptionData
	{
		// Token: 0x060000B0 RID: 176
		CampaignOptionDataType GetDataType();

		// Token: 0x060000B1 RID: 177
		int GetPriorityIndex();

		// Token: 0x060000B2 RID: 178
		bool IsRelatedToDifficultyPreset();

		// Token: 0x060000B3 RID: 179
		float GetValueFromDifficultyPreset(CampaignOptionsDifficultyPresets preset);

		// Token: 0x060000B4 RID: 180
		string GetIdentifier();

		// Token: 0x060000B5 RID: 181
		CampaignOptionEnableState GetEnableState();

		// Token: 0x060000B6 RID: 182
		string GetName();

		// Token: 0x060000B7 RID: 183
		string GetDescription();

		// Token: 0x060000B8 RID: 184
		float GetValue();

		// Token: 0x060000B9 RID: 185
		void SetValue(float value);

		// Token: 0x060000BA RID: 186
		CampaignOptionDisableStatus GetIsDisabledWithReason();
	}
}
