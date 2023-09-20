using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000012 RID: 18
	public class BooleanCampaignOptionData : CampaignOptionData
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00003FD8 File Offset: 0x000021D8
		public BooleanCampaignOptionData(string identifier, int priorityIndex, CampaignOptionEnableState enableState, Func<float> getValue, Action<float> setValue, Func<CampaignOptionDisableStatus> getIsDisabledWithReason = null, bool isRelatedToDifficultyPreset = false, Func<float, CampaignOptionsDifficultyPresets> onGetDifficultyPresetFromValue = null, Func<CampaignOptionsDifficultyPresets, float> onGetValueFromDifficultyPreset = null)
			: base(identifier, priorityIndex, enableState, getValue, setValue, getIsDisabledWithReason, isRelatedToDifficultyPreset, onGetDifficultyPresetFromValue, onGetValueFromDifficultyPreset)
		{
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00003FFA File Offset: 0x000021FA
		public override CampaignOptionDataType GetDataType()
		{
			return CampaignOptionDataType.Boolean;
		}
	}
}
