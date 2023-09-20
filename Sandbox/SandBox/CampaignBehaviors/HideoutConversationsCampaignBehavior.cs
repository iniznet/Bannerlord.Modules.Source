using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x0200009F RID: 159
	public class HideoutConversationsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000812 RID: 2066 RVA: 0x000405CC File Offset: 0x0003E7CC
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x000405E5 File Offset: 0x0003E7E5
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x000405E7 File Offset: 0x0003E7E7
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x000405F0 File Offset: 0x0003E7F0
		private void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("bandit_hideout_start_defender", "start", "bandit_hideout_defender", "{=nYCXzAYH}You! You've cut quite a swathe through my men there, damn you. How about we settle this, one-on-one?", new ConversationSentence.OnConditionDelegate(this.bandit_hideout_start_defender_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("bandit_hideout_start_defender_1", "bandit_hideout_defender", "close_window", "{=dzXaXKaC}Very well.", null, new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_start_duel_fight_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("bandit_hideout_start_defender_2", "bandit_hideout_defender", "close_window", "{=ukRZd2AA}I don't fight duels with brigands.", null, new ConversationSentence.OnConsequenceDelegate(this.bandit_hideout_continue_battle_on_consequence), 100, null, null);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00040680 File Offset: 0x0003E880
		private bool bandit_hideout_start_defender_on_condition()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			return encounteredParty != null && !encounteredParty.IsMobile && encounteredParty.MapFaction.IsBanditFaction && (encounteredParty.MapFaction.IsBanditFaction && encounteredParty.IsSettlement && encounteredParty.Settlement.IsHideout && Mission.Current != null) && Mission.Current.GetMissionBehavior<HideoutMissionController>() != null;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x000406E5 File Offset: 0x0003E8E5
		private void bandit_hideout_start_duel_fight_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightDuelMode;
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x00040702 File Offset: 0x0003E902
		private void bandit_hideout_continue_battle_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += HideoutMissionController.StartBossFightBattleMode;
		}
	}
}
