using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class HideoutCampaignBehavior : CampaignBehaviorBase
	{
		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		public void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.HourlyTickSettlement));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.OnHideoutSpottedEvent.AddNonSerializedListener(this, new Action<PartyBase, PartyBase>(this.OnHideoutSpotted));
		}

		private void OnHideoutSpotted(PartyBase party, PartyBase hideout)
		{
			SkillLevelingManager.OnHideoutSpotted(party.MobileParty, hideout);
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<float>("_hideoutWaitProgressHours", ref this._hideoutWaitProgressHours);
			dataStore.SyncData<float>("_hideoutWaitTargetHours", ref this._hideoutWaitTargetHours);
		}

		public void HourlyTickSettlement(Settlement settlement)
		{
			if (settlement.IsHideout && settlement.Hideout.IsInfested && !settlement.Hideout.IsSpotted)
			{
				float hideoutSpottingDistance = Campaign.Current.Models.MapVisibilityModel.GetHideoutSpottingDistance();
				float num = MobileParty.MainParty.Position2D.DistanceSquared(settlement.Position2D);
				float num2 = 1f - num / (hideoutSpottingDistance * hideoutSpottingDistance);
				if (num2 > 0f && settlement.Parties.Count > 0 && MBRandom.RandomFloat < num2 && !settlement.Hideout.IsSpotted)
				{
					settlement.Hideout.IsSpotted = true;
					settlement.IsVisible = true;
					CampaignEventDispatcher.Instance.OnHideoutSpotted(MobileParty.MainParty.Party, settlement.Party);
				}
			}
		}

		protected void AddGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenu("hideout_place", "{=Vo64LSGP}{HIDEOUT_TEXT}", new OnInitDelegate(this.game_menu_hideout_place_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameStarter.AddGameMenuOption("hideout_place", "wait", "{=4Sb0d8FY}Wait until nightfall to attack", new GameMenuOption.OnConditionDelegate(this.game_menu_wait_until_nightfall_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_wait_until_nightfall_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("hideout_place", "attack", "{=zxMOqlhs}Attack", new GameMenuOption.OnConditionDelegate(this.game_menu_attack_hideout_parties_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_attack_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("hideout_place", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_hideout_leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddWaitGameMenu("hideout_wait", "{=VLLAOXve}Waiting until nightfall to ambush", null, new OnConditionDelegate(this.hideout_wait_menu_on_condition), new OnConsequenceDelegate(this.hideout_wait_menu_on_consequence), new OnTickDelegate(this.hideout_wait_menu_on_tick), GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption, GameOverlays.MenuOverlayType.None, this._hideoutWaitTargetHours, GameMenu.MenuFlags.None, null);
			campaignGameStarter.AddGameMenuOption("hideout_wait", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_hideout_leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenu("hideout_after_wait", "{=Vo64LSGP}{HIDEOUT_TEXT}", new OnInitDelegate(this.hideout_after_wait_menu_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameStarter.AddGameMenuOption("hideout_after_wait", "attack", "{=zxMOqlhs}Attack", new GameMenuOption.OnConditionDelegate(this.game_menu_attack_hideout_parties_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_attack_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("hideout_after_wait", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_hideout_leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenu("hideout_after_defeated_and_saved", "{=1zLZf5rw}The rest of your men rushed to your help, dragging you out to safety and driving the bandits back into hiding.", new OnInitDelegate(this.game_menu_hideout_after_defeated_and_saved_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameStarter.AddGameMenuOption("hideout_after_defeated_and_saved", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_hideout_leave_on_consequence), true, -1, false, null);
		}

		private bool IsHideoutAttackableNow()
		{
			float currentHourInDay = CampaignTime.Now.CurrentHourInDay;
			return (this.CanAttackHideoutStart > this.CanAttackHideoutEnd && (currentHourInDay >= (float)this.CanAttackHideoutStart || currentHourInDay <= (float)this.CanAttackHideoutEnd)) || (this.CanAttackHideoutStart < this.CanAttackHideoutEnd && currentHourInDay >= (float)this.CanAttackHideoutStart && currentHourInDay <= (float)this.CanAttackHideoutEnd);
		}

		public bool hideout_wait_menu_on_condition(MenuCallbackArgs args)
		{
			return true;
		}

		public void hideout_wait_menu_on_tick(MenuCallbackArgs args, CampaignTime campaignTime)
		{
			this._hideoutWaitProgressHours += (float)campaignTime.ToHours;
			if (this._hideoutWaitTargetHours.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this.CalculateHideoutAttackTime();
			}
			args.MenuContext.GameMenu.SetProgressOfWaitingInMenu(this._hideoutWaitProgressHours / this._hideoutWaitTargetHours);
		}

		public void hideout_wait_menu_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("hideout_after_wait");
		}

		private bool leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		[GameMenuInitializationHandler("hideout_wait")]
		[GameMenuInitializationHandler("hideout_after_wait")]
		private static void game_menu_hideout_ui_place_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Hideout.WaitMeshName);
		}

		[GameMenuInitializationHandler("hideout_place")]
		private static void game_menu_hideout_sound_place_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetPanelSound("event:/ui/panels/settlement_hideout");
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Hideout.WaitMeshName);
		}

		private void game_menu_hideout_after_defeated_and_saved_on_init(MenuCallbackArgs args)
		{
			if (!Settlement.CurrentSettlement.IsHideout)
			{
				return;
			}
			if (MobileParty.MainParty.CurrentSettlement == null)
			{
				PlayerEncounter.EnterSettlement();
			}
		}

		private void game_menu_hideout_place_on_init(MenuCallbackArgs args)
		{
			if (!Settlement.CurrentSettlement.IsHideout)
			{
				return;
			}
			this._hideoutWaitProgressHours = 0f;
			if (!this.IsHideoutAttackableNow())
			{
				this.CalculateHideoutAttackTime();
			}
			else
			{
				this._hideoutWaitTargetHours = 0f;
			}
			Settlement currentSettlement = Settlement.CurrentSettlement;
			int num = 0;
			foreach (MobileParty mobileParty in currentSettlement.Parties)
			{
				num += mobileParty.MemberRoster.TotalManCount - mobileParty.MemberRoster.TotalWounded;
			}
			GameTexts.SetVariable("HIDEOUT_DESCRIPTION", "{=DOmb81Mu}(Undefined hideout type)");
			if (currentSettlement.Culture.StringId.Equals("forest_bandits"))
			{
				GameTexts.SetVariable("HIDEOUT_DESCRIPTION", "{=cu2cLT5r}You spy though the trees what seems to be a clearing in the forest with what appears to be the outlines of a camp.");
			}
			if (currentSettlement.Culture.StringId.Equals("sea_raiders"))
			{
				GameTexts.SetVariable("HIDEOUT_DESCRIPTION", "{=bJ6ygV3P}As you travel along the coast, you see a sheltered cove with what appears to the outlines of a camp.");
			}
			if (currentSettlement.Culture.StringId.Equals("mountain_bandits"))
			{
				GameTexts.SetVariable("HIDEOUT_DESCRIPTION", "{=iyWUDSm8}Passing by the slopes of the mountains, you see an outcrop crowned with the ruins of an ancient fortress.");
			}
			if (currentSettlement.Culture.StringId.Equals("desert_bandits"))
			{
				GameTexts.SetVariable("HIDEOUT_DESCRIPTION", "{=b3iBOVXN}Passing by a wadi, you see what looks like a camouflaged well to tap the groundwater left behind by rare rainfalls.");
			}
			if (currentSettlement.Culture.StringId.Equals("steppe_bandits"))
			{
				GameTexts.SetVariable("HIDEOUT_DESCRIPTION", "{=5JaGVr0U}While traveling by a low range of hills, you see what appears to be the remains of a campsite in a stream gully.");
			}
			bool flag = !currentSettlement.Hideout.NextPossibleAttackTime.IsPast;
			if (flag)
			{
				GameTexts.SetVariable("HIDEOUT_TEXT", "{=KLWn6yZQ}{HIDEOUT_DESCRIPTION} The remains of a fire suggest that it's been recently occupied, but its residents - whoever they are - are well-hidden for now.");
			}
			else if (num > 0)
			{
				GameTexts.SetVariable("HIDEOUT_TEXT", "{=prcBBqMR}{HIDEOUT_DESCRIPTION} You see armed men moving about. As you listen quietly, you hear scraps of conversation about raids, ransoms, and the best places to waylay travellers.");
			}
			else
			{
				GameTexts.SetVariable("HIDEOUT_TEXT", "{=gywyEgZa}{HIDEOUT_DESCRIPTION} There seems to be no one inside.");
			}
			if (!flag && num > 0 && Hero.MainHero.IsWounded)
			{
				GameTexts.SetVariable("HIDEOUT_TEXT", "{=fMekM2UH}{HIDEOUT_DESCRIPTION}. You can not attack since your wounds do not allow you.");
			}
			if (MobileParty.MainParty.CurrentSettlement == null)
			{
				PlayerEncounter.EnterSettlement();
			}
			bool isInfested = Settlement.CurrentSettlement.Hideout.IsInfested;
			Settlement settlement = (Settlement.CurrentSettlement.IsHideout ? Settlement.CurrentSettlement : null);
			if (PlayerEncounter.Battle != null)
			{
				bool flag2 = PlayerEncounter.Battle.WinningSide == PlayerEncounter.Current.PlayerSide;
				PlayerEncounter.Update();
				if (flag2 && PlayerEncounter.Battle == null && settlement != null)
				{
					this.SetCleanHideoutRelations(settlement);
				}
			}
		}

		private void CalculateHideoutAttackTime()
		{
			this._hideoutWaitTargetHours = (((float)this.CanAttackHideoutStart > CampaignTime.Now.CurrentHourInDay) ? ((float)this.CanAttackHideoutStart - CampaignTime.Now.CurrentHourInDay) : (24f - CampaignTime.Now.CurrentHourInDay + (float)this.CanAttackHideoutStart));
		}

		private void SetCleanHideoutRelations(Settlement hideout)
		{
			List<Settlement> list = new List<Settlement>();
			foreach (Village village in Campaign.Current.AllVillages)
			{
				if (village.Settlement.Position2D.DistanceSquared(hideout.Position2D) <= 1600f)
				{
					list.Add(village.Settlement);
				}
			}
			foreach (Settlement settlement in list)
			{
				if (settlement.Notables.Count > 0)
				{
					ChangeRelationAction.ApplyPlayerRelation(settlement.Notables.GetRandomElement<Hero>(), 2, true, false);
				}
			}
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Charm.EffortForThePeople))
			{
				Town town = SettlementHelper.FindNearestTown(null, hideout).Town;
				Hero leader = town.OwnerClan.Leader;
				if (leader == Hero.MainHero)
				{
					town.Loyalty += 1f;
				}
				else
				{
					ChangeRelationAction.ApplyPlayerRelation(leader, (int)DefaultPerks.Charm.EffortForThePeople.PrimaryBonus, true, true);
				}
			}
			MBTextManager.SetTextVariable("RELATION_VALUE", 2);
			MBInformationManager.AddQuickInformation(new TextObject("{=o0qwDa0q}Your relation increased by {RELATION_VALUE} with nearby notables.", null), 0, null, "");
		}

		private void hideout_after_wait_menu_on_init(MenuCallbackArgs args)
		{
			TextObject textObject = new TextObject("{=VbU8Ue0O}After waiting for a while you find a good opportunity to close in undetected beneath the shroud of the night.", null);
			if (Hero.MainHero.IsWounded)
			{
				TextObject textObject2 = new TextObject("{=fMekM2UH}{HIDEOUT_DESCRIPTION}. You can not attack since your wounds do not allow you.", null);
				textObject2.SetTextVariable("HIDEOUT_DESCRIPTION", textObject);
				MBTextManager.SetTextVariable("HIDEOUT_TEXT", textObject2, false);
				return;
			}
			MBTextManager.SetTextVariable("HIDEOUT_TEXT", textObject, false);
		}

		private bool game_menu_attack_hideout_parties_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			Hideout hideout = Settlement.CurrentSettlement.Hideout;
			if (!Hero.MainHero.IsWounded && Settlement.CurrentSettlement.MapFaction != PartyBase.MainParty.MapFaction)
			{
				if (Settlement.CurrentSettlement.Parties.Any((MobileParty x) => x.IsBandit) && hideout.NextPossibleAttackTime.IsPast)
				{
					return this.IsHideoutAttackableNow();
				}
			}
			return false;
		}

		private void game_menu_encounter_attack_on_consequence(MenuCallbackArgs args)
		{
			int playerMaximumTroopCountForHideoutMission = Campaign.Current.Models.BanditDensityModel.GetPlayerMaximumTroopCountForHideoutMission(MobileParty.MainParty);
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster strongestAndPriorTroops = MobilePartyHelper.GetStrongestAndPriorTroops(MobileParty.MainParty, playerMaximumTroopCountForHideoutMission, true);
			troopRoster.Add(strongestAndPriorTroops);
			Campaign campaign = Campaign.Current;
			int num = ((campaign != null) ? campaign.Models.BanditDensityModel.GetPlayerMaximumTroopCountForHideoutMission(MobileParty.MainParty) : 0);
			args.MenuContext.OpenTroopSelection(MobileParty.MainParty.MemberRoster, troopRoster, new Func<CharacterObject, bool>(this.CanChangeStatusOfTroop), new Action<TroopRoster>(this.OnTroopRosterManageDone), num, 1);
		}

		private void ArrangeHideoutTroopCountsForMission()
		{
			int numberOfMinimumBanditTroopsInHideoutMission = Campaign.Current.Models.BanditDensityModel.NumberOfMinimumBanditTroopsInHideoutMission;
			int num = Campaign.Current.Models.BanditDensityModel.NumberOfMaximumTroopCountForFirstFightInHideout + Campaign.Current.Models.BanditDensityModel.NumberOfMaximumTroopCountForBossFightInHideout;
			MBList<MobileParty> mblist = Settlement.CurrentSettlement.Parties.Where((MobileParty x) => x.IsBandit || x.IsBanditBossParty).ToMBList<MobileParty>();
			int num2 = mblist.Sum((MobileParty x) => x.MemberRoster.TotalHealthyCount);
			if (num2 > num)
			{
				int i = num2 - num;
				mblist.RemoveAll((MobileParty x) => x.IsBanditBossParty || x.MemberRoster.TotalHealthyCount == 1);
				while (i > 0)
				{
					if (mblist.Count <= 0)
					{
						return;
					}
					MobileParty randomElement = mblist.GetRandomElement<MobileParty>();
					List<TroopRosterElement> troopRoster = randomElement.MemberRoster.GetTroopRoster();
					List<ValueTuple<TroopRosterElement, float>> list = new List<ValueTuple<TroopRosterElement, float>>();
					foreach (TroopRosterElement troopRosterElement in troopRoster)
					{
						list.Add(new ValueTuple<TroopRosterElement, float>(troopRosterElement, (float)(troopRosterElement.Number - troopRosterElement.WoundedNumber)));
					}
					TroopRosterElement troopRosterElement2 = MBRandom.ChooseWeighted<TroopRosterElement>(list);
					randomElement.MemberRoster.AddToCounts(troopRosterElement2.Character, -1, false, 0, 0, true, -1);
					i--;
					if (randomElement.MemberRoster.TotalHealthyCount == 1)
					{
						mblist.Remove(randomElement);
					}
				}
			}
			else if (num2 < numberOfMinimumBanditTroopsInHideoutMission)
			{
				int num3 = numberOfMinimumBanditTroopsInHideoutMission - num2;
				mblist.RemoveAll((MobileParty x) => x.MemberRoster.GetTroopRoster().All((TroopRosterElement y) => y.Number == 0 || y.Character.Culture.BanditBoss == y.Character || y.Character.IsHero));
				while (num3 > 0 && mblist.Count > 0)
				{
					MobileParty randomElement2 = mblist.GetRandomElement<MobileParty>();
					List<TroopRosterElement> troopRoster2 = randomElement2.MemberRoster.GetTroopRoster();
					List<ValueTuple<TroopRosterElement, float>> list2 = new List<ValueTuple<TroopRosterElement, float>>();
					foreach (TroopRosterElement troopRosterElement3 in troopRoster2)
					{
						list2.Add(new ValueTuple<TroopRosterElement, float>(troopRosterElement3, (float)(troopRosterElement3.Number * ((troopRosterElement3.Character.Culture.BanditBoss == troopRosterElement3.Character || troopRosterElement3.Character.IsHero) ? 0 : 1))));
					}
					TroopRosterElement troopRosterElement4 = MBRandom.ChooseWeighted<TroopRosterElement>(list2);
					randomElement2.MemberRoster.AddToCounts(troopRosterElement4.Character, 1, false, 0, 0, true, -1);
					num3--;
				}
			}
		}

		private void OnTroopRosterManageDone(TroopRoster hideoutTroops)
		{
			this.ArrangeHideoutTroopCountsForMission();
			GameMenu.SwitchToMenu("hideout_place");
			Settlement.CurrentSettlement.Hideout.UpdateNextPossibleAttackTime();
			if (PlayerEncounter.IsActive)
			{
				PlayerEncounter.LeaveEncounter = false;
			}
			else
			{
				PlayerEncounter.Start();
				PlayerEncounter.Current.SetupFields(PartyBase.MainParty, Settlement.CurrentSettlement.Party);
			}
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
				PlayerEncounter.Update();
			}
			CampaignMission.OpenHideoutBattleMission(Settlement.CurrentSettlement.Hideout.SceneName, (hideoutTroops != null) ? hideoutTroops.ToFlattenedRoster() : null);
		}

		private bool CanChangeStatusOfTroop(CharacterObject character)
		{
			return !character.IsPlayerCharacter && !character.IsNotTransferableInHideouts;
		}

		private bool game_menu_talk_to_leader_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			PartyBase party = Settlement.CurrentSettlement.Parties[0].Party;
			return party != null && party.LeaderHero != null && party.LeaderHero != Hero.MainHero;
		}

		private void game_menu_talk_to_leader_on_consequence(MenuCallbackArgs args)
		{
			PartyBase party = Settlement.CurrentSettlement.Parties[0].Party;
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false);
			ConversationCharacterData conversationCharacterData2 = new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(party), party, false, false, false, false, false, false);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "", "");
		}

		private bool game_menu_wait_until_nightfall_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			return Settlement.CurrentSettlement.Parties.Any((MobileParty t) => t != MobileParty.MainParty) && !this.IsHideoutAttackableNow();
		}

		private void game_menu_wait_until_nightfall_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("hideout_wait");
		}

		private void game_menu_hideout_leave_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (MobileParty.MainParty.CurrentSettlement != null)
			{
				PlayerEncounter.LeaveSettlement();
			}
			PlayerEncounter.Finish(true);
		}

		private const int MaxDistanceSquaredBetweenHideoutAndBoundVillage = 1600;

		private const int HideoutClearRelationEffect = 2;

		private readonly int CanAttackHideoutStart = 23;

		private readonly int CanAttackHideoutEnd = 2;

		private float _hideoutWaitProgressHours;

		private float _hideoutWaitTargetHours;
	}
}
