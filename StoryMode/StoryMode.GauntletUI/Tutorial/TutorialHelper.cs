using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.GauntletUI;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200003D RID: 61
	public static class TutorialHelper
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00006180 File Offset: 0x00004380
		public static bool PlayerIsInAnySettlement
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && (currentSettlement.IsFortification || currentSettlement.IsVillage);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000166 RID: 358 RVA: 0x000061A8 File Offset: 0x000043A8
		public static bool PlayerIsInAnyVillage
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && currentSettlement.IsVillage;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000167 RID: 359 RVA: 0x000061BC File Offset: 0x000043BC
		public static bool IsOrderingAvailable
		{
			get
			{
				Mission mission = Mission.Current;
				if (((mission != null) ? mission.PlayerTeam : null) != null)
				{
					for (int i = 0; i < Mission.Current.PlayerTeam.FormationsIncludingEmpty.Count; i++)
					{
						Formation formation = Mission.Current.PlayerTeam.FormationsIncludingEmpty[i];
						if (formation.PlayerOwner == Agent.Main && formation.CountOfUnits > 0)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000622A File Offset: 0x0000442A
		public static bool IsCharacterPopUpWindowOpen
		{
			get
			{
				return GauntletTutorialSystem.Current.IsCharacterPortraitPopupOpen;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00006236 File Offset: 0x00004436
		public static EncyclopediaPages CurrentEncyclopediaPage
		{
			get
			{
				return GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00006242 File Offset: 0x00004442
		public static TutorialContexts CurrentContext
		{
			get
			{
				return GauntletTutorialSystem.Current.CurrentContext;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00006250 File Offset: 0x00004450
		public static bool PlayerIsInNonEnemyTown
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && currentSettlement.IsTown && !FactionManager.IsAtWarAgainstFaction(currentSettlement.MapFaction, MobileParty.MainParty.MapFaction);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00006288 File Offset: 0x00004488
		public static string ActiveVillageRaidGameMenuID
		{
			get
			{
				return "raiding_village";
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600016D RID: 365 RVA: 0x0000628F File Offset: 0x0000448F
		public static bool IsActiveVillageRaidGameMenuOpen
		{
			get
			{
				Campaign campaign = Campaign.Current;
				string text;
				if (campaign == null)
				{
					text = null;
				}
				else
				{
					MenuContext currentMenuContext = campaign.CurrentMenuContext;
					if (currentMenuContext == null)
					{
						text = null;
					}
					else
					{
						GameMenu gameMenu = currentMenuContext.GameMenu;
						text = ((gameMenu != null) ? gameMenu.StringId : null);
					}
				}
				return text == TutorialHelper.ActiveVillageRaidGameMenuID;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600016E RID: 366 RVA: 0x000062C4 File Offset: 0x000044C4
		public static bool TownMenuIsOpen
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (currentSettlement != null && currentSettlement.IsTown)
				{
					MenuContext currentMenuContext = Campaign.Current.CurrentMenuContext;
					string text;
					if (currentMenuContext == null)
					{
						text = null;
					}
					else
					{
						GameMenu gameMenu = currentMenuContext.GameMenu;
						text = ((gameMenu != null) ? gameMenu.StringId : null);
					}
					return text == "town";
				}
				return false;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00006312 File Offset: 0x00004512
		public static bool VillageMenuIsOpen
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && currentSettlement.IsVillage;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00006324 File Offset: 0x00004524
		public static bool BackStreetMenuIsOpen
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (currentSettlement != null && currentSettlement.IsTown && LocationComplex.Current != null)
				{
					Location locationWithId = LocationComplex.Current.GetLocationWithId("tavern");
					return TutorialHelper.GetMenuLocations.Contains(locationWithId);
				}
				return false;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000171 RID: 369 RVA: 0x00006368 File Offset: 0x00004568
		public static bool IsPlayerInABattleMission
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.Mode == 2;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000172 RID: 370 RVA: 0x00006389 File Offset: 0x00004589
		public static bool IsOrderOfBattleOpenAndReady
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.Mode == 6 && !LoadingWindow.IsLoadingWindowActive;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000173 RID: 371 RVA: 0x000063AC File Offset: 0x000045AC
		public static bool IsPlayerInAFight
		{
			get
			{
				Mission mission = Mission.Current;
				MissionMode? missionMode = ((mission != null) ? new MissionMode?(mission.Mode) : null);
				if (missionMode != null)
				{
					MissionMode? missionMode2 = missionMode;
					MissionMode missionMode3 = 2;
					if (!((missionMode2.GetValueOrDefault() == missionMode3) & (missionMode2 != null)))
					{
						missionMode2 = missionMode;
						missionMode3 = 3;
						if (!((missionMode2.GetValueOrDefault() == missionMode3) & (missionMode2 != null)))
						{
							missionMode2 = missionMode;
							missionMode3 = 7;
							return (missionMode2.GetValueOrDefault() == missionMode3) & (missionMode2 != null);
						}
					}
					return true;
				}
				return false;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000174 RID: 372 RVA: 0x0000642C File Offset: 0x0000462C
		public static bool IsPlayerEncounterLeader
		{
			get
			{
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return false;
				}
				Team playerTeam = mission.PlayerTeam;
				bool? flag = ((playerTeam != null) ? new bool?(playerTeam.IsPlayerGeneral) : null);
				bool flag2 = true;
				return (flag.GetValueOrDefault() == flag2) & (flag != null);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00006478 File Offset: 0x00004678
		public static bool IsPlayerInAHideoutBattleMission
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.HasMissionBehavior<HideoutMissionController>();
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00006496 File Offset: 0x00004696
		public static IList<Location> GetMenuLocations
		{
			get
			{
				return Campaign.Current.GameMenuManager.MenuLocations;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000177 RID: 375 RVA: 0x000064A7 File Offset: 0x000046A7
		public static bool PlayerIsSafeOnMap
		{
			get
			{
				return !TutorialHelper.IsActiveVillageRaidGameMenuOpen;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000178 RID: 376 RVA: 0x000064B4 File Offset: 0x000046B4
		public static bool IsCurrentTownHaveDoableCraftingOrder
		{
			get
			{
				ICraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
				CraftingCampaignBehavior.CraftingOrderSlots craftingOrderSlots;
				if (campaignBehavior == null)
				{
					craftingOrderSlots = null;
				}
				else
				{
					IReadOnlyDictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> craftingOrders = campaignBehavior.CraftingOrders;
					Settlement currentSettlement = Settlement.CurrentSettlement;
					craftingOrderSlots = craftingOrders[(currentSettlement != null) ? currentSettlement.Town : null];
				}
				CraftingCampaignBehavior.CraftingOrderSlots craftingOrderSlots2 = craftingOrderSlots;
				List<CraftingOrder> list;
				if (craftingOrderSlots2 == null)
				{
					list = null;
				}
				else
				{
					list = craftingOrderSlots2.Slots.Where((CraftingOrder x) => x != null).ToList<CraftingOrder>();
				}
				List<CraftingOrder> list2 = list;
				PartyBase mainParty = PartyBase.MainParty;
				MBList<TroopRosterElement> mblist = ((mainParty != null) ? mainParty.MemberRoster.GetTroopRoster() : null);
				if (campaignBehavior == null || craftingOrderSlots2 == null || list2 == null || mblist == null)
				{
					return false;
				}
				for (int i = 0; i < mblist.Count; i++)
				{
					TroopRosterElement troopRosterElement = mblist[i];
					if (troopRosterElement.Character.IsHero)
					{
						for (int j = 0; j < list2.Count; j++)
						{
							if (list2[j].IsOrderAvailableForHero(troopRosterElement.Character.HeroObject))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000179 RID: 377 RVA: 0x000065A4 File Offset: 0x000047A4
		public static bool CurrentInventoryScreenIncludesBannerItem
		{
			get
			{
				InventoryState inventoryState;
				if ((inventoryState = Game.Current.GameStateManager.ActiveState as InventoryState) != null)
				{
					InventoryLogic inventoryLogic = inventoryState.InventoryLogic;
					IEnumerable<ItemRosterElement> enumerable = ((inventoryLogic != null) ? inventoryLogic.GetElementsInRoster(0) : null);
					if (enumerable != null)
					{
						foreach (ItemRosterElement itemRosterElement in enumerable)
						{
							if (itemRosterElement.EquipmentElement.Item.IsBannerItem)
							{
								return true;
							}
						}
						return false;
					}
				}
				return false;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00006634 File Offset: 0x00004834
		public static bool PlayerHasUnassignedRolesAndMember
		{
			get
			{
				bool flag = false;
				PartyBase mainParty = PartyBase.MainParty;
				MBList<TroopRosterElement> mblist = ((mainParty != null) ? mainParty.MemberRoster.GetTroopRoster() : null);
				for (int i = 0; i < mblist.Count; i++)
				{
					TroopRosterElement troopRosterElement = mblist[i];
					if (troopRosterElement.Character.IsHero && !troopRosterElement.Character.IsPlayerCharacter && MobileParty.MainParty.GetHeroPerkRole(troopRosterElement.Character.HeroObject) == null)
					{
						flag = true;
						break;
					}
				}
				bool flag2 = MobileParty.MainParty.GetRoleHolder(7) == null || MobileParty.MainParty.GetRoleHolder(8) == null || MobileParty.MainParty.GetRoleHolder(10) == null || MobileParty.MainParty.GetRoleHolder(9) == null;
				return flag && flag2;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600017B RID: 379 RVA: 0x000066EC File Offset: 0x000048EC
		public static bool PlayerCanRecruit
		{
			get
			{
				if (TutorialHelper.PlayerIsInAnySettlement && (TutorialHelper.TownMenuIsOpen || TutorialHelper.VillageMenuIsOpen) && !Hero.MainHero.IsPrisoner && MobileParty.MainParty.MemberRoster.TotalManCount < PartyBase.MainParty.PartySizeLimit)
				{
					foreach (Hero hero in Settlement.CurrentSettlement.Notables)
					{
						int num = 0;
						foreach (CharacterObject characterObject in HeroHelper.GetVolunteerTroopsOfHeroForRecruitment(hero))
						{
							if (characterObject != null && HeroHelper.HeroCanRecruitFromHero(hero, Hero.MainHero, num))
							{
								int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(characterObject, Hero.MainHero, false);
								return Hero.MainHero.Gold >= 5 * troopRecruitmentCost;
							}
							num++;
						}
					}
					return false;
				}
				return false;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00006818 File Offset: 0x00004A18
		public static bool IsKingdomDecisionPanelActiveAndHasOptions
		{
			get
			{
				GauntletKingdomScreen gauntletKingdomScreen = ScreenManager.TopScreen as GauntletKingdomScreen;
				if (gauntletKingdomScreen != null)
				{
					KingdomManagementVM dataSource = gauntletKingdomScreen.DataSource;
					bool? flag;
					if (dataSource == null)
					{
						flag = null;
					}
					else
					{
						KingdomDecisionsVM decision = dataSource.Decision;
						flag = ((decision != null) ? new bool?(decision.IsCurrentDecisionActive) : null);
					}
					bool? flag2 = flag;
					bool flag3 = true;
					if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
					{
						return gauntletKingdomScreen.DataSource.Decision.CurrentDecision.DecisionOptionsList.Count > 0;
					}
				}
				return false;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000689C File Offset: 0x00004A9C
		public static Location CurrentMissionLocation
		{
			get
			{
				ICampaignMission campaignMission = CampaignMission.Current;
				if (campaignMission == null)
				{
					return null;
				}
				return campaignMission.Location;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600017E RID: 382 RVA: 0x000068B0 File Offset: 0x00004AB0
		public static bool BuyingFoodBaseConditions
		{
			get
			{
				if ((TutorialHelper.TownMenuIsOpen || TutorialHelper.VillageMenuIsOpen || TutorialHelper.CurrentContext == 2) && Settlement.CurrentSettlement != null)
				{
					ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("grain");
					if (@object != null)
					{
						ItemRoster itemRoster = Settlement.CurrentSettlement.ItemRoster;
						int num = itemRoster.FindIndexOfItem(@object);
						if (num >= 0)
						{
							int elementUnitCost = itemRoster.GetElementUnitCost(num);
							return Hero.MainHero.Gold >= 5 * elementUnitCost;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00006928 File Offset: 0x00004B28
		public static bool PlayerHasAnyUpgradeableTroop
		{
			get
			{
				foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
				{
					CharacterObject character = troopRosterElement.Character;
					if (!character.IsHero && troopRosterElement.Number > 0)
					{
						for (int i = 0; i < character.UpgradeTargets.Length; i++)
						{
							if (character.GetUpgradeXpCost(PartyBase.MainParty, i) <= troopRosterElement.Xp)
							{
								CharacterObject characterObject = character.UpgradeTargets[i];
								if (characterObject.UpgradeRequiresItemFromCategory == null)
								{
									return true;
								}
								foreach (ItemRosterElement itemRosterElement in MobileParty.MainParty.ItemRoster)
								{
									if (itemRosterElement.EquipmentElement.Item.ItemCategory == characterObject.UpgradeRequiresItemFromCategory && itemRosterElement.Amount > 0)
									{
										return true;
									}
								}
							}
						}
					}
				}
				return false;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00006A5C File Offset: 0x00004C5C
		public static bool PlayerIsInAConversation
		{
			get
			{
				return !Extensions.IsEmpty<CharacterObject>(CharacterObject.ConversationCharacters);
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00006A6C File Offset: 0x00004C6C
		public static bool? IsThereAvailableCompanionInLocation(Location location)
		{
			if (location == null)
			{
				return null;
			}
			return new bool?(location.GetCharacterList().Any((LocationCharacter x) => x.Character.IsHero && x.Character.HeroObject.IsWanderer && !x.Character.HeroObject.IsPlayerCompanion));
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000182 RID: 386 RVA: 0x00006AB5 File Offset: 0x00004CB5
		public static DateTime CurrentTime
		{
			get
			{
				return DateTime.Now;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00006ABC File Offset: 0x00004CBC
		public static int MinimumGoldForCompanion
		{
			get
			{
				return 999;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00006AC3 File Offset: 0x00004CC3
		public static float MaximumSpeedForPartyForSpeedTutorial
		{
			get
			{
				return 4f;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00006ACA File Offset: 0x00004CCA
		public static float MaxCohesionForCohesionTutorial
		{
			get
			{
				return 30f;
			}
		}
	}
}
