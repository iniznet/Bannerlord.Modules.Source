using System;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	// Token: 0x0200003F RID: 63
	public class StoryModeEncounterGameMenuModel : DefaultEncounterGameMenuModel
	{
		// Token: 0x060003B6 RID: 950 RVA: 0x000172B8 File Offset: 0x000154B8
		public override string GetEncounterMenu(PartyBase attackerParty, PartyBase defenderParty, out bool startBattle, out bool joinBattle)
		{
			Settlement settlement = base.GetEncounteredPartyBase(attackerParty, defenderParty).Settlement;
			string text;
			if (settlement != null && settlement.SettlementComponent is TrainingField)
			{
				text = "training_field_menu";
				startBattle = false;
				joinBattle = false;
			}
			else if (StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
			{
				text = "storymode_game_menu_blocker";
				startBattle = false;
				joinBattle = false;
			}
			else
			{
				text = base.GetEncounterMenu(attackerParty, defenderParty, ref startBattle, ref joinBattle);
			}
			return text;
		}
	}
}
