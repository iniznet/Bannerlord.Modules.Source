using System;

namespace TaleWorlds.CampaignSystem.Party
{
	// Token: 0x020002AF RID: 687
	public struct TroopTradeDifference
	{
		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x0600273A RID: 10042 RVA: 0x000A7398 File Offset: 0x000A5598
		// (set) Token: 0x0600273B RID: 10043 RVA: 0x000A73A0 File Offset: 0x000A55A0
		public CharacterObject Troop { get; set; }

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x0600273C RID: 10044 RVA: 0x000A73A9 File Offset: 0x000A55A9
		// (set) Token: 0x0600273D RID: 10045 RVA: 0x000A73B1 File Offset: 0x000A55B1
		public bool IsPrisoner { get; set; }

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x0600273E RID: 10046 RVA: 0x000A73BA File Offset: 0x000A55BA
		// (set) Token: 0x0600273F RID: 10047 RVA: 0x000A73C2 File Offset: 0x000A55C2
		public int FromCount { get; set; }

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x06002740 RID: 10048 RVA: 0x000A73CB File Offset: 0x000A55CB
		// (set) Token: 0x06002741 RID: 10049 RVA: 0x000A73D3 File Offset: 0x000A55D3
		public int ToCount { get; set; }

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x06002742 RID: 10050 RVA: 0x000A73DC File Offset: 0x000A55DC
		public int DifferenceCount
		{
			get
			{
				return this.FromCount - this.ToCount;
			}
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06002743 RID: 10051 RVA: 0x000A73EB File Offset: 0x000A55EB
		// (set) Token: 0x06002744 RID: 10052 RVA: 0x000A73F3 File Offset: 0x000A55F3
		public bool IsEmpty { get; private set; }

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x06002745 RID: 10053 RVA: 0x000A73FC File Offset: 0x000A55FC
		public static TroopTradeDifference Empty
		{
			get
			{
				return new TroopTradeDifference
				{
					IsEmpty = true
				};
			}
		}
	}
}
