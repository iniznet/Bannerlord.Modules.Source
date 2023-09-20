using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Siege
{
	// Token: 0x0200028C RID: 652
	public static class PlayerSiege
	{
		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x06002269 RID: 8809 RVA: 0x000920A0 File Offset: 0x000902A0
		public static SiegeEvent PlayerSiegeEvent
		{
			get
			{
				SiegeEvent siegeEvent;
				if ((siegeEvent = MobileParty.MainParty.SiegeEvent) == null)
				{
					Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
					if (currentSettlement == null)
					{
						return null;
					}
					siegeEvent = currentSettlement.SiegeEvent;
				}
				return siegeEvent;
			}
		}

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x0600226A RID: 8810 RVA: 0x000920C5 File Offset: 0x000902C5
		public static Settlement BesiegedSettlement
		{
			get
			{
				SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
				if (playerSiegeEvent == null)
				{
					return null;
				}
				return playerSiegeEvent.BesiegedSettlement;
			}
		}

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x0600226B RID: 8811 RVA: 0x000920D7 File Offset: 0x000902D7
		public static BattleSideEnum PlayerSide
		{
			get
			{
				if (MobileParty.MainParty.BesiegerCamp == null)
				{
					return BattleSideEnum.Defender;
				}
				return BattleSideEnum.Attacker;
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x0600226C RID: 8812 RVA: 0x000920E8 File Offset: 0x000902E8
		public static bool IsRebellion
		{
			get
			{
				return PlayerSiege.BesiegedSettlement != null && PlayerSiege.BesiegedSettlement.IsUnderRebellionAttack();
			}
		}

		// Token: 0x0600226D RID: 8813 RVA: 0x000920FD File Offset: 0x000902FD
		private static void SetPlayerSiegeEvent()
		{
		}

		// Token: 0x0600226E RID: 8814 RVA: 0x000920FF File Offset: 0x000902FF
		public static void StartSiegePreparation()
		{
			if (Campaign.Current.CurrentMenuContext != null)
			{
				GameMenu.ExitToLast();
			}
			GameMenu.ActivateGameMenu("menu_siege_strategies");
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x0009211C File Offset: 0x0009031C
		public static void OnSiegeEventFinalized(bool besiegerPartyDefeated)
		{
			MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
			if (PlayerSiege.IsRebellion)
			{
				if (mapState != null && mapState.AtMenu)
				{
					GameMenu.ExitToLast();
					return;
				}
			}
			else if (PlayerSiege.PlayerSide == BattleSideEnum.Defender && !PlayerSiege.IsRebellion)
			{
				if (Settlement.CurrentSettlement != null)
				{
					if (mapState != null && !mapState.AtMenu)
					{
						GameMenu.ActivateGameMenu(besiegerPartyDefeated ? "siege_attacker_defeated" : "siege_attacker_left");
						return;
					}
					GameMenu.SwitchToMenu(besiegerPartyDefeated ? "siege_attacker_defeated" : "siege_attacker_left");
					return;
				}
			}
			else if (Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Army != null && Hero.MainHero.PartyBelongedTo.Army.LeaderParty != MobileParty.MainParty)
			{
				if (MobileParty.MainParty.CurrentSettlement != null)
				{
					LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
				}
				if (PlayerEncounter.Battle == null)
				{
					if (mapState != null)
					{
						if (mapState.AtMenu)
						{
							GameMenu.SwitchToMenu("army_wait");
							return;
						}
						GameMenu.ActivateGameMenu("army_wait");
						return;
					}
					else
					{
						Campaign.Current.GameMenuManager.SetNextMenu("army_wait");
					}
				}
			}
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x00092234 File Offset: 0x00090434
		public static void StartPlayerSiege(BattleSideEnum playerSide, bool isSimulation = false, Settlement settlement = null)
		{
			if (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				MobileParty.MainParty.Ai.SetMoveModeHold();
			}
			PlayerSiege.SetPlayerSiegeEvent();
			if (!isSimulation)
			{
				GameState gameState = Game.Current.GameStateManager.GameStates.FirstOrDefault((GameState s) => s is MapState);
				if (gameState != null)
				{
					MapState mapState = gameState as MapState;
					if (mapState != null)
					{
						mapState.OnPlayerSiegeActivated();
					}
				}
			}
			CampaignEventDispatcher.Instance.OnPlayerSiegeStarted();
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x000922CC File Offset: 0x000904CC
		public static void ClosePlayerSiege()
		{
			if (PlayerSiege.PlayerSiegeEvent == null)
			{
				return;
			}
			PlayerSiege.BesiegedSettlement.Party.Visuals.SetMapIconAsDirty();
			Campaign.Current.autoEnterTown = null;
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				MobileParty.MainParty.Army.AiBehaviorObject = null;
			}
			MobileParty.MainParty.Ai.SetMoveModeHold();
			GameState gameState = Game.Current.GameStateManager.GameStates.FirstOrDefault((GameState s) => s is MapState);
			if (gameState != null)
			{
				MapState mapState = gameState as MapState;
				if (mapState == null)
				{
					return;
				}
				mapState.OnPlayerSiegeDeactivated();
			}
		}

		// Token: 0x06002272 RID: 8818 RVA: 0x00092388 File Offset: 0x00090588
		public static void StartSiegeMission(bool isSallyOut = false, Settlement settlement = null)
		{
			Settlement besiegedSettlement = PlayerSiege.BesiegedSettlement;
			Settlement.SiegeState currentSiegeState = besiegedSettlement.CurrentSiegeState;
			if (currentSiegeState != Settlement.SiegeState.OnTheWalls)
			{
				if (currentSiegeState != Settlement.SiegeState.Invalid)
				{
					return;
				}
				Debug.FailedAssert("Siege state is invalid!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Siege\\PlayerSiege.cs", "StartSiegeMission", 202);
				return;
			}
			else
			{
				List<MissionSiegeWeapon> preparedAndActiveSiegeEngines = PlayerSiege.PlayerSiegeEvent.GetPreparedAndActiveSiegeEngines(PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker));
				List<MissionSiegeWeapon> preparedAndActiveSiegeEngines2 = PlayerSiege.PlayerSiegeEvent.GetPreparedAndActiveSiegeEngines(PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender));
				bool flag = preparedAndActiveSiegeEngines.Exists((MissionSiegeWeapon data) => data.Type == DefaultSiegeEngineTypes.SiegeTower);
				int num = besiegedSettlement.Town.GetWallLevel();
				string text = besiegedSettlement.LocationComplex.GetLocationWithId("center").GetSceneName(num);
				if (CampaignSiegeTestStatic.IsSiegeTestBuild)
				{
					text = CampaignSiegeTestStatic.SiegeTestSceneName;
					num = CampaignSiegeTestStatic.SiegeLevel;
				}
				float num2 = besiegedSettlement.SettlementTotalWallHitPoints / besiegedSettlement.MaxWallHitPoints;
				if (isSallyOut)
				{
					CampaignMission.OpenSiegeMissionWithDeployment(text, besiegedSettlement.SettlementWallSectionHitPointsRatioList.ToArray(), flag, preparedAndActiveSiegeEngines, preparedAndActiveSiegeEngines2, PlayerEncounter.Current.PlayerSide == BattleSideEnum.Attacker, num, true, false);
					return;
				}
				CampaignMission.OpenSiegeMissionWithDeployment(text, besiegedSettlement.SettlementWallSectionHitPointsRatioList.ToArray(), flag, preparedAndActiveSiegeEngines, preparedAndActiveSiegeEngines2, PlayerEncounter.Current.PlayerSide == BattleSideEnum.Attacker, num, false, false);
				return;
			}
		}
	}
}
