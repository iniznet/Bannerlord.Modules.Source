using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C3 RID: 451
	public abstract class PartyTroopUpgradeModel : GameModel
	{
		// Token: 0x06001B43 RID: 6979
		public abstract bool CanPartyUpgradeTroopToTarget(PartyBase party, CharacterObject character, CharacterObject target);

		// Token: 0x06001B44 RID: 6980
		public abstract bool IsTroopUpgradeable(PartyBase party, CharacterObject character);

		// Token: 0x06001B45 RID: 6981
		public abstract bool DoesPartyHaveRequiredItemsForUpgrade(PartyBase party, CharacterObject upgradeTarget);

		// Token: 0x06001B46 RID: 6982
		public abstract bool DoesPartyHaveRequiredPerksForUpgrade(PartyBase party, CharacterObject character, CharacterObject upgradeTarget, out PerkObject requiredPerk);

		// Token: 0x06001B47 RID: 6983
		public abstract bool CanTroopGainXp(PartyBase owner, CharacterObject character);

		// Token: 0x06001B48 RID: 6984
		public abstract int GetGoldCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget);

		// Token: 0x06001B49 RID: 6985
		public abstract int GetXpCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget);

		// Token: 0x06001B4A RID: 6986
		public abstract int GetSkillXpFromUpgradingTroops(PartyBase party, CharacterObject troop, int numberOfTroops);

		// Token: 0x06001B4B RID: 6987
		public abstract float GetUpgradeChanceForTroopUpgrade(PartyBase party, CharacterObject troop, int upgradeTargetIndex);
	}
}
