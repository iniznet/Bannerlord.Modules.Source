﻿using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class StartBattleAction
	{
		private static void ApplyInternal(PartyBase attackerParty, PartyBase defenderParty, object subject, StartBattleAction.StartBattleActionDetails detail)
		{
			if (defenderParty.MapEvent == null)
			{
				switch (detail)
				{
				case StartBattleAction.StartBattleActionDetails.Battle:
					Campaign.Current.MapEventManager.StartBattleMapEvent(attackerParty, defenderParty);
					break;
				case StartBattleAction.StartBattleActionDetails.Raid:
					RaidEventComponent.CreateRaidEvent(attackerParty, defenderParty);
					break;
				case StartBattleAction.StartBattleActionDetails.Siege:
					Campaign.Current.MapEventManager.StartSiegeMapEvent(attackerParty, defenderParty);
					break;
				case StartBattleAction.StartBattleActionDetails.Hideout:
					Campaign.Current.MapEventManager.StartHideoutMapEvent(attackerParty, defenderParty);
					break;
				case StartBattleAction.StartBattleActionDetails.SallyOut:
					Campaign.Current.MapEventManager.StartSallyOutMapEvent(attackerParty, defenderParty);
					break;
				case StartBattleAction.StartBattleActionDetails.SiegeOutside:
					Campaign.Current.MapEventManager.StartSiegeOutsideMapEvent(attackerParty, defenderParty);
					break;
				}
			}
			else
			{
				BattleSideEnum battleSideEnum = BattleSideEnum.Attacker;
				if (defenderParty.Side == BattleSideEnum.Attacker)
				{
					battleSideEnum = BattleSideEnum.Defender;
				}
				attackerParty.MapEventSide = defenderParty.MapEvent.GetMapEventSide(battleSideEnum);
			}
			if (defenderParty.MapEvent.IsPlayerMapEvent && !defenderParty.MapEvent.IsSallyOut && PlayerEncounter.Current != null && MobileParty.MainParty.CurrentSettlement != null)
			{
				PlayerEncounter.Current.InterruptEncounter("encounter_interrupted");
			}
			MobileParty mobileParty = attackerParty.MobileParty;
			bool flag;
			if (((mobileParty != null) ? mobileParty.Army : null) != null)
			{
				MobileParty mobileParty2 = attackerParty.MobileParty;
				if (((mobileParty2 != null) ? mobileParty2.Army.LeaderParty : null) != attackerParty.MobileParty)
				{
					flag = false;
					goto IL_165;
				}
			}
			MobileParty mobileParty3 = defenderParty.MobileParty;
			if (((mobileParty3 != null) ? mobileParty3.Army : null) != null)
			{
				MobileParty mobileParty4 = defenderParty.MobileParty;
				flag = ((mobileParty4 != null) ? mobileParty4.Army.LeaderParty : null) == defenderParty.MobileParty;
			}
			else
			{
				flag = true;
			}
			IL_165:
			bool flag2 = flag;
			if (flag2 && defenderParty.IsSettlement && defenderParty.MapEvent != null && defenderParty.MapEvent.DefenderSide.Parties.Count > 1)
			{
				flag2 = false;
			}
			CampaignEventDispatcher.Instance.OnStartBattle(attackerParty, defenderParty, subject, flag2);
		}

		public static void Apply(PartyBase attackerParty, PartyBase defenderParty)
		{
			StartBattleAction.StartBattleActionDetails startBattleActionDetails = StartBattleAction.StartBattleActionDetails.None;
			object obj = null;
			Settlement settlement;
			if (defenderParty.MapEvent == null)
			{
				if (attackerParty.MobileParty != null && attackerParty.MobileParty.IsGarrison)
				{
					settlement = attackerParty.MobileParty.CurrentSettlement;
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.SallyOut;
				}
				else if (attackerParty.MobileParty.CurrentSettlement != null)
				{
					settlement = attackerParty.MobileParty.CurrentSettlement;
				}
				else if (defenderParty.MobileParty.CurrentSettlement != null)
				{
					settlement = defenderParty.MobileParty.CurrentSettlement;
				}
				else if (attackerParty.MobileParty.BesiegedSettlement != null)
				{
					settlement = attackerParty.MobileParty.BesiegedSettlement;
					if (!defenderParty.IsSettlement)
					{
						startBattleActionDetails = StartBattleAction.StartBattleActionDetails.SiegeOutside;
					}
				}
				else if (defenderParty.MobileParty.BesiegedSettlement != null)
				{
					settlement = defenderParty.MobileParty.BesiegedSettlement;
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.SiegeOutside;
				}
				else
				{
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.Battle;
					settlement = null;
				}
				if (settlement != null && startBattleActionDetails == StartBattleAction.StartBattleActionDetails.None)
				{
					if (settlement.IsTown)
					{
						startBattleActionDetails = StartBattleAction.StartBattleActionDetails.Siege;
					}
					else if (settlement.IsHideout)
					{
						startBattleActionDetails = StartBattleAction.StartBattleActionDetails.Hideout;
					}
					else if (settlement.IsVillage)
					{
						Debug.FailedAssert("Since villages can be raided or sieged, this block cannot decide if the battle is raid or siege.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\StartBattleAction.cs", "Apply", 147);
					}
					else
					{
						Debug.FailedAssert("Missing settlement type in StartBattleAction.GetGameAction", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\StartBattleAction.cs", "Apply", 151);
					}
				}
			}
			else
			{
				if (defenderParty.MapEvent.IsFieldBattle)
				{
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.Battle;
				}
				else if (defenderParty.MapEvent.IsRaid)
				{
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.Raid;
				}
				else if (defenderParty.MapEvent.IsSiegeAssault)
				{
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.Siege;
				}
				else if (defenderParty.MapEvent.IsSallyOut)
				{
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.SallyOut;
				}
				else if (defenderParty.MapEvent.IsSiegeOutside)
				{
					startBattleActionDetails = StartBattleAction.StartBattleActionDetails.SiegeOutside;
				}
				else
				{
					Debug.FailedAssert("Missing mapEventType?", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\StartBattleAction.cs", "Apply", 179);
				}
				settlement = defenderParty.MapEvent.MapEventSettlement;
			}
			obj = obj ?? settlement;
			StartBattleAction.ApplyInternal(attackerParty, defenderParty, obj, startBattleActionDetails);
		}

		public static void ApplyStartBattle(MobileParty attackerParty, MobileParty defenderParty)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, defenderParty.Party, null, StartBattleAction.StartBattleActionDetails.Battle);
		}

		public static void ApplyStartRaid(MobileParty attackerParty, Settlement settlement)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, settlement.Party, settlement, StartBattleAction.StartBattleActionDetails.Raid);
		}

		public static void ApplyStartSallyOut(Settlement settlement, MobileParty defenderParty)
		{
			StartBattleAction.ApplyInternal(settlement.Town.GarrisonParty.Party, defenderParty.Party, settlement, StartBattleAction.StartBattleActionDetails.SallyOut);
		}

		public static void ApplyStartAssaultAgainstWalls(MobileParty attackerParty, Settlement settlement)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, settlement.Party, settlement, StartBattleAction.StartBattleActionDetails.Siege);
		}

		private enum StartBattleActionDetails
		{
			None,
			Battle,
			Raid,
			Siege,
			Hideout,
			SallyOut,
			SiegeOutside
		}
	}
}