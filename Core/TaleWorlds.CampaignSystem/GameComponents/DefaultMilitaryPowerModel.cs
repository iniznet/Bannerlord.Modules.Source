using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultMilitaryPowerModel : MilitaryPowerModel
	{
		public override float GetTroopPowerBasedOnContext(CharacterObject troop, MapEvent.BattleTypes battleType = MapEvent.BattleTypes.None, BattleSideEnum battleSideEnum = BattleSideEnum.None, bool isSimulation = false)
		{
			MilitaryPowerModel.PowerCalculationContext powerCalculationContext = DefaultMilitaryPowerModel.DetermineContext(battleType, battleSideEnum, isSimulation);
			return DefaultMilitaryPowerModel.GetTroopPowerBasedOnContextInternal(troop, powerCalculationContext);
		}

		public override float GetTroopPowerToCalculateSecurity(CharacterObject troop)
		{
			return DefaultMilitaryPowerModel.GetTroopPowerBasedOnContextInternal(troop, MilitaryPowerModel.PowerCalculationContext.ToCalculateSettlementSecurity);
		}

		private static float GetTroopPowerBasedOnContextInternal(CharacterObject troop, MilitaryPowerModel.PowerCalculationContext context)
		{
			if (context - MilitaryPowerModel.PowerCalculationContext.SiegeSimulationAsAttacker <= 1)
			{
				int num = (troop.IsHero ? (troop.HeroObject.Level / 4 + 1) : troop.Tier);
				return (float)((2 + num) * (10 + num)) * 0.02f * (troop.IsHero ? 1.5f : 1f);
			}
			return DefaultMilitaryPowerModel.DefaultTroopPower(troop);
		}

		private static float DefaultTroopPower(CharacterObject troop)
		{
			int num = (troop.IsHero ? (troop.HeroObject.Level / 4 + 1) : troop.Tier);
			return (float)((2 + num) * (10 + num)) * 0.02f * (troop.IsHero ? 1.5f : (troop.IsMounted ? 1.2f : 1f));
		}

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
