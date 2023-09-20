using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000013 RID: 19
	public class NumericCampaignOptionData : CampaignOptionData
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00003FFD File Offset: 0x000021FD
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00004005 File Offset: 0x00002205
		public float MinValue { get; private set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000BF RID: 191 RVA: 0x0000400E File Offset: 0x0000220E
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00004016 File Offset: 0x00002216
		public float MaxValue { get; private set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000401F File Offset: 0x0000221F
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00004027 File Offset: 0x00002227
		public bool IsDiscrete { get; private set; }

		// Token: 0x060000C3 RID: 195 RVA: 0x00004030 File Offset: 0x00002230
		public NumericCampaignOptionData(string identifier, int priorityIndex, CampaignOptionEnableState enableState, Func<float> getValue, Action<float> setValue, float minValue, float maxValue, bool isDiscrete, Func<CampaignOptionDisableStatus> getIsDisabledWithReason = null, bool isRelatedToDifficultyPreset = false, Func<float, CampaignOptionsDifficultyPresets> onGetDifficultyPresetFromValue = null, Func<CampaignOptionsDifficultyPresets, float> onGetValueFromDifficultyPreset = null)
			: base(identifier, priorityIndex, enableState, getValue, setValue, getIsDisabledWithReason, isRelatedToDifficultyPreset, onGetDifficultyPresetFromValue, onGetValueFromDifficultyPreset)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.IsDiscrete = isDiscrete;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000406A File Offset: 0x0000226A
		public override CampaignOptionDataType GetDataType()
		{
			return CampaignOptionDataType.Numeric;
		}
	}
}
