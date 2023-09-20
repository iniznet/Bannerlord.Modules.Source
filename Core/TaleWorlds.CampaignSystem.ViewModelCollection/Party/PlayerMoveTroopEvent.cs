using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x0200002B RID: 43
	public class PlayerMoveTroopEvent : EventBase
	{
		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00017E7F File Offset: 0x0001607F
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x00017E87 File Offset: 0x00016087
		public CharacterObject Troop { get; private set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x00017E90 File Offset: 0x00016090
		// (set) Token: 0x06000448 RID: 1096 RVA: 0x00017E98 File Offset: 0x00016098
		public int Amount { get; private set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x00017EA1 File Offset: 0x000160A1
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x00017EA9 File Offset: 0x000160A9
		public bool IsPrisoner { get; private set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x00017EB2 File Offset: 0x000160B2
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x00017EBA File Offset: 0x000160BA
		public PartyScreenLogic.PartyRosterSide FromSide { get; private set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x00017EC3 File Offset: 0x000160C3
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x00017ECB File Offset: 0x000160CB
		public PartyScreenLogic.PartyRosterSide ToSide { get; private set; }

		// Token: 0x0600044F RID: 1103 RVA: 0x00017ED4 File Offset: 0x000160D4
		public PlayerMoveTroopEvent(CharacterObject troop, PartyScreenLogic.PartyRosterSide fromSide, PartyScreenLogic.PartyRosterSide toSide, int amount, bool isPrisoner)
		{
			this.Troop = troop;
			this.FromSide = fromSide;
			this.ToSide = toSide;
			this.IsPrisoner = isPrisoner;
			this.Amount = amount;
		}
	}
}
