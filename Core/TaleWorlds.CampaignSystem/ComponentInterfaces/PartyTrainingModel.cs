using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000175 RID: 373
	public abstract class PartyTrainingModel : GameModel
	{
		// Token: 0x0600190D RID: 6413
		public abstract int GenerateSharedXp(CharacterObject troop, int xp, MobileParty mobileParty);

		// Token: 0x0600190E RID: 6414
		public abstract int CalculateXpGainFromBattles(FlattenedTroopRosterElement troopRosterElement, PartyBase party);

		// Token: 0x0600190F RID: 6415
		public abstract int GetXpReward(CharacterObject character);

		// Token: 0x06001910 RID: 6416
		public abstract ExplainedNumber GetEffectiveDailyExperience(MobileParty party, TroopRosterElement troop);
	}
}
