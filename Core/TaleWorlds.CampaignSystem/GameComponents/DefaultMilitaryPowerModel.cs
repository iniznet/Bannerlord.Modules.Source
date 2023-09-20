using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200011C RID: 284
	public class DefaultMilitaryPowerModel : MilitaryPowerModel
	{
		// Token: 0x06001637 RID: 5687 RVA: 0x0006A148 File Offset: 0x00068348
		public override float GetTroopPowerBasedOnContext(CharacterObject troop, MapEvent.BattleTypes battleType = MapEvent.BattleTypes.None, BattleSideEnum battleSideEnum = BattleSideEnum.None, bool isSimulation = false)
		{
			MilitaryPowerModel.PowerCalculationContext powerCalculationContext = DefaultMilitaryPowerModel.DetermineContext(battleType, battleSideEnum, isSimulation);
			return DefaultMilitaryPowerModel.GetTroopPowerBasedOnContextInternal(troop, powerCalculationContext);
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x0006A166 File Offset: 0x00068366
		public override float GetTroopPowerToCalculateSecurity(CharacterObject troop)
		{
			return DefaultMilitaryPowerModel.GetTroopPowerBasedOnContextInternal(troop, MilitaryPowerModel.PowerCalculationContext.ToCalculateSettlementSecurity);
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x0006A170 File Offset: 0x00068370
		private static float GetTroopPowerBasedOnContextInternal(CharacterObject troop, MilitaryPowerModel.PowerCalculationContext context)
		{
			if (context - MilitaryPowerModel.PowerCalculationContext.SiegeSimulationAsAttacker <= 1)
			{
				int num = (troop.IsHero ? (troop.HeroObject.Level / 4 + 1) : troop.Tier);
				return (float)((2 + num) * (10 + num)) * 0.02f * (troop.IsHero ? 1.5f : 1f);
			}
			return DefaultMilitaryPowerModel.DefaultTroopPower(troop);
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x0006A1D0 File Offset: 0x000683D0
		private static float DefaultTroopPower(CharacterObject troop)
		{
			int num = (troop.IsHero ? (troop.HeroObject.Level / 4 + 1) : troop.Tier);
			return (float)((2 + num) * (10 + num)) * 0.02f * (troop.IsHero ? 1.5f : (troop.IsMounted ? 1.2f : 1f));
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x0006A230 File Offset: 0x00068430
		private static MilitaryPowerModel.PowerCalculationContext DetermineContext(MapEvent.BattleTypes battleType, BattleSideEnum battleSideEnum, bool isSimulation)
		{
			MilitaryPowerModel.PowerCalculationContext powerCalculationContext = MilitaryPowerModel.PowerCalculationContext.Default;
			switch (battleType)
			{
			case MapEvent.BattleTypes.FieldBattle:
				powerCalculationContext = (isSimulation ? MilitaryPowerModel.PowerCalculationContext.FieldBattleSimulation : MilitaryPowerModel.PowerCalculationContext.FieldBattle);
				break;
			case MapEvent.BattleTypes.Raid:
			case MapEvent.BattleTypes.IsForcingVolunteers:
			case MapEvent.BattleTypes.IsForcingSupplies:
				if (battleSideEnum == BattleSideEnum.Attacker)
				{
					powerCalculationContext = (isSimulation ? MilitaryPowerModel.PowerCalculationContext.RaidSimulationAsAttacker : MilitaryPowerModel.PowerCalculationContext.RaidAsAttacker);
				}
				else
				{
					powerCalculationContext = (isSimulation ? MilitaryPowerModel.PowerCalculationContext.RaidSimulationAsDefender : MilitaryPowerModel.PowerCalculationContext.RaidAsDefender);
				}
				break;
			case MapEvent.BattleTypes.Siege:
			case MapEvent.BattleTypes.SallyOut:
			case MapEvent.BattleTypes.SiegeOutside:
				if (battleSideEnum == BattleSideEnum.Attacker)
				{
					powerCalculationContext = (isSimulation ? MilitaryPowerModel.PowerCalculationContext.SiegeSimulationAsAttacker : MilitaryPowerModel.PowerCalculationContext.SiegeAsAttacker);
				}
				else
				{
					powerCalculationContext = (isSimulation ? MilitaryPowerModel.PowerCalculationContext.SiegeSimulationAsDefender : MilitaryPowerModel.PowerCalculationContext.SiegeAsDefender);
				}
				break;
			case MapEvent.BattleTypes.Hideout:
				powerCalculationContext = MilitaryPowerModel.PowerCalculationContext.Hideout;
				break;
			}
			return powerCalculationContext;
		}
	}
}
