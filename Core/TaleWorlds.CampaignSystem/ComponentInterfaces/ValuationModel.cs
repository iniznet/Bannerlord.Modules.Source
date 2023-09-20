using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000195 RID: 405
	public abstract class ValuationModel : GameModel
	{
		// Token: 0x06001A16 RID: 6678
		public abstract float GetValueOfTroop(CharacterObject troop);

		// Token: 0x06001A17 RID: 6679
		public abstract float GetMilitaryValueOfParty(MobileParty party);

		// Token: 0x06001A18 RID: 6680
		public abstract float GetValueOfHero(Hero hero);
	}
}
