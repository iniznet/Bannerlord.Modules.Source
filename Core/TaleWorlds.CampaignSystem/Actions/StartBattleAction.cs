using System;
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
		private static void ApplyInternal(PartyBase attackerParty, PartyBase defenderParty, object subject, MapEvent.BattleTypes battleType)
		{
			if (defenderParty.MapEvent == null)
			{
				Campaign.Current.Models.EncounterModel.CreateMapEventComponentForEncounter(attackerParty, defenderParty, battleType);
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
					goto IL_E9;
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
			IL_E9:
			bool flag2 = flag;
			if (flag2 && defenderParty.IsSettlement && defenderParty.MapEvent != null && defenderParty.MapEvent.DefenderSide.Parties.Count > 1)
			{
				flag2 = false;
			}
			CampaignEventDispatcher.Instance.OnStartBattle(attackerParty, defenderParty, subject, flag2);
		}

		public static void Apply(PartyBase attackerParty, PartyBase defenderParty)
		{
			MapEvent.BattleTypes battleTypes = MapEvent.BattleTypes.None;
			object obj = null;
			Settlement settlement;
			if (defenderParty.MapEvent == null)
			{
				if (attackerParty.MobileParty != null && attackerParty.MobileParty.IsGarrison)
				{
					settlement = attackerParty.MobileParty.CurrentSettlement;
					battleTypes = MapEvent.BattleTypes.SallyOut;
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
						battleTypes = MapEvent.BattleTypes.SiegeOutside;
					}
				}
				else if (defenderParty.MobileParty.BesiegedSettlement != null)
				{
					settlement = defenderParty.MobileParty.BesiegedSettlement;
					battleTypes = MapEvent.BattleTypes.SiegeOutside;
				}
				else
				{
					battleTypes = MapEvent.BattleTypes.FieldBattle;
					settlement = null;
				}
				if (settlement != null && battleTypes == MapEvent.BattleTypes.None)
				{
					if (settlement.IsTown)
					{
						battleTypes = MapEvent.BattleTypes.Siege;
					}
					else if (settlement.IsHideout)
					{
						battleTypes = MapEvent.BattleTypes.Hideout;
					}
					else if (settlement.IsVillage)
					{
						Debug.FailedAssert("Since villages can be raided or sieged, this block cannot decide if the battle is raid or siege.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\StartBattleAction.cs", "Apply", 116);
					}
					else
					{
						Debug.FailedAssert("Missing settlement type in StartBattleAction.GetGameAction", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\StartBattleAction.cs", "Apply", 120);
					}
				}
			}
			else
			{
				if (defenderParty.MapEvent.IsFieldBattle)
				{
					battleTypes = MapEvent.BattleTypes.FieldBattle;
				}
				else if (defenderParty.MapEvent.IsRaid)
				{
					battleTypes = MapEvent.BattleTypes.Raid;
				}
				else if (defenderParty.MapEvent.IsSiegeAssault)
				{
					battleTypes = MapEvent.BattleTypes.Siege;
				}
				else if (defenderParty.MapEvent.IsSallyOut)
				{
					battleTypes = MapEvent.BattleTypes.SallyOut;
				}
				else if (defenderParty.MapEvent.IsSiegeOutside)
				{
					battleTypes = MapEvent.BattleTypes.SiegeOutside;
				}
				else
				{
					Debug.FailedAssert("Missing mapEventType?", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\StartBattleAction.cs", "Apply", 148);
				}
				settlement = defenderParty.MapEvent.MapEventSettlement;
			}
			obj = obj ?? settlement;
			StartBattleAction.ApplyInternal(attackerParty, defenderParty, obj, battleTypes);
		}

		public static void ApplyStartBattle(MobileParty attackerParty, MobileParty defenderParty)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, defenderParty.Party, null, MapEvent.BattleTypes.FieldBattle);
		}

		public static void ApplyStartRaid(MobileParty attackerParty, Settlement settlement)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, settlement.Party, settlement, MapEvent.BattleTypes.Raid);
		}

		public static void ApplyStartSallyOut(Settlement settlement, MobileParty defenderParty)
		{
			StartBattleAction.ApplyInternal(settlement.Town.GarrisonParty.Party, defenderParty.Party, settlement, MapEvent.BattleTypes.SallyOut);
		}

		public static void ApplyStartAssaultAgainstWalls(MobileParty attackerParty, Settlement settlement)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, settlement.Party, settlement, MapEvent.BattleTypes.Siege);
		}
	}
}
