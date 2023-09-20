using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x02000029 RID: 41
	public class PlayerRequestUpgradeTroopEvent : EventBase
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x00017E0F File Offset: 0x0001600F
		// (set) Token: 0x0600043C RID: 1084 RVA: 0x00017E17 File Offset: 0x00016017
		public CharacterObject SourceTroop { get; private set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x00017E20 File Offset: 0x00016020
		// (set) Token: 0x0600043E RID: 1086 RVA: 0x00017E28 File Offset: 0x00016028
		public CharacterObject TargetTroop { get; private set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x00017E31 File Offset: 0x00016031
		// (set) Token: 0x06000440 RID: 1088 RVA: 0x00017E39 File Offset: 0x00016039
		public int Number { get; private set; }

		// Token: 0x06000441 RID: 1089 RVA: 0x00017E42 File Offset: 0x00016042
		public PlayerRequestUpgradeTroopEvent(CharacterObject sourceTroop, CharacterObject targetTroop, int num)
		{
			this.SourceTroop = sourceTroop;
			this.TargetTroop = targetTroop;
			this.Number = num;
		}
	}
}
