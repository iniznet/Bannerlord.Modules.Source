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
	public static class TutorialHelper
	{
		public static bool PlayerIsInAnySettlement
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && (currentSettlement.IsFortification || currentSettlement.IsVillage);
			}
		}

		public static bool PlayerIsInAnyVillage
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && currentSettlement.IsVillage;
			}
		}

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

		public static bool IsCharacterPopUpWindowOpen
		{
			get
			{
				return GauntletTutorialSystem.Current.IsCharacterPortraitPopupOpen;
			}
		}

		public static EncyclopediaPages CurrentEncyclopediaPage
		{
			get
			{
				return GauntletTutorialSystem.Current.CurrentEncyclopediaPageContext;
			}
		}

		public static TutorialContexts CurrentContext
		{
			get
			{
				return GauntletTutorialSystem.Current.CurrentContext;
			}
		}

		public static bool PlayerIsInNonEnemyTown
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && currentSettlement.IsTown && !FactionManager.IsAtWarAgainstFaction(currentSettlement.MapFaction, MobileParty.MainParty.MapFaction);
			}
		}

		public static string ActiveVillageRaidGameMenuID
		{
			get
			{
				return "raiding_village";
			}
		}

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

		public static bool VillageMenuIsOpen
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return currentSettlement != null && currentSettlement.IsVillage;
			}
		}

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

		public static bool IsPlayerInABattleMission
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.Mode == 2;
			}
		}

		public static bool IsOrderOfBattleOpenAndReady
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.Mode == 6 && !LoadingWindow.IsLoadingWindowActive;
			}
		}

		public static bool CanPlayerAssignHimselfToFormation
		{
			get
			{
				if (!TutorialHelper.IsOrderOfBattleOpenAndReady)
				{
					return false;
				}
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return false;
				}
				return mission.PlayerTeam.FormationsIncludingEmpty.Any((Formation x) => x.CountOfUnits > 0 && x.Captain == null);
			}
		}

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

		public static bool IsPlayerInAHideoutBattleMission
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.HasMissionBehavior<HideoutMissionController>();
			}
		}

		public static IList<Location> GetMenuLocations
		{
			get
			{
				return Campaign.Current.GameMenuManager.MenuLocations;
			}
		}

		public static bool PlayerIsSafeOnMap
		{
			get
			{
				return !TutorialHelper.IsActiveVillageRaidGameMenuOpen;
			}
		}

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

		public static int BuyGrainAmount
		{
			get
			{
				return 2;
			}
		}

		public static int RecruitTroopAmount
		{
			get
			{
				return 4;
			}
		}

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

		public static bool PlayerIsInAConversation
		{
			get
			{
				return !Extensions.IsEmpty<CharacterObject>(CharacterObject.ConversationCharacters);
			}
		}

		public static bool? IsThereAvailableCompanionInLocation(Location location)
		{
			if (location == null)
			{
				return null;
			}
			return new bool?(location.GetCharacterList().Any((LocationCharacter x) => x.Character.IsHero && x.Character.HeroObject.IsWanderer && !x.Character.HeroObject.IsPlayerCompanion));
		}

		public static DateTime CurrentTime
		{
			get
			{
				return DateTime.Now;
			}
		}

		public static int MinimumGoldForCompanion
		{
			get
			{
				return 999;
			}
		}

		public static float MaximumSpeedForPartyForSpeedTutorial
		{
			get
			{
				return 4f;
			}
		}

		public static float MaxCohesionForCohesionTutorial
		{
			get
			{
				return 30f;
			}
		}
	}
}
