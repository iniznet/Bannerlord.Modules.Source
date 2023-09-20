using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement
{
	// Token: 0x02000135 RID: 309
	public class PartyAddedToArmyByPlayerEvent : EventBase
	{
		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06001E2A RID: 7722 RVA: 0x0006B860 File Offset: 0x00069A60
		// (set) Token: 0x06001E2B RID: 7723 RVA: 0x0006B868 File Offset: 0x00069A68
		public MobileParty AddedParty { get; private set; }

		// Token: 0x06001E2C RID: 7724 RVA: 0x0006B871 File Offset: 0x00069A71
		public PartyAddedToArmyByPlayerEvent(MobileParty addedParty)
		{
			this.AddedParty = addedParty;
		}
	}
}
