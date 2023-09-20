using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000010 RID: 16
	public struct CampaignOptionDisableStatus
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00003F8E File Offset: 0x0000218E
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00003F96 File Offset: 0x00002196
		public bool IsDisabled { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00003F9F File Offset: 0x0000219F
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00003FA7 File Offset: 0x000021A7
		public string DisabledReason { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00003FB0 File Offset: 0x000021B0
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00003FB8 File Offset: 0x000021B8
		public float ValueIfDisabled { get; private set; }

		// Token: 0x060000AF RID: 175 RVA: 0x00003FC1 File Offset: 0x000021C1
		public CampaignOptionDisableStatus(bool isDisabled, string disabledReason, float valueIfDisabled = -1f)
		{
			this.IsDisabled = isDisabled;
			this.DisabledReason = disabledReason;
			this.ValueIfDisabled = valueIfDisabled;
		}
	}
}
