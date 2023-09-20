using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003E1 RID: 993
	public class VillageHostileActionCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003C1B RID: 15387 RVA: 0x0011CE09 File Offset: 0x0011B009
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, CampaignTime>>("_villageLastHostileActionTimeDictionary", ref this._villageLastHostileActionTimeDictionary);
		}

		// Token: 0x06003C1C RID: 15388 RVA: 0x0011CE1D File Offset: 0x0011B01D
		public override void RegisterEvents()
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			CampaignEvents.ItemsLooted.AddNonSerializedListener(this, new Action<MobileParty, ItemRoster>(this.OnItemsLooted));
		}

		// Token: 0x06003C1D RID: 15389 RVA: 0x0011CE4D File Offset: 0x0011B04D
		private void OnItemsLooted(MobileParty mobileParty, ItemRoster lootedItems)
		{
			SkillLevelingManager.OnRaid(mobileParty, lootedItems);
		}

		// Token: 0x06003C1E RID: 15390 RVA: 0x0011CE56 File Offset: 0x0011B056
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
		{
			this.AddGameMenus(campaignGameSystemStarter);
		}

		// Token: 0x06003C1F RID: 15391 RVA: 0x0011CE60 File Offset: 0x0011B060
		public void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("village", "hostile_action", "{=GM3tAYMr}Take a hostile action", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.game_menu_village_hostile_action_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.game_menu_village_hostile_action_on_consequence), false, 1, false, null);
			campaignGameSystemStarter.AddGameMenu("village_hostile_action", "{=YVNZaVCA}What action do you have in mind?", new OnInitDelegate(VillageHostileActionCampaignBehavior.game_menu_village_hostile_menu_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("village_hostile_action", "raid_village", "{=CTi0ml5F}Raid the village", new GameMenuOption.OnConditionDelegate(this.game_menu_village_hostile_action_raid_village_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_hostile_action_raid_village_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village_hostile_action", "force_peasants_to_give_volunteers", "{=RL8z99Dt}Force notables to give you recruits", new GameMenuOption.OnConditionDelegate(this.game_menu_village_hostile_action_force_volunteers_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_hostile_action_force_volunteers_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village_hostile_action", "force_peasants_to_give_supplies", "{=eAzwpqE1}Force peasants to give you goods", new GameMenuOption.OnConditionDelegate(this.game_menu_village_hostile_action_take_food_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_hostile_action_take_food_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village_hostile_action", "forget_it", "{=sP9ohQTs}Forget it", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.back_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_hostile_action_forget_it_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddWaitGameMenu("raiding_village", "{=hWwr3mrC}You are raiding {VILLAGE_NAME}.", new OnInitDelegate(VillageHostileActionCampaignBehavior.hostile_action_village_on_init), new OnConditionDelegate(VillageHostileActionCampaignBehavior.wait_menu_start_raiding_on_condition), new OnConsequenceDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_on_consequence), new OnTickDelegate(VillageHostileActionCampaignBehavior.wait_menu_raiding_village_on_tick), GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption, GameOverlays.MenuOverlayType.None, 0f, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("raiding_village", "raiding_village_end", "{=M7CcfbIx}End Raiding", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("raiding_village", "leave_army", "{=hSdJ0UUv}Leave Army", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_at_army_by_leaving_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_at_army_by_leaving_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("raiding_village", "abandon_army", "{=0vnegjxf}Abandon Army", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_at_army_by_abandoning_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.wait_menu_end_raiding_at_army_by_abandoning_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("raid_village_no_resist_warn_player", "{=lOwjyUi5}Raiding this village will cause a war with {KINGDOM}.", new OnInitDelegate(VillageHostileActionCampaignBehavior.game_menu_warn_player_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("raid_village_no_resist_warn_player", "raid_village_warn_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.game_menu_village_hostile_action_raid_village_warn_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_hostile_action_raid_village_warn_continue_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("raid_village_no_resist_warn_player", "raid_village_warn_leave", "{=sP9ohQTs}Forget it", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.back_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_hostile_action_raid_village_warn_leave_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("force_supplies_village", "{=EqFbNha8}The villagers grudgingly bring out what they have for you.", new OnInitDelegate(VillageHostileActionCampaignBehavior.hostile_action_village_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("force_supplies_village", "force_supplies_village_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.village_force_supplies_ended_successfully_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("force_volunteers_village", "{=BqkD4YWr}You manage to round up some men from the village who look like they might make decent recruits.", new OnInitDelegate(VillageHostileActionCampaignBehavior.hostile_action_village_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("force_volunteers_village", "force_supplies_village_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.village_force_volunteers_ended_successfully_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("village_looted", "{=NxcXfUxu}The village has been looted. A handful of souls scatter as you pass through the burnt-out houses.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("village_looted", "leave", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.back_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.game_menu_settlement_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("village_player_raid_ended", "{=m1rzHfxI}{VILLAGE_ENCOUNTER_RESULT}", new OnInitDelegate(VillageHostileActionCampaignBehavior.village_player_raid_ended_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("village_player_raid_ended", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.continue_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.village_player_raid_ended_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("village_raid_ended_leaded_by_someone_else", "{=m1rzHfxI}{VILLAGE_ENCOUNTER_RESULT}", new OnInitDelegate(VillageHostileActionCampaignBehavior.village_raid_ended_leaded_by_someone_else_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("village_raid_ended_leaded_by_someone_else", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(VillageHostileActionCampaignBehavior.continue_on_condition), new GameMenuOption.OnConsequenceDelegate(VillageHostileActionCampaignBehavior.village_raid_ended_leaded_by_someone_else_on_consequence), true, -1, false, null);
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x0011D263 File Offset: 0x0011B463
		private static bool wait_menu_end_raiding_on_condition(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}
			return false;
		}

		// Token: 0x06003C21 RID: 15393 RVA: 0x0011D292 File Offset: 0x0011B492
		private static bool wait_menu_end_raiding_at_army_by_leaving_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty && MobileParty.MainParty.MapEvent == null;
		}

		// Token: 0x06003C22 RID: 15394 RVA: 0x0011D2D0 File Offset: 0x0011B4D0
		private void village_force_supplies_ended_successfully_on_consequence(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			GameMenu.SwitchToMenu("village");
			ItemRoster itemRoster = new ItemRoster();
			int num = MathF.Max((int)(Settlement.CurrentSettlement.Village.Hearth * 0.15f), 20);
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, num * Campaign.Current.Models.RaidModel.GoldRewardForEachLostHearth, false);
			for (int i = 0; i < Settlement.CurrentSettlement.Village.VillageType.Productions.Count; i++)
			{
				ValueTuple<ItemObject, float> valueTuple = Settlement.CurrentSettlement.Village.VillageType.Productions[i];
				ItemObject item = valueTuple.Item1;
				int num2 = (int)(valueTuple.Item2 / 60f * (float)num);
				if (num2 > 0)
				{
					itemRoster.AddToCounts(item, num2);
				}
			}
			if (!this._villageLastHostileActionTimeDictionary.ContainsKey(Settlement.CurrentSettlement.StringId))
			{
				this._villageLastHostileActionTimeDictionary.Add(Settlement.CurrentSettlement.StringId, CampaignTime.Now);
			}
			else
			{
				this._villageLastHostileActionTimeDictionary[Settlement.CurrentSettlement.StringId] = CampaignTime.Now;
			}
			Settlement.CurrentSettlement.SettlementHitPoints -= Settlement.CurrentSettlement.SettlementHitPoints * 0.8f;
			InventoryManager.OpenScreenAsLoot(new Dictionary<PartyBase, ItemRoster> { 
			{
				PartyBase.MainParty,
				itemRoster
			} });
			bool flag = MapEvent.PlayerMapEvent == null;
			SkillLevelingManager.OnForceSupplies(MobileParty.MainParty, itemRoster, flag);
			PlayerEncounter.Current.ForceSupplies = false;
			PlayerEncounter.Current.FinalizeBattle();
		}

		// Token: 0x06003C23 RID: 15395 RVA: 0x0011D44C File Offset: 0x0011B64C
		private void village_force_volunteers_ended_successfully_on_consequence(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			GameMenu.SwitchToMenu("village");
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			int num = (int)Math.Ceiling((double)(Settlement.CurrentSettlement.Village.Hearth / 30f));
			if (MobileParty.MainParty.HasPerk(DefaultPerks.Roguery.InBestLight, false))
			{
				num += Settlement.CurrentSettlement.Notables.Count;
			}
			troopRoster.AddToCounts(Settlement.CurrentSettlement.MapFaction.BasicTroop, num, false, 0, 0, true, -1);
			if (!this._villageLastHostileActionTimeDictionary.ContainsKey(Settlement.CurrentSettlement.StringId))
			{
				this._villageLastHostileActionTimeDictionary.Add(Settlement.CurrentSettlement.StringId, CampaignTime.Now);
			}
			else
			{
				this._villageLastHostileActionTimeDictionary[Settlement.CurrentSettlement.StringId] = CampaignTime.Now;
			}
			Settlement.CurrentSettlement.SettlementHitPoints -= Settlement.CurrentSettlement.SettlementHitPoints * 0.8f;
			Settlement.CurrentSettlement.Village.Hearth -= (float)(num / 2);
			PartyScreenManager.OpenScreenAsLoot(troopRoster, TroopRoster.CreateDummyTroopRoster(), MobileParty.MainParty.CurrentSettlement.Name, troopRoster.TotalManCount, null);
			PlayerEncounter.Current.ForceVolunteers = false;
			SkillLevelingManager.OnForceVolunteers(MobileParty.MainParty, Settlement.CurrentSettlement.Party);
			PlayerEncounter.Current.FinalizeBattle();
		}

		// Token: 0x06003C24 RID: 15396 RVA: 0x0011D5A0 File Offset: 0x0011B7A0
		private static bool game_menu_village_hostile_action_raid_village_warn_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Raid;
			return true;
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x0011D5AC File Offset: 0x0011B7AC
		private static bool wait_menu_end_raiding_at_army_by_abandoning_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			if (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty || MobileParty.MainParty.MapEvent == null)
			{
				return false;
			}
			args.Tooltip = GameTexts.FindText("str_abandon_army", null);
			args.Tooltip.SetTextVariable("INFLUENCE_COST", Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAbandoningArmy());
			return true;
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x0011D62E File Offset: 0x0011B82E
		private static void wait_menu_end_raiding_at_army_by_leaving_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Current.ForceRaid = false;
			PlayerEncounter.Finish(true);
			MobileParty.MainParty.Army = null;
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x0011D64C File Offset: 0x0011B84C
		private static void wait_menu_end_raiding_at_army_by_abandoning_on_consequence(MenuCallbackArgs args)
		{
			Clan.PlayerClan.Influence -= (float)Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAbandoningArmy();
			PlayerEncounter.Current.ForceRaid = false;
			PlayerEncounter.Finish(true);
			MobileParty.MainParty.Army = null;
		}

		// Token: 0x06003C28 RID: 15400 RVA: 0x0011D69B File Offset: 0x0011B89B
		private static void village_player_raid_ended_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.ExitToLast();
		}

		// Token: 0x06003C29 RID: 15401 RVA: 0x0011D6A4 File Offset: 0x0011B8A4
		private static void village_raid_ended_leaded_by_someone_else_on_init(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army == null)
			{
				MobileParty shortTermTargetParty = MobileParty.MainParty.ShortTermTargetParty;
				if (((shortTermTargetParty != null) ? shortTermTargetParty.Ai.AiBehaviorPartyBase : null) != null && MobileParty.MainParty.ShortTermTargetParty.Ai.AiBehaviorPartyBase.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					goto IL_8F;
				}
			}
			if (MobileParty.MainParty.Ai.AiBehaviorPartyBase == null || !MobileParty.MainParty.Ai.AiBehaviorPartyBase.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				if (MobileParty.MainParty.Army == null || (Settlement)MobileParty.MainParty.Army.AiBehaviorObject == null || ((Settlement)MobileParty.MainParty.Army.AiBehaviorObject).MapFaction == null)
				{
					MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", TextObject.Empty, false);
					return;
				}
				if (((Settlement)MobileParty.MainParty.Army.AiBehaviorObject).MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", new TextObject("{=iyJaisRb}Village is successfully raided by the army you are following.", null), false);
					return;
				}
				MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", new TextObject("{=JNGUIIQ1}Village is saved by the army you are following.", null), false);
				return;
			}
			IL_8F:
			MobileParty shortTermTargetParty2 = MobileParty.MainParty.ShortTermTargetParty;
			if ((((shortTermTargetParty2 != null) ? shortTermTargetParty2.Ai.AiBehaviorPartyBase : null) != null) ? MobileParty.MainParty.ShortTermTargetParty.Ai.AiBehaviorPartyBase.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) : MobileParty.MainParty.Ai.AiBehaviorPartyBase.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", new TextObject("{=04tBEafz}Village is successfully raided by your help.", null), false);
				return;
			}
			MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", new TextObject("{=2Ixb5OKD}Village is successfully saved by your help.", null), false);
		}

		// Token: 0x06003C2A RID: 15402 RVA: 0x0011D884 File Offset: 0x0011BA84
		private static void village_player_raid_ended_on_init(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.LastVisitedSettlement != null && MobileParty.MainParty.LastVisitedSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", "{=4YuFTXxC}You successfully raided the village.", false);
				return;
			}
			MBTextManager.SetTextVariable("VILLAGE_ENCOUNTER_RESULT", "{=aih1Y62W}You have saved the village.", false);
		}

		// Token: 0x06003C2B RID: 15403 RVA: 0x0011D8DE File Offset: 0x0011BADE
		private static void village_raid_ended_leaded_by_someone_else_on_consequence(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
			{
				GameMenu.SwitchToMenu("army_wait");
				return;
			}
			GameMenu.ExitToLast();
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x0011D914 File Offset: 0x0011BB14
		private static void game_menu_warn_player_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			MBTextManager.SetTextVariable("KINGDOM", currentSettlement.MapFaction.IsKingdomFaction ? ((Kingdom)currentSettlement.MapFaction).EncyclopediaTitle : currentSettlement.MapFaction.Name, false);
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x0011D95C File Offset: 0x0011BB5C
		private static void game_menu_village_hostile_menu_on_init(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveEncounter = false;
			if (Campaign.Current.GameMenuManager.NextLocation != null)
			{
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation, Campaign.Current.GameMenuManager.PreviousLocation, null, null);
				Campaign.Current.GameMenuManager.NextLocation = null;
				Campaign.Current.GameMenuManager.PreviousLocation = null;
				return;
			}
			if (Settlement.CurrentSettlement.SettlementHitPoints <= 0f)
			{
				GameMenu.ActivateGameMenu("RaidCompleted");
			}
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x0011D9E8 File Offset: 0x0011BBE8
		private static bool game_menu_village_hostile_action_on_condition(MenuCallbackArgs args)
		{
			Village village = Settlement.CurrentSettlement.Village;
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return village != null && Hero.MainHero.MapFaction != village.Owner.MapFaction && village.VillageState == Village.VillageStates.Normal;
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x0011DA2E File Offset: 0x0011BC2E
		private static void game_menu_village_hostile_action_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village_hostile_action");
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x0011DA3C File Offset: 0x0011BC3C
		private bool game_menu_village_hostile_action_take_food_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.ForceToGiveGoods;
			this.CheckVillageAttackableHonorably(args);
			if (this._villageLastHostileActionTimeDictionary.ContainsKey(Settlement.CurrentSettlement.StringId))
			{
				if (this._villageLastHostileActionTimeDictionary[Settlement.CurrentSettlement.StringId].ElapsedDaysUntilNow <= 10f)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=mvhyI8Hb}You have already done hostile action to this village recently.", null);
				}
				else
				{
					this._villageLastHostileActionTimeDictionary.Remove(Settlement.CurrentSettlement.StringId);
				}
			}
			return true;
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x0011DAC3 File Offset: 0x0011BCC3
		private void game_menu_village_hostile_action_forget_it_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village");
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x0011DACF File Offset: 0x0011BCCF
		private void game_menu_village_hostile_action_take_food_on_consequence(MenuCallbackArgs args)
		{
			this._lastSelectedHostileAction = VillageHostileActionCampaignBehavior.HostileAction.TakeSupplies;
			PlayerEncounter.Current.ForceSupplies = true;
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x0011DAF0 File Offset: 0x0011BCF0
		private bool game_menu_village_hostile_action_force_volunteers_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.ForceToGiveTroops;
			this.CheckVillageAttackableHonorably(args);
			CampaignTime campaignTime;
			if (this._villageLastHostileActionTimeDictionary.TryGetValue(Settlement.CurrentSettlement.StringId, out campaignTime) && campaignTime.ElapsedDaysUntilNow <= 10f)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=mvhyI8Hb}You have already done hostile action to this village recently.", null);
			}
			else if (this._villageLastHostileActionTimeDictionary.ContainsKey(Settlement.CurrentSettlement.StringId))
			{
				this._villageLastHostileActionTimeDictionary.Remove(Settlement.CurrentSettlement.StringId);
			}
			return true;
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x0011DB82 File Offset: 0x0011BD82
		private void game_menu_village_hostile_action_force_volunteers_on_consequence(MenuCallbackArgs args)
		{
			this._lastSelectedHostileAction = VillageHostileActionCampaignBehavior.HostileAction.GetVolunteers;
			PlayerEncounter.Current.ForceVolunteers = true;
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x0011DBA0 File Offset: 0x0011BDA0
		private bool game_menu_village_hostile_action_raid_village_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			this.CheckVillageAttackableHonorably(args);
			return !FactionManager.IsAlliedWithFaction(Hero.MainHero.MapFaction, Settlement.CurrentSettlement.MapFaction);
		}

		// Token: 0x06003C36 RID: 15414 RVA: 0x0011DBCC File Offset: 0x0011BDCC
		private void game_menu_village_hostile_action_raid_village_warn_continue_on_consequence(MenuCallbackArgs args)
		{
			this._lastSelectedHostileAction = VillageHostileActionCampaignBehavior.HostileAction.RaidTheVillage;
			PlayerEncounter.Current.ForceRaid = true;
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x0011DBEA File Offset: 0x0011BDEA
		private void game_menu_village_hostile_action_raid_village_warn_leave_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village_hostile_action");
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x0011DBFC File Offset: 0x0011BDFC
		private void game_menu_village_hostile_action_raid_village_on_consequence(MenuCallbackArgs args)
		{
			if (!FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Settlement.CurrentSettlement.MapFaction))
			{
				GameMenu.SwitchToMenu("raid_village_no_resist_warn_player");
				return;
			}
			this._lastSelectedHostileAction = VillageHostileActionCampaignBehavior.HostileAction.RaidTheVillage;
			PlayerEncounter.Current.ForceRaid = true;
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x06003C39 RID: 15417 RVA: 0x0011DC4B File Offset: 0x0011BE4B
		private void game_menu_villagers_resist_attack_resistance_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x0011DC58 File Offset: 0x0011BE58
		private void CheckVillageAttackableHonorably(MenuCallbackArgs args)
		{
			Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
			IFaction faction = ((currentSettlement != null) ? currentSettlement.MapFaction : null);
			this.CheckFactionAttackableHonorably(args, faction);
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x0011DC84 File Offset: 0x0011BE84
		private void CheckFactionAttackableHonorably(MenuCallbackArgs args, IFaction faction)
		{
			if (faction.NotAttackableByPlayerUntilTime.IsFuture)
			{
				args.IsEnabled = false;
				args.Tooltip = this.EnemyNotAttackableTooltip;
			}
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x0011DCB4 File Offset: 0x0011BEB4
		private bool game_menu_no_resist_plunder_village_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Raid;
			return !this.IsThereAnyDefence() && !FactionManager.IsAlliedWithFaction(Hero.MainHero.MapFaction, Settlement.CurrentSettlement.MapFaction);
		}

		// Token: 0x06003C3D RID: 15421 RVA: 0x0011DCE4 File Offset: 0x0011BEE4
		private void game_menu_villagers_resist_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			Village village = currentSettlement.Village;
			if (PlayerEncounter.Battle != null)
			{
				PlayerEncounter.Update();
			}
			else if (currentSettlement.SettlementHitPoints <= 0f)
			{
				GameMenu.SwitchToMenu("village_looted");
				return;
			}
			if (!this.game_menu_villagers_resist_attack_resistance_on_condition(args))
			{
				if (this._lastSelectedHostileAction == VillageHostileActionCampaignBehavior.HostileAction.TakeSupplies)
				{
					GameMenu.SwitchToMenu("village_take_food_confirm");
				}
				else if (this._lastSelectedHostileAction == VillageHostileActionCampaignBehavior.HostileAction.GetVolunteers)
				{
					GameMenu.SwitchToMenu("village_press_into_service_confirm");
				}
			}
			MBTextManager.SetTextVariable("STATE", GameTexts.FindText(this.IsThereAnyDefence() ? "str_raid_resist" : "str_village_raid_villagers_are_nonresistant", null), false);
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x0011DD7C File Offset: 0x0011BF7C
		private static void game_menu_village_start_attack_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (PlayerEncounter.Battle != null)
			{
				PlayerEncounter.Update();
			}
			else if (currentSettlement.SettlementHitPoints <= 0f)
			{
				GameMenu.SwitchToMenu("village_looted");
			}
			float lastDemandSatisfiedTime = currentSettlement.Village.LastDemandSatisfiedTime;
			if (lastDemandSatisfiedTime > 0f && (Campaign.CurrentTime - lastDemandSatisfiedTime) / 24f < 7f)
			{
				GameTexts.SetVariable("STATE", GameTexts.FindText("str_villiger_recently_satisfied_demands", null));
				return;
			}
			GameTexts.SetVariable("STATE", GameTexts.FindText("str_villigers_grab_their_tools", null));
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x0011DE07 File Offset: 0x0011C007
		private static bool game_menu_menu_village_take_food_success_take_supplies_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			return true;
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x0011DE12 File Offset: 0x0011C012
		private bool game_menu_villagers_resist_attack_resistance_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return this.IsThereAnyDefence();
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x0011DE28 File Offset: 0x0011C028
		private bool IsThereAnyDefence()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null)
			{
				foreach (MobileParty mobileParty in currentSettlement.Parties)
				{
					if (!mobileParty.IsMainParty && mobileParty.MapFaction == currentSettlement.MapFaction && mobileParty.Party.NumberOfHealthyMembers > 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x0011DEA8 File Offset: 0x0011C0A8
		public static void game_menu_menu_village_take_food_success_let_them_keep_it_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village");
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x0011DEB4 File Offset: 0x0011C0B4
		public static bool game_menu_menu_village_take_food_success_let_them_keep_it_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x0011DEBE File Offset: 0x0011C0BE
		public static void hostile_action_village_on_init(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("VILLAGE_NAME", PlayerEncounter.EncounterSettlement.Name, false);
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x0011DED5 File Offset: 0x0011C0D5
		public static void wait_menu_raiding_village_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			args.MenuContext.GameMenu.SetProgressOfWaitingInMenu(1f - PlayerEncounter.Battle.MapEventSettlement.SettlementHitPoints);
		}

		// Token: 0x06003C46 RID: 15430 RVA: 0x0011DEFC File Offset: 0x0011C0FC
		public static bool wait_menu_start_raiding_on_condition(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("SETTLEMENT_NAME", PlayerEncounter.Battle.MapEventSettlement.Name, false);
			return true;
		}

		// Token: 0x06003C47 RID: 15431 RVA: 0x0011DF19 File Offset: 0x0011C119
		public static void wait_menu_end_raiding_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Current.ForceRaid = false;
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x0011DF2C File Offset: 0x0011C12C
		private static void game_menu_settlement_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
			Campaign.Current.SaveHandler.SignalAutoSave();
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x0011DF48 File Offset: 0x0011C148
		[GameMenuInitializationHandler("village_player_raid_ended")]
		public static void game_menu_village_raid_ended_menu_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("wait_raiding_village");
			if (MobileParty.MainParty.LastVisitedSettlement != null && MobileParty.MainParty.LastVisitedSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/village_raided");
			}
		}

		// Token: 0x06003C4A RID: 15434 RVA: 0x0011DFA1 File Offset: 0x0011C1A1
		[GameMenuInitializationHandler("village_looted")]
		[GameMenuInitializationHandler("village_raid_ended_leaded_by_someone_else")]
		[GameMenuInitializationHandler("raiding_village")]
		private static void game_menu_ui_village_hostile_raid_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("wait_raiding_village");
		}

		// Token: 0x06003C4B RID: 15435 RVA: 0x0011DFB4 File Offset: 0x0011C1B4
		[GameMenuInitializationHandler("village_hostile_action")]
		[GameMenuInitializationHandler("force_volunteers_village")]
		[GameMenuInitializationHandler("force_supplies_village")]
		[GameMenuInitializationHandler("raid_village_no_resist_warn_player")]
		[GameMenuInitializationHandler("raid_village_resisted")]
		[GameMenuInitializationHandler("village_loot_no_resist")]
		[GameMenuInitializationHandler("village_take_food_confirm")]
		[GameMenuInitializationHandler("village_press_into_service_confirm")]
		[GameMenuInitializationHandler("menu_press_into_service_success")]
		[GameMenuInitializationHandler("menu_village_take_food_success")]
		public static void game_menu_village_menu_on_init(MenuCallbackArgs args)
		{
			Village village = Settlement.CurrentSettlement.Village;
			args.MenuContext.SetBackgroundMeshName(village.WaitMeshName);
		}

		// Token: 0x06003C4C RID: 15436 RVA: 0x0011DFDD File Offset: 0x0011C1DD
		private static bool continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003C4D RID: 15437 RVA: 0x0011DFE8 File Offset: 0x0011C1E8
		private static bool back_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x0400124B RID: 4683
		private readonly TextObject EnemyNotAttackableTooltip = GameTexts.FindText("str_enemy_not_attackable_tooltip", null);

		// Token: 0x0400124C RID: 4684
		private VillageHostileActionCampaignBehavior.HostileAction _lastSelectedHostileAction;

		// Token: 0x0400124D RID: 4685
		private Dictionary<string, CampaignTime> _villageLastHostileActionTimeDictionary = new Dictionary<string, CampaignTime>();

		// Token: 0x02000740 RID: 1856
		private enum HostileAction
		{
			// Token: 0x04001DEF RID: 7663
			RaidTheVillage,
			// Token: 0x04001DF0 RID: 7664
			TakeSupplies,
			// Token: 0x04001DF1 RID: 7665
			GetVolunteers
		}
	}
}
