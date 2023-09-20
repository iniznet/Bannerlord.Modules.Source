﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x020000A5 RID: 165
	public class WorkshopsCharactersCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060009CF RID: 2511 RVA: 0x00050E44 File Offset: 0x0004F044
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00050E74 File Offset: 0x0004F074
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x00050E76 File Offset: 0x0004F076
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddShopWorkerDialogs(campaignGameStarter);
			this.AddWorkshopOwnerDialogs(campaignGameStarter);
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00050E88 File Offset: 0x0004F088
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
			if (CampaignMission.Current.Location == locationWithId && CampaignTime.Now.IsDayTime)
			{
				this.AddShopWorkersToTownCenter(unusedUsablePointCount);
			}
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x00050ED0 File Offset: 0x0004F0D0
		private void AddShopWorkersToTownCenter(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			CharacterObject shopWorker = settlement.Culture.ShopWorker;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(shopWorker.Race, "_settlement");
			Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(shopWorker, ref num, ref num2, "");
			foreach (Workshop workshop in settlement.Town.Workshops)
			{
				if (workshop.IsRunning)
				{
					int num3;
					unusedUsablePointCount.TryGetValue(workshop.Tag, out num3);
					float num4 = (float)num3 * 0.33f;
					if (num4 > 0f)
					{
						int num5 = 0;
						while ((float)num5 < num4)
						{
							LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(shopWorker, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), workshop.Tag, true, 0, null, true, false, null, false, false, true);
							locationWithId.AddCharacter(locationCharacter);
							num5++;
						}
					}
				}
			}
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x00051004 File Offset: 0x0004F204
		private Workshop FindCurrentWorkshop(Agent agent)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown)
			{
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				AgentNavigator agentNavigator = ((component != null) ? component.AgentNavigator : null);
				if (agentNavigator != null)
				{
					foreach (Workshop workshop in Settlement.CurrentSettlement.Town.Workshops)
					{
						if (workshop.Tag == agentNavigator.SpecialTargetTag)
						{
							return workshop;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x00051074 File Offset: 0x0004F274
		private void AddWorkshopOwnerDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddPlayerLine("workshop_notable_owner_begin_1", "hero_main_options", "workshop_owner_notable_single_response", "{=ug5E8FCZ}I wish to buy your {.%}{WORKSHOP_NAME}{.%}.", new ConversationSentence.OnConditionDelegate(this.workshop_notable_owner_begin_single_on_condition), new ConversationSentence.OnConsequenceDelegate(this.workshop_notable_owner_begin_single_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.player_war_status_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("workshop_notable_owner_begin_2", "hero_main_options", "workshop_owner_notable_multiple_response", "{=LuLttpc5}I wish to buy one of your workshops.", new ConversationSentence.OnConditionDelegate(this.workshop_notable_owner_begin_multiple_on_condition), new ConversationSentence.OnConsequenceDelegate(this.workshop_notable_owner_answer_list_workshops_on_condition), 100, new ConversationSentence.OnClickableConditionDelegate(this.player_war_status_clickable_condition), null);
			campaignGameStarter.AddDialogLine("workshop_notable_owner_answer_1_single", "workshop_owner_notable_single_response", "workshop_notable_player_buy_options", "{=IdvcaDMu}I'm willing to sell. But it will cost you {COST} {GOLD_ICON}. Are you willing to pay?", new ConversationSentence.OnConditionDelegate(this.workshop_notable_owner_answer_single_workshop_cost_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_notable_player_buy_positive", "workshop_notable_player_buy_options", "workshop_notable_player_buy_positive_end", "{=kB65SzbF}Yes.", null, new ConversationSentence.OnConsequenceDelegate(this.workshop_notable_owner_player_buys_workshop_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.workshop_notable_owner_player_buys_workshop_on_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("workshop_notable_player_buy_negative", "workshop_notable_player_buy_options", "workshop_notable_player_buy_negative_end", "{=znDzVxVJ}No.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_notable_player_buy_negative_end", "workshop_notable_player_buy_negative_end", "hero_main_options", "{=Hj25CLlZ}As you wish. Let me know if you change your mind.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_notable_player_buy_positive_end", "workshop_notable_player_buy_positive_end", "hero_main_options", "{=ZtULAKGb}Well then, we have a deal. I will instruct my workers that they are now working for you. Good luck!", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_notable_owner_answer_1_multiple", "workshop_owner_notable_multiple_response", "workshop_player_select_workshop", "{=j78hz3Qc}Hmm. That's possible. Which one?", null, null, 100, null);
			campaignGameStarter.AddRepeatablePlayerLine("workshop_player_select_workshop", "workshop_player_select_workshop", "workshop_owner_notable_single_response", "{=!}{WORKSHOP_NAME}", "{=5z4hEq68}I am thinking of a different kind of workshop.", "workshop_owner_notable_multiple_response", new ConversationSentence.OnConditionDelegate(this.workshop_notable_owner_player_select_workshop_multiple_on_condition), new ConversationSentence.OnConsequenceDelegate(this.workshop_notable_owner_player_select_workshop_multiple_on_consequence), 100, null);
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00051228 File Offset: 0x0004F428
		private bool workshop_notable_owner_begin_single_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsNotable && Hero.OneToOneConversationHero.CurrentSettlement == Settlement.CurrentSettlement)
			{
				if (Hero.OneToOneConversationHero.OwnedWorkshops.Count((Workshop x) => !x.WorkshopType.IsHidden) == 1)
				{
					MBTextManager.SetTextVariable("WORKSHOP_NAME", Hero.OneToOneConversationHero.OwnedWorkshops.First((Workshop x) => !x.WorkshopType.IsHidden).Name, false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x000512D1 File Offset: 0x0004F4D1
		private void workshop_notable_owner_begin_single_on_consequence()
		{
			this._lastSelectedWorkshop = Hero.OneToOneConversationHero.OwnedWorkshops.First((Workshop x) => !x.WorkshopType.IsHidden);
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00051308 File Offset: 0x0004F508
		private bool workshop_notable_owner_begin_multiple_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsNotable && Hero.OneToOneConversationHero.CurrentSettlement == Settlement.CurrentSettlement)
			{
				return Hero.OneToOneConversationHero.OwnedWorkshops.Count((Workshop x) => !x.WorkshopType.IsHidden) > 1;
			}
			return false;
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0005136C File Offset: 0x0004F56C
		private bool workshop_notable_owner_answer_single_workshop_cost_on_condition()
		{
			if (this._lastSelectedWorkshop != null)
			{
				MBTextManager.SetTextVariable("COST", Campaign.Current.Models.WorkshopModel.GetBuyingCostForPlayer(this._lastSelectedWorkshop));
				return true;
			}
			return false;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x0005139D File Offset: 0x0004F59D
		private void workshop_notable_owner_answer_list_workshops_on_condition()
		{
			ConversationSentence.SetObjectsToRepeatOver(Hero.OneToOneConversationHero.OwnedWorkshops.Where((Workshop x) => !x.WorkshopType.IsHidden).ToList<Workshop>(), 5);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x000513D8 File Offset: 0x0004F5D8
		private bool workshop_notable_owner_player_select_workshop_multiple_on_condition()
		{
			Workshop workshop = ConversationSentence.CurrentProcessedRepeatObject as Workshop;
			if (workshop != null)
			{
				ConversationSentence.SelectedRepeatLine.SetTextVariable("WORKSHOP_NAME", workshop.Name);
				return true;
			}
			return false;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x0005140C File Offset: 0x0004F60C
		private void workshop_notable_owner_player_select_workshop_multiple_on_consequence()
		{
			this._lastSelectedWorkshop = ConversationSentence.SelectedRepeatObject as Workshop;
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00051420 File Offset: 0x0004F620
		private void workshop_notable_owner_player_buys_workshop_on_consequence()
		{
			Workshop lastSelectedWorkshop = this._lastSelectedWorkshop;
			int buyingCostForPlayer = Campaign.Current.Models.WorkshopModel.GetBuyingCostForPlayer(lastSelectedWorkshop);
			ChangeOwnerOfWorkshopAction.ApplyByTrade(lastSelectedWorkshop, Hero.MainHero, lastSelectedWorkshop.WorkshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, buyingCostForPlayer, null);
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00051473 File Offset: 0x0004F673
		private bool workshop_notable_owner_player_buys_workshop_on_clickable_condition(out TextObject explanation)
		{
			return this.can_player_buy_workshop_clickable_condition(this._lastSelectedWorkshop, out explanation);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x00051484 File Offset: 0x0004F684
		private bool can_player_buy_workshop_clickable_condition(Workshop workshop, out TextObject explanation)
		{
			bool flag = Hero.MainHero.Gold < Campaign.Current.Models.WorkshopModel.GetBuyingCostForPlayer(workshop);
			bool flag2 = Campaign.Current.Models.WorkshopModel.GetMaxWorkshopCountForTier(Clan.PlayerClan.Tier) <= Hero.MainHero.OwnedWorkshops.Count;
			bool flag3 = false;
			if (flag)
			{
				explanation = new TextObject("{=B2jpmFh6}You don't have enough money to buy this workshop.", null);
			}
			else if (flag2)
			{
				explanation = new TextObject("{=Mzs39I2G}You have reached the maximum amount of workshops you can have.", null);
			}
			else
			{
				explanation = TextObject.Empty;
				flag3 = true;
			}
			return flag3;
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00051514 File Offset: 0x0004F714
		private bool player_war_status_clickable_condition(out TextObject explanation)
		{
			if (Hero.MainHero.MapFaction.IsAtWarWith(Settlement.CurrentSettlement.MapFaction))
			{
				explanation = new TextObject("{=QkiqdcKa}You cannot own a workshop in an enemy town.", null);
				return false;
			}
			explanation = TextObject.Empty;
			return true;
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00051548 File Offset: 0x0004F748
		private void AddShopWorkerDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("workshop_npc_owner_begin", "start", "shopworker_npc_player", "{=DGKgQycl}{WORKSHOP_INTRO_LINE}", new ConversationSentence.OnConditionDelegate(this.workshop_npc_owner_begin_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_1", "start_2", "shopworker_npc_player", "{=XZgD99ol}Anything else I can do for you?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_2", "shopworker_npc_player", "player_ask_questions", "{=HbaziRMP}I have some questions.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_3", "shopworker_npc_player", "workshop_buy", "{=p3a44dQN}I would like to buy this workshop.", null, null, 100, new ConversationSentence.OnClickableConditionDelegate(this.workshop_buy_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("workshop_4", "shopworker_npc_player", "workshop_end_dialog", "{=90YOVmcG}Good day to you.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_5", "workshop_end_dialog", "close_window", "{=QwAyt4aW}Have a nice day, {?PLAYER.GENDER}madam{?}sir{\\?}.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_6", "player_ask_questions", "player_ask_questions2", "{=AbIUjOLZ}Sure. What do you want to know?", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_7", "player_ask_questions_return", "player_ask_questions2", "{=1psOym3y}Any other questions?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_8", "player_ask_questions2", "player_ask_questions3", "{=hmmoXy0E}Whose workshop is this?", new ConversationSentence.OnConditionDelegate(this.workshop_player_not_owner_on_condition), null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_9", "player_ask_questions2", "player_ask_questions4", "{=5siWBRk8}What do you produce here?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_10", "player_ask_questions2", "player_ask_questions5", "{=v0HhVu4z}How do workshops work?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_11", "player_ask_questions2", "start_2", "{=rXbL9mhQ}I want to talk about other things.", new ConversationSentence.OnConditionDelegate(this.workshop_player_not_owner_on_condition), null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_12", "player_ask_questions3", "player_ask_questions_return", "{=aE0kPqcT}This workshop belongs to {OWNER.NAME}, {?PLAYER.GENDER}madam{?}sir{\\?}.", new ConversationSentence.OnConditionDelegate(this.workshop_12_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_13", "player_ask_questions4", "player_ask_questions_return", "{=LXtebqEF}This a {.%}{WORKSHOP_TYPE}{.%}, {?PLAYER.GENDER}madam{?}sir{\\?}. {WORKSHOP_DESCRIPTION}", new ConversationSentence.OnConditionDelegate(this.workshop_13_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_14", "player_ask_questions5", "player_ask_questions_return", "{=QKsaPj6w}We take raw materials and produce goods and sell them at the local market. After paying the wages we send profits the shop owner.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_15", "workshop_buy", "workshop_buy_2", "{=bQ8CxZIy}Well, it will cost you {COST}{GOLD_ICON} to buy it out. Are you going to buy it?", new ConversationSentence.OnConditionDelegate(this.workshop_15_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_16", "workshop_buy_2", "workshop_buy_3", "{=aeouhelq}Yes", null, new ConversationSentence.OnConsequenceDelegate(this.workshop_19_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.workshop_16_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("workshop_17", "workshop_buy_2", "workshop_buy_4", "{=8OkPHu4f}No", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_18", "workshop_buy_4", "start_2", "{=OiaLnmbY}Your call.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_19", "workshop_buy_3", "workshop_buy_5", "{=UesQ8a2B}What kind of workshop do you want to open here?", null, null, 100, null);
			campaignGameStarter.AddRepeatablePlayerLine("workshop_20", "workshop_buy_5", "workshop_buy_6", "{=242TpAqL}{WORKSHOP_TYPE}", "{=5z4hEq68}I am thinking of a different kind of workshop.", "workshop_buy_3", new ConversationSentence.OnConditionDelegate(this.workshop_20_on_condition), new ConversationSentence.OnConsequenceDelegate(this.workshop_20_on_consequence), 100, null);
			campaignGameStarter.AddPlayerLine("workshop_21", "workshop_buy_5", "workshop_buy_6", "{=QHbcFrPX}On second thought, I don't want to change what we are producing. Go on like this.", null, new ConversationSentence.OnConsequenceDelegate(this.workshop_21_on_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_22", "workshop_buy_6", "workshop_buy_7", "{=QaqF7dvb}Let me just go over what we'd do there. {WORKSHOP_DESCRIPTION}. Are you sure you want to open {.%}{.a} {WORKSHOP_TYPE}{.%}?", new ConversationSentence.OnConditionDelegate(this.workshop_22_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_23", "workshop_buy_7", "close_window", "{=aeouhelq}Yes", null, new ConversationSentence.OnConsequenceDelegate(this.workshop_23_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_24", "workshop_buy_7", "workshop_buy_3", "{=EAb4hSDP}Let me think again.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_25", "workshop_buy_7", "start_2", "{=pP4sdfZc}On second though I don't want to open a workshop now.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_player_owner_begin", "start", "player_options", "{=VO8qBWrv}Hey boss, come to check on the business?", new ConversationSentence.OnConditionDelegate(this.workshop_player_owner_begin_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_26", "shopkeeper_start", "player_options", "{=XZgD99ol}Anything else I can do for you?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_27", "player_options", "player_ask_questions", "{=uotflvH2}I would like to ask some questions.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_28", "player_options", "player_change_production", "{=b92969l9}I would like to change what you are producing here.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_29", "player_options", "player_sell_workshop", "{=z3BeN9ro}I would like to sell this workshop.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_30", "player_options", "player_workshop_end_dialog", "{=90YOVmcG}Good day to you.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_31", "player_workshop_end_dialog", "close_window", "{=NyxcGLxf}At your service.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_32", "player_workshop_questions", "player_workshop_questions_start", "{=Y4LhmAdi}Sure, boss. Go ahead.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_33", "player_workshop_questions_return", "player_workshop_questions_start", "{=1psOym3y}Any other questions?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_34", "player_ask_questions2", "shopkeeper_start", "{=rXbL9mhQ}I want to talk about other things.", new ConversationSentence.OnConditionDelegate(this.workshop_player_is_owner_on_condition), null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_35", "player_sell_workshop", "player_sell_price", "{=XQa48r6e}Really, boss, if I sell everything, and with all the money we have on hand, you can get {PRICE}{GOLD_ICON}. You sure you want to sell?", new ConversationSentence.OnConditionDelegate(this.conversation_shopworker_sell_player_workshop_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_36", "player_sell_workshop", "shopkeeper_start", "{=nE4Ud7Ca}Really, boss, if I sell everything, and with all the money we have on hand, you can get {PRICE}{GOLD_ICON}. But I don't know anybody who can afford this right now.", new ConversationSentence.OnConditionDelegate(this.conversation_shopworker_sell_player_workshop_no_buyer_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_37", "player_sell_price", "player_sell_1", "{=tc4DyxaL}Yes, I would like to sell.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_shopworker_player_sell_workshop_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_38", "player_sell_price", "player_sell_2", "{=bi7ZhwNO}No, we are doing fine.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_39", "player_sell_1", "close_window", "{=EPb2BDuo}We had a good run, boss. Maybe we can work again some time. It will take sometime to pack things up and everything.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_40", "player_sell_2", "shopkeeper_start", "{=DBTOhVl0}That's the right call, boss. You should never let go of a good investment.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("workshop_41", "player_change_production", "player_change_production_1", "{=9eLyeU7M}Sure, boss. It will cost us {COST}{GOLD_ICON}. Want to go ahead?", new ConversationSentence.OnConditionDelegate(this.conversation_player_workshop_change_production_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_42", "player_change_production_1", "player_change_production_2", "{=kB65SzbF}Yes.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_workshop_change_production_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.workshop_42_on_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("workshop_43", "player_change_production_1", "shopkeeper_start", "{=8OkPHu4f}No", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_44", "player_change_production_2", "player_change_production_2_1", "{=WAa4yaTo}What do you want us to build?", null, null, 100, null);
			campaignGameStarter.AddRepeatablePlayerLine("workshop_45", "player_change_production_2_1", "player_change_production_3", "{=!}{BUILDING}", "{=5z4hEq68}I am thinking of a different kind of workshop.", "player_change_production_2", new ConversationSentence.OnConditionDelegate(this.conversation_player_workshop_player_decision_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_workshop_player_decision_on_consequence), 100, null);
			campaignGameStarter.AddPlayerLine("workshop_cancel", "player_change_production_2_1", "shopkeeper_start", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("workshop_46", "player_change_production_3", "player_change_production_5", "{=8d7y1IHp}Let me just go over what we'd do there. {WORKSHOP_DESCRIPTION} Are you sure you want to change this workshop to {.%}{.a} {WORKSHOP_TYPE}{.%}?", new ConversationSentence.OnConditionDelegate(this.workshop_46_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("workshop_47", "player_change_production_5", "close_window", "{=Imes09et}I'll deposit the money. You arrange some tools and workers and let's get it built.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_workshop_player_changed_production_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_48", "player_change_production_5", "player_change_production_2", "{=EAb4hSDP}Let me think again.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("workshop_49", "player_change_production_5", "shopkeeper_start", "{=QHbcFrPX}On second thought, I don't want to change what we are producing. Go on like this.", null, null, 100, null, null);
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00051D24 File Offset: 0x0004FF24
		private bool workshop_npc_owner_begin_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.Occupation == 25)
			{
				Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
				if (workshop != null && workshop.Owner != Hero.MainHero && workshop.WorkshopType != null)
				{
					if (workshop.WorkshopType.StringId == "smithy" || workshop.WorkshopType.StringId == "silversmithy")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=KTgOXBPN}Hot around here, eh {?PLAYER.GENDER}madame{?}sir{\\?}? I need to get back to the forges in a minute, but how can I help you?", false);
					}
					else if (workshop.WorkshopType.StringId == "tannery")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=4pkfkSbe}Sorry about the smell around here, {?PLAYER.GENDER}madame{?}sir{\\?}. Tanning's like that, sorry to say. How can I help you?", false);
					}
					else if (workshop.WorkshopType.StringId == "pottery_shop")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=lUjCslGK}Hope you don't mind the smoke around here, {?PLAYER.GENDER}madame{?}sir{\\?}. Got to keep the kilns hot. How can I help you?", false);
					}
					else if (workshop.WorkshopType.StringId == "brewery")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=keLJvk37}I just have a minute, {?PLAYER.GENDER}madame{?}sir{\\?}. I need to check on a batch that's brewing. How can I help you?", false);
					}
					else if (workshop.WorkshopType.StringId == "wool_weavery" || workshop.WorkshopType.StringId == "linen_weavery" || workshop.WorkshopType.StringId == "velvet_weavery")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=1xyj3HW0}I just have a minute here, {?PLAYER.GENDER}madame{?}sir{\\?}. I need to get back to the loom. How can I help you?", false);
					}
					else if (workshop.WorkshopType.StringId == "olive_press" || workshop.WorkshopType.StringId == "wine_press")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=PRiOVcO9}Careful around here, {?PLAYER.GENDER}madame{?}sir{\\?}. Don't want to get your hands caught in one of the presses. How can I help you?", false);
					}
					else if (workshop.WorkshopType.StringId == "wood_WorkshopType")
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=kMdBpDu1}Hope you don't mind all the sawdust around here, {?PLAYER.GENDER}madame{?}sir{\\?}. How can I help you?", false);
					}
					else
					{
						MBTextManager.SetTextVariable("WORKSHOP_INTRO_LINE", "{=TnZ9Mynm}Right... So, {?PLAYER.GENDER}madame{?}sir{\\?}, how can I help you?", false);
					}
					MBTextManager.SetTextVariable("WORKSHOP_TYPE", workshop.WorkshopType.Name, false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00051F1F File Offset: 0x0005011F
		private bool workshop_buy_clickable_condition(out TextObject explanation)
		{
			return this.player_war_status_clickable_condition(out explanation);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00051F28 File Offset: 0x00050128
		private bool workshop_12_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			StringHelpers.SetCharacterProperties("OWNER", workshop.Owner.CharacterObject, null, false);
			return true;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x00051F5C File Offset: 0x0005015C
		private bool workshop_13_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			MBTextManager.SetTextVariable("WORKSHOP_TYPE", workshop.WorkshopType.Name, false);
			MBTextManager.SetTextVariable("WORKSHOP_DESCRIPTION", workshop.WorkshopType.Description, false);
			return true;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00051FA4 File Offset: 0x000501A4
		private bool workshop_15_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			int buyingCostForPlayer = Campaign.Current.Models.WorkshopModel.GetBuyingCostForPlayer(workshop);
			MBTextManager.SetTextVariable("COST", buyingCostForPlayer);
			return true;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00051FE0 File Offset: 0x000501E0
		private bool workshop_16_clickable_condition(out TextObject explanation)
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			return this.can_player_buy_workshop_clickable_condition(workshop, out explanation);
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x00052004 File Offset: 0x00050204
		private void workshop_19_on_consequence()
		{
			Workshop currentWorkshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			ConversationSentence.SetObjectsToRepeatOver(WorkshopType.All.Where((WorkshopType x) => x != currentWorkshop.WorkshopType && !x.IsHidden).ToList<WorkshopType>(), 5);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0005204C File Offset: 0x0005024C
		private bool workshop_20_on_condition()
		{
			WorkshopType workshopType = ConversationSentence.CurrentProcessedRepeatObject as WorkshopType;
			if (workshopType != null)
			{
				ConversationSentence.SelectedRepeatLine.SetTextVariable("WORKSHOP_TYPE", workshopType.Name);
				return true;
			}
			return false;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00052080 File Offset: 0x00050280
		private void workshop_20_on_consequence()
		{
			this._lastSelectedWorkshopType = ConversationSentence.SelectedRepeatObject as WorkshopType;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00052094 File Offset: 0x00050294
		private void workshop_21_on_consequence()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			this._lastSelectedWorkshopType = workshop.WorkshopType;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x000520B9 File Offset: 0x000502B9
		private bool workshop_22_on_condition()
		{
			MBTextManager.SetTextVariable("WORKSHOP_DESCRIPTION", this._lastSelectedWorkshopType.Description, false);
			MBTextManager.SetTextVariable("WORKSHOP_TYPE", this._lastSelectedWorkshopType.Name, false);
			return true;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x000520E8 File Offset: 0x000502E8
		private void workshop_23_on_consequence()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			int buyingCostForPlayer = Campaign.Current.Models.WorkshopModel.GetBuyingCostForPlayer(workshop);
			ChangeOwnerOfWorkshopAction.ApplyByTrade(workshop, Hero.MainHero, this._lastSelectedWorkshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, buyingCostForPlayer, null);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00052140 File Offset: 0x00050340
		private bool workshop_player_owner_begin_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.Occupation == 25)
			{
				Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
				return workshop != null && workshop.Owner == Hero.MainHero;
			}
			return false;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0005217C File Offset: 0x0005037C
		private bool workshop_player_not_owner_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			return workshop != null && workshop.Owner != Hero.MainHero;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x000521AC File Offset: 0x000503AC
		private bool workshop_player_is_owner_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			return workshop != null && workshop.Owner == Hero.MainHero;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x000521D7 File Offset: 0x000503D7
		private bool workshop_42_on_clickable_condition(out TextObject explanation)
		{
			if (Hero.MainHero.Gold < Campaign.Current.Models.WorkshopModel.GetConvertProductionCost(null))
			{
				explanation = new TextObject("{=EASiM8NU}You haven't got enough denars to change production.", null);
				return false;
			}
			explanation = TextObject.Empty;
			return true;
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00052211 File Offset: 0x00050411
		private bool workshop_46_on_condition()
		{
			MBTextManager.SetTextVariable("WORKSHOP_DESCRIPTION", this._lastSelectedWorkshopType.Description, false);
			MBTextManager.SetTextVariable("WORKSHOP_TYPE", this._lastSelectedWorkshopType.Name, false);
			return true;
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x00052240 File Offset: 0x00050440
		private bool conversation_shopworker_sell_player_workshop_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(workshop);
			MBTextManager.SetTextVariable("PRICE", sellingCost);
			return !this.conversation_shopworker_sell_player_workshop_no_buyer_on_condition();
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00052284 File Offset: 0x00050484
		private bool conversation_shopworker_sell_player_workshop_no_buyer_on_condition()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(workshop);
			return Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(Settlement.CurrentSettlement.Town, workshop, Hero.MainHero, sellingCost) == null;
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x000522DC File Offset: 0x000504DC
		private void conversation_shopworker_player_sell_workshop_on_consequence()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(workshop);
			Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(Settlement.CurrentSettlement.Town, workshop, workshop.Owner, sellingCost);
			ChangeOwnerOfWorkshopAction.ApplyByTrade(workshop, hero, workshop.WorkshopType, Campaign.Current.Models.WorkshopModel.GetInitialCapital(1), true, sellingCost, null);
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x00052358 File Offset: 0x00050558
		private bool conversation_player_workshop_change_production_on_condition()
		{
			int convertProductionCost = Campaign.Current.Models.WorkshopModel.GetConvertProductionCost(null);
			MBTextManager.SetTextVariable("COST", convertProductionCost);
			return true;
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00052388 File Offset: 0x00050588
		private void conversation_player_workshop_change_production_on_consequence()
		{
			Workshop currentWorkshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			ConversationSentence.SetObjectsToRepeatOver(WorkshopType.All.Where((WorkshopType x) => x != currentWorkshop.WorkshopType && !x.IsHidden).ToList<WorkshopType>(), 5);
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x000523D0 File Offset: 0x000505D0
		private bool conversation_player_workshop_player_decision_on_condition()
		{
			WorkshopType workshopType = ConversationSentence.CurrentProcessedRepeatObject as WorkshopType;
			if (workshopType != null)
			{
				ConversationSentence.SelectedRepeatLine.SetTextVariable("BUILDING", workshopType.Name);
				return true;
			}
			return false;
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x00052404 File Offset: 0x00050604
		private void conversation_player_workshop_player_decision_on_consequence()
		{
			this._lastSelectedWorkshopType = ConversationSentence.SelectedRepeatObject as WorkshopType;
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00052418 File Offset: 0x00050618
		private void conversation_player_workshop_player_changed_production_on_consequence()
		{
			Workshop workshop = this.FindCurrentWorkshop(ConversationMission.OneToOneConversationAgent);
			if (workshop.WorkshopType != this._lastSelectedWorkshopType)
			{
				ChangeProductionTypeOfWorkshopAction.Apply(workshop, this._lastSelectedWorkshopType, false);
			}
		}

		// Token: 0x0400035F RID: 863
		public const float WorkerSpawnPercentage = 0.33f;

		// Token: 0x04000360 RID: 864
		private WorkshopType _lastSelectedWorkshopType;

		// Token: 0x04000361 RID: 865
		private Workshop _lastSelectedWorkshop;
	}
}
