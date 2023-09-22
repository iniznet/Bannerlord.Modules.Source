using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	public class TradersCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		protected void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("weaponsmith_talk_start_normal", "start", "weaponsmith_talk_player", "{=7IxFrati}Greetings my {?PLAYER.GENDER}lady{?}lord{\\?}, how may I help you?", new ConversationSentence.OnConditionDelegate(this.conversation_weaponsmith_talk_start_normal_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_start_to_player_in_disguise", "start", "close_window", "{=1auLEn9y}Look, my good {?PLAYER.GENDER}woman{?}man{\\?}, these are hard times for sure, but I need you to move along. You'll scare away my customers.", new ConversationSentence.OnConditionDelegate(this.conversation_weaponsmith_talk_start_to_player_in_disguise_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_initial", "weaponsmith_begin", "weaponsmith_talk_player", "{=jxw54Ijt}Okay, is there anything more I can help with?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("weaponsmith_talk_player_1", "weaponsmith_talk_player", "merchant_response_1", "{=ExltvaKo}Let me see what you have for sale...", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("weaponsmith_talk_player_request_craft", "weaponsmith_talk_player", "merchant_response_crafting", "{=w1vzpCNi}I need you to craft a weapon for me", new ConversationSentence.OnConditionDelegate(this.conversation_open_crafting_on_condition), null, 100, null, null);
			campaignGameStarter.AddPlayerLine("weaponsmith_talk_player_3", "weaponsmith_talk_player", "merchant_response_3", "{=8hNYr2VX}I was just passing by.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_merchant_response_1", "merchant_response_1", "player_merchant_talk_close", "{=K5mG9nDv}With pleasure.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_merchant_response_2", "merchant_response_2", "player_merchant_talk_2", "{=5bRQ0gt7}How many men do you need for it? For each men I want 100{GOLD_ICON}.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_merchant_response_craft", "merchant_response_crafting", "player_merchant_craft_talk_close", "{=lF5HkBDy}As you wish.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_merchant_craft_opened", "player_merchant_craft_talk_close", "close_window", "{=TD8Jxn7U}Have a nice day my {?PLAYER.GENDER}lady{?}lord{\\?}.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_weaponsmith_craft_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_merchant_response_3", "merchant_response_3", "close_window", "{=FpNWdIaT}Yes, of course. Just ask me if there is anything you need.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("weaponsmith_talk_end", "player_merchant_talk_close", "close_window", "{=Yh0danUf}Thank you and good day my {?PLAYER.GENDER}lady{?}lord{\\?}.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_weaponsmith_talk_player_on_consequence), 100, null);
		}

		private bool conversation_open_crafting_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter != null && CharacterObject.OneToOneConversationCharacter.Occupation == 28;
		}

		private bool conversation_weaponsmith_talk_start_normal_on_condition()
		{
			return !Campaign.Current.IsMainHeroDisguised && this.IsTrader();
		}

		private bool conversation_weaponsmith_talk_start_to_player_in_disguise_on_condition()
		{
			return Campaign.Current.IsMainHeroDisguised && this.IsTrader();
		}

		private bool IsTrader()
		{
			return CharacterObject.OneToOneConversationCharacter.Occupation == 10 || CharacterObject.OneToOneConversationCharacter.Occupation == 11 || CharacterObject.OneToOneConversationCharacter.Occupation == 12 || CharacterObject.OneToOneConversationCharacter.Occupation == 4 || CharacterObject.OneToOneConversationCharacter.Occupation == 28;
		}

		private void conversation_weaponsmith_talk_player_on_consequence()
		{
			InventoryManager.InventoryCategoryType inventoryCategoryType = -1;
			Occupation occupation = CharacterObject.OneToOneConversationCharacter.Occupation;
			if (occupation != 4)
			{
				switch (occupation)
				{
				case 10:
					inventoryCategoryType = 2;
					break;
				case 11:
					inventoryCategoryType = 1;
					break;
				case 12:
					inventoryCategoryType = 4;
					break;
				default:
					if (occupation == 28)
					{
						inventoryCategoryType = 2;
					}
					break;
				}
			}
			else
			{
				inventoryCategoryType = 5;
			}
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (Mission.Current != null)
			{
				InventoryManager.OpenScreenAsTrade(currentSettlement.ItemRoster, currentSettlement.Town, inventoryCategoryType, new InventoryManager.DoneLogicExtrasDelegate(this.OnInventoryScreenDone));
				return;
			}
			InventoryManager.OpenScreenAsTrade(currentSettlement.ItemRoster, currentSettlement.Town, inventoryCategoryType, null);
		}

		private void conversation_weaponsmith_craft_on_consequence()
		{
			CraftingHelper.OpenCrafting(CraftingTemplate.All[0], null);
		}

		private void OnInventoryScreenDone()
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				CharacterObject characterObject = (CharacterObject)agent.Character;
				if (agent.IsHuman && characterObject != null && characterObject.IsHero && characterObject.HeroObject.PartyBelongedTo == MobileParty.MainParty)
				{
					agent.UpdateSpawnEquipmentAndRefreshVisuals(Mission.Current.DoesMissionRequireCivilianEquipment ? characterObject.FirstCivilianEquipment : characterObject.FirstBattleEquipment);
				}
			}
		}
	}
}
