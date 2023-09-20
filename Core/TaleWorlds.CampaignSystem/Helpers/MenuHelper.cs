using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class MenuHelper
	{
		public static bool SetOptionProperties(MenuCallbackArgs args, bool canPlayerDo, bool shouldBeDisabled, TextObject disabledText)
		{
			if (canPlayerDo)
			{
				return true;
			}
			if (!shouldBeDisabled)
			{
				return false;
			}
			args.IsEnabled = false;
			args.Tooltip = disabledText;
			return true;
		}

		public static void SetIssueAndQuestDataForHero(MenuCallbackArgs args, Hero hero)
		{
			if (hero.Issue != null && hero.Issue.IssueQuest == null)
			{
				args.OptionQuestData |= GameMenuOption.IssueQuestFlags.AvailableIssue;
			}
			List<QuestBase> list;
			Campaign.Current.QuestManager.TrackedObjects.TryGetValue(hero, out list);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].IsTrackEnabled)
					{
						if (list[i].IsSpecialQuest)
						{
							if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.TrackedStoryQuest) == GameMenuOption.IssueQuestFlags.None && list[i].QuestGiver != hero)
							{
								args.OptionQuestData |= GameMenuOption.IssueQuestFlags.TrackedStoryQuest;
							}
							else if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveStoryQuest) == GameMenuOption.IssueQuestFlags.None && list[i].QuestGiver == hero)
							{
								args.OptionQuestData |= GameMenuOption.IssueQuestFlags.ActiveStoryQuest;
							}
						}
						else if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.TrackedIssue) == GameMenuOption.IssueQuestFlags.None && list[i].QuestGiver != hero)
						{
							args.OptionQuestData |= GameMenuOption.IssueQuestFlags.TrackedIssue;
						}
						else if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveIssue) == GameMenuOption.IssueQuestFlags.None && list[i].QuestGiver == hero)
						{
							args.OptionQuestData |= GameMenuOption.IssueQuestFlags.ActiveIssue;
						}
					}
				}
			}
			if (hero.PartyBelongedTo != null && ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveStoryQuest) == GameMenuOption.IssueQuestFlags.None || (args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveIssue) == GameMenuOption.IssueQuestFlags.None || (args.OptionQuestData & GameMenuOption.IssueQuestFlags.TrackedIssue) == GameMenuOption.IssueQuestFlags.None || (args.OptionQuestData & GameMenuOption.IssueQuestFlags.TrackedStoryQuest) == GameMenuOption.IssueQuestFlags.None))
			{
				List<QuestBase> list2;
				Campaign.Current.QuestManager.TrackedObjects.TryGetValue(hero.PartyBelongedTo, out list2);
				if (list2 != null)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						if (list2[j].IsTrackEnabled)
						{
							if (list2[j].IsSpecialQuest)
							{
								if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.TrackedStoryQuest) == GameMenuOption.IssueQuestFlags.None && list2[j].QuestGiver != hero)
								{
									args.OptionQuestData |= GameMenuOption.IssueQuestFlags.TrackedStoryQuest;
								}
								else if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveStoryQuest) == GameMenuOption.IssueQuestFlags.None && list2[j].QuestGiver == hero)
								{
									args.OptionQuestData |= GameMenuOption.IssueQuestFlags.ActiveStoryQuest;
								}
							}
							else if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.TrackedIssue) == GameMenuOption.IssueQuestFlags.None && list2[j].QuestGiver != hero)
							{
								args.OptionQuestData |= GameMenuOption.IssueQuestFlags.TrackedIssue;
							}
							else if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveIssue) == GameMenuOption.IssueQuestFlags.None && list2[j].QuestGiver == hero)
							{
								args.OptionQuestData |= GameMenuOption.IssueQuestFlags.ActiveIssue;
							}
						}
					}
				}
			}
			if ((args.OptionQuestData & GameMenuOption.IssueQuestFlags.ActiveIssue) == GameMenuOption.IssueQuestFlags.None)
			{
				IssueBase issue = hero.Issue;
				if (((issue != null) ? issue.IssueQuest : null) != null && hero.Issue.IssueQuest.IsTrackEnabled)
				{
					args.OptionQuestData |= GameMenuOption.IssueQuestFlags.ActiveIssue;
				}
			}
		}

		public static void SetIssueAndQuestDataForLocations(MenuCallbackArgs args, List<Location> locations)
		{
			GameMenuOption.IssueQuestFlags issueQuestFlags = Campaign.Current.IssueManager.CheckIssueForMenuLocations(locations, true);
			args.OptionQuestData |= issueQuestFlags;
			args.OptionQuestData |= Campaign.Current.QuestManager.CheckQuestForMenuLocations(locations);
		}

		public static void DecideMenuState()
		{
			string genericStateMenu = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (!string.IsNullOrEmpty(genericStateMenu))
			{
				GameMenu.SwitchToMenu(genericStateMenu);
				return;
			}
			GameMenu.ExitToLast();
		}

		public static bool EncounterAttackCondition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			if (MapEvent.PlayerMapEvent == null)
			{
				return false;
			}
			MapEvent battle = PlayerEncounter.Battle;
			Settlement settlement = ((battle != null) ? battle.MapEventSettlement : null);
			if (battle != null && settlement != null && settlement.IsFortification && battle.IsSiegeAssault && PlayerSiege.PlayerSiegeEvent != null && !PlayerSiege.PlayerSiegeEvent.BesiegerCamp.IsPreparationComplete)
			{
				return false;
			}
			bool flag = battle != null && (battle.HasTroopsOnBothSides() || battle.IsSiegeAssault) && MapEvent.PlayerMapEvent.GetLeaderParty(PartyBase.MainParty.OpponentSide) != null;
			if (Hero.MainHero.IsWounded)
			{
				args.Tooltip = new TextObject("{=UL8za0AO}You are wounded.", null);
				args.IsEnabled = false;
			}
			return flag;
		}

		public static bool EncounterCaptureEnemyCondition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
			MapEvent battle = PlayerEncounter.Battle;
			if (battle != null)
			{
				return battle.PartiesOnSide(battle.GetOtherSide(battle.PlayerSide)).All((MapEventParty party) => !party.Party.IsSettlement && party.Party.NumberOfHealthyMembers == 0);
			}
			return false;
		}

		public static void EncounterAttackConsequence(MenuCallbackArgs args)
		{
			MapEvent battle = PlayerEncounter.Battle;
			PartyBase leaderParty = battle.GetLeaderParty(PartyBase.MainParty.OpponentSide);
			if (!leaderParty.IsMobile)
			{
				BeHostileAction.ApplyEncounterHostileAction(PartyBase.MainParty, leaderParty);
			}
			if (PlayerEncounter.Current != null)
			{
				Settlement mapEventSettlement = MobileParty.MainParty.MapEvent.MapEventSettlement;
				if (mapEventSettlement != null && !battle.IsSallyOut && !battle.IsSiegeOutside)
				{
					if (mapEventSettlement.IsFortification)
					{
						if (battle.IsRaid)
						{
							PlayerEncounter.StartVillageBattleMission();
						}
						else if (battle.IsSiegeAssault)
						{
							if (PlayerSiege.PlayerSiegeEvent == null && PartyBase.MainParty.Side == BattleSideEnum.Attacker)
							{
								PlayerSiege.StartPlayerSiege(MobileParty.MainParty.Party.Side, false, mapEventSettlement);
							}
							else
							{
								if (PlayerSiege.PlayerSiegeEvent != null)
								{
									if (!PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide.GetOppositeSide()).GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.NumberOfHealthyMembers > 0))
									{
										PlayerEncounter.Update();
										return;
									}
								}
								if (PlayerSiege.BesiegedSettlement != null && PlayerSiege.BesiegedSettlement.CurrentSiegeState == Settlement.SiegeState.InTheLordsHall)
								{
									FlattenedTroopRoster priorityListForLordsHallFightMission = Campaign.Current.Models.SiegeLordsHallFightModel.GetPriorityListForLordsHallFightMission(MapEvent.PlayerMapEvent, BattleSideEnum.Defender, Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderSideTroopCount);
									int num = MathF.Max(1, MathF.Min(Campaign.Current.Models.SiegeLordsHallFightModel.MaxAttackerSideTroopCount, MathF.Round((float)priorityListForLordsHallFightMission.Troops.Count<CharacterObject>() * Campaign.Current.Models.SiegeLordsHallFightModel.AttackerDefenderTroopCountRatio)));
									TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
									MobileParty mobileParty = ((MobileParty.MainParty.Army != null) ? MobileParty.MainParty.Army.LeaderParty : MobileParty.MainParty);
									troopRoster.Add(mobileParty.MemberRoster);
									foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
									{
										troopRoster.Add(mobileParty2.MemberRoster);
									}
									TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
									FlattenedTroopRoster flattenedTroopRoster = troopRoster.ToFlattenedRoster();
									flattenedTroopRoster.RemoveIf((FlattenedTroopRosterElement x) => x.IsWounded);
									troopRoster2.Add(MobilePartyHelper.GetStrongestAndPriorTroops(flattenedTroopRoster, num, true));
									args.MenuContext.OpenTroopSelection(troopRoster, troopRoster2, (CharacterObject character) => !character.IsPlayerCharacter, new Action<TroopRoster>(MenuHelper.LordsHallTroopRosterManageDone), num, num);
								}
								else
								{
									PlayerSiege.StartSiegeMission(PlayerEncounter.Current.IsSallyOutAmbush, mapEventSettlement);
								}
							}
						}
					}
					else if (mapEventSettlement.IsVillage)
					{
						PlayerEncounter.StartVillageBattleMission();
					}
					else if (mapEventSettlement.IsHideout)
					{
						CampaignMission.OpenHideoutBattleMission("sea_bandit_a", null);
					}
				}
				else
				{
					MapPatchData mapPatchAtPosition = Campaign.Current.MapSceneWrapper.GetMapPatchAtPosition(MobileParty.MainParty.Position2D);
					string battleSceneForMapPatch = PlayerEncounter.GetBattleSceneForMapPatch(mapPatchAtPosition);
					MissionInitializerRecord missionInitializerRecord = new MissionInitializerRecord(battleSceneForMapPatch);
					missionInitializerRecord.TerrainType = (int)Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
					missionInitializerRecord.DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier();
					missionInitializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
					missionInitializerRecord.DamageFromPlayerToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
					missionInitializerRecord.NeedsRandomTerrain = false;
					missionInitializerRecord.PlayingInCampaignMode = true;
					missionInitializerRecord.RandomTerrainSeed = MBRandom.RandomInt(10000);
					missionInitializerRecord.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition());
					missionInitializerRecord.SceneHasMapPatch = true;
					missionInitializerRecord.AtlasGroup = 2;
					missionInitializerRecord.PatchCoordinates = mapPatchAtPosition.normalizedCoordinates;
					missionInitializerRecord.PatchEncounterDir = (battle.AttackerSide.LeaderParty.Position2D - battle.DefenderSide.LeaderParty.Position2D).Normalized();
					float num2 = Campaign.CurrentTime % 24f;
					if (Campaign.Current != null)
					{
						missionInitializerRecord.TimeOfDay = num2;
					}
					bool flag = MapEvent.PlayerMapEvent.PartiesOnSide(BattleSideEnum.Defender).Any((MapEventParty involvedParty) => involvedParty.Party.IsMobile && involvedParty.Party.MobileParty.IsCaravan);
					bool flag2;
					if (MapEvent.PlayerMapEvent.MapEventSettlement == null)
					{
						flag2 = MapEvent.PlayerMapEvent.PartiesOnSide(BattleSideEnum.Defender).Any((MapEventParty involvedParty) => involvedParty.Party.IsMobile && involvedParty.Party.MobileParty.IsVillager);
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag || flag3)
					{
						CampaignMission.OpenCaravanBattleMission(missionInitializerRecord, flag);
					}
					else
					{
						CampaignMission.OpenBattleMission(missionInitializerRecord);
					}
				}
				PlayerEncounter.StartAttackMission();
				MapEvent.PlayerMapEvent.BeginWait();
			}
		}

		private static void LordsHallTroopRosterManageDone(TroopRoster selectedTroops)
		{
			MapEvent.PlayerMapEvent.ResetBattleState();
			int wallLevel = PlayerSiege.BesiegedSettlement.Town.GetWallLevel();
			CampaignMission.OpenSiegeLordsHallFightMission(PlayerSiege.BesiegedSettlement.LocationComplex.GetLocationWithId("lordshall").GetSceneName(wallLevel), selectedTroops.ToFlattenedRoster());
		}

		private static void LordsHallTroopRosterManageDoneForSimulation(TroopRoster selectedTroops)
		{
			MenuHelper.EncounterOrderAttack(selectedTroops);
		}

		private static void EncounterOrderAttack(TroopRoster selectedTroopsForPlayerSide)
		{
			MapEvent battle = PlayerEncounter.Battle;
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				ISiegeEventSide siegeEventSide = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(PlayerSiege.PlayerSide.GetOppositeSide());
				if (siegeEventSide != null)
				{
					if (!siegeEventSide.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.NumberOfHealthyMembers > 0))
					{
						PlayerEncounter.Update();
						return;
					}
				}
			}
			PartyBase leaderParty = battle.GetLeaderParty(PartyBase.MainParty.OpponentSide);
			SiegeEvent siegeEvent = leaderParty.SiegeEvent;
			if (((siegeEvent != null) ? siegeEvent.BesiegerCamp : null) != null && !leaderParty.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(leaderParty, MapEvent.BattleTypes.Siege) && MobileParty.MainParty.BesiegerCamp == null)
			{
				MobileParty.MainParty.BesiegerCamp = leaderParty.SiegeEvent.BesiegerCamp;
			}
			BeHostileAction.ApplyEncounterHostileAction(PartyBase.MainParty, leaderParty);
			if (PlayerEncounter.Current != null)
			{
				GameMenu.ExitToLast();
				if (selectedTroopsForPlayerSide != null && PlayerSiege.BesiegedSettlement != null && PlayerSiege.BesiegedSettlement.CurrentSiegeState == Settlement.SiegeState.InTheLordsHall)
				{
					FlattenedTroopRoster priorityListForLordsHallFightMission = Campaign.Current.Models.SiegeLordsHallFightModel.GetPriorityListForLordsHallFightMission(MapEvent.PlayerMapEvent, BattleSideEnum.Defender, Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderSideTroopCount);
					PlayerEncounter.InitSimulation(selectedTroopsForPlayerSide.ToFlattenedRoster(), priorityListForLordsHallFightMission);
				}
				else
				{
					PlayerEncounter.InitSimulation(null, null);
				}
				if (PlayerEncounter.Current != null && PlayerEncounter.Current.BattleSimulation != null)
				{
					((MapState)Game.Current.GameStateManager.ActiveState).StartBattleSimulation();
				}
			}
		}

		public static void EncounterOrderAttackConsequence(MenuCallbackArgs args)
		{
			if (PlayerSiege.BesiegedSettlement != null && PlayerSiege.BesiegedSettlement.CurrentSiegeState == Settlement.SiegeState.InTheLordsHall)
			{
				FlattenedTroopRoster priorityListForLordsHallFightMission = Campaign.Current.Models.SiegeLordsHallFightModel.GetPriorityListForLordsHallFightMission(MapEvent.PlayerMapEvent, BattleSideEnum.Defender, Campaign.Current.Models.SiegeLordsHallFightModel.MaxDefenderSideTroopCount);
				int num = MathF.Max(1, MathF.Min(Campaign.Current.Models.SiegeLordsHallFightModel.MaxAttackerSideTroopCount, MathF.Round((float)priorityListForLordsHallFightMission.Troops.Count<CharacterObject>() * Campaign.Current.Models.SiegeLordsHallFightModel.AttackerDefenderTroopCountRatio)));
				TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
				MobileParty mobileParty = ((MobileParty.MainParty.Army != null) ? MobileParty.MainParty.Army.LeaderParty : MobileParty.MainParty);
				troopRoster.Add(mobileParty.MemberRoster);
				foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
				{
					troopRoster.Add(mobileParty2.MemberRoster);
				}
				TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
				FlattenedTroopRoster flattenedTroopRoster = troopRoster.ToFlattenedRoster();
				flattenedTroopRoster.RemoveIf((FlattenedTroopRosterElement x) => x.IsWounded);
				troopRoster2.Add(MobilePartyHelper.GetStrongestAndPriorTroops(flattenedTroopRoster, num, false));
				args.MenuContext.OpenTroopSelection(troopRoster, troopRoster2, (CharacterObject character) => !character.IsPlayerCharacter, new Action<TroopRoster>(MenuHelper.LordsHallTroopRosterManageDoneForSimulation), num, num);
				return;
			}
			MenuHelper.EncounterOrderAttack(null);
		}

		public static void EncounterCaptureTheEnemyOnConsequence(MenuCallbackArgs args)
		{
			MapEvent.PlayerMapEvent.SetOverrideWinner(MapEvent.PlayerMapEvent.PlayerSide);
			PlayerEncounter.Update();
		}

		public static void EncounterLeaveConsequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
			MapEvent mapEvent = ((PlayerEncounter.Battle != null) ? PlayerEncounter.Battle : PlayerEncounter.EncounteredBattle);
			int numberOfInvolvedMen = mapEvent.GetNumberOfInvolvedMen(PartyBase.MainParty.Side);
			Settlement currentSettlement2 = MobileParty.MainParty.CurrentSettlement;
			bool flag;
			if (((currentSettlement2 != null) ? currentSettlement2.SiegeEvent : null) != null)
			{
				Settlement currentSettlement3 = MobileParty.MainParty.CurrentSettlement;
				flag = ((currentSettlement3 != null) ? currentSettlement3.MapFaction : null) != MobileParty.MainParty.MapFaction;
			}
			else
			{
				flag = true;
			}
			PlayerEncounter.Finish(flag);
			if (MobileParty.MainParty.BesiegerCamp != null)
			{
				MobileParty.MainParty.BesiegerCamp = null;
			}
			if (mapEvent != null && !mapEvent.IsRaid && numberOfInvolvedMen == PartyBase.MainParty.NumberOfHealthyMembers)
			{
				MapEvent mapEvent2 = mapEvent;
				PlayerEncounter playerEncounter = PlayerEncounter.Current;
				FlattenedTroopRoster[] array;
				if (playerEncounter == null)
				{
					array = null;
				}
				else
				{
					BattleSimulation battleSimulation = playerEncounter.BattleSimulation;
					array = ((battleSimulation != null) ? battleSimulation.SelectedTroops : null);
				}
				mapEvent2.SimulateBattleSetup(array);
				mapEvent.SimulateBattleForRounds((PartyBase.MainParty.Side == BattleSideEnum.Attacker) ? 1 : 0, (PartyBase.MainParty.Side == BattleSideEnum.Attacker) ? 0 : 1);
			}
			if (currentSettlement != null)
			{
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, currentSettlement);
			}
		}
	}
}
