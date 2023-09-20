using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000457 RID: 1111
	public static class StartBattleAction
	{
		// Token: 0x06003F59 RID: 16217 RVA: 0x0012F848 File Offset: 0x0012DA48
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

		// Token: 0x06003F5A RID: 16218 RVA: 0x0012F9F8 File Offset: 0x0012DBF8
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

		// Token: 0x06003F5B RID: 16219 RVA: 0x0012FBAA File Offset: 0x0012DDAA
		public static void ApplyStartBattle(MobileParty attackerParty, MobileParty defenderParty)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, defenderParty.Party, null, StartBattleAction.StartBattleActionDetails.Battle);
		}

		// Token: 0x06003F5C RID: 16220 RVA: 0x0012FBBF File Offset: 0x0012DDBF
		public static void ApplyStartRaid(MobileParty attackerParty, Settlement settlement)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, settlement.Party, settlement, StartBattleAction.StartBattleActionDetails.Raid);
		}

		// Token: 0x06003F5D RID: 16221 RVA: 0x0012FBD4 File Offset: 0x0012DDD4
		public static void ApplyStartSallyOut(Settlement settlement, MobileParty defenderParty)
		{
			StartBattleAction.ApplyInternal(settlement.Town.GarrisonParty.Party, defenderParty.Party, settlement, StartBattleAction.StartBattleActionDetails.SallyOut);
		}

		// Token: 0x06003F5E RID: 16222 RVA: 0x0012FBF3 File Offset: 0x0012DDF3
		public static void ApplyStartAssaultAgainstWalls(MobileParty attackerParty, Settlement settlement)
		{
			StartBattleAction.ApplyInternal(attackerParty.Party, settlement.Party, settlement, StartBattleAction.StartBattleActionDetails.Siege);
		}

		// Token: 0x0200076C RID: 1900
		private enum StartBattleActionDetails
		{
			// Token: 0x04001E85 RID: 7813
			None,
			// Token: 0x04001E86 RID: 7814
			Battle,
			// Token: 0x04001E87 RID: 7815
			Raid,
			// Token: 0x04001E88 RID: 7816
			Siege,
			// Token: 0x04001E89 RID: 7817
			Hideout,
			// Token: 0x04001E8A RID: 7818
			SallyOut,
			// Token: 0x04001E8B RID: 7819
			SiegeOutside
		}
	}
}
