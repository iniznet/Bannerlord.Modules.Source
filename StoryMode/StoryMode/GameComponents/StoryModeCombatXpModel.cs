using System;
using StoryMode.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	// Token: 0x0200003D RID: 61
	public class StoryModeCombatXpModel : DefaultCombatXpModel
	{
		// Token: 0x060003B2 RID: 946 RVA: 0x00017230 File Offset: 0x00015430
		public override void GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase party, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTrainingField())
			{
				xpAmount = 0;
				return;
			}
			base.GetXpFromHit(attackerTroop, captain, attackedTroop, party, damage, isFatal, missionType, ref xpAmount);
		}
	}
}
