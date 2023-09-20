using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class MilitaryPowerModel : GameModel
	{
		public abstract float GetTroopPowerBasedOnContext(CharacterObject troop, MapEvent.BattleTypes battleType = MapEvent.BattleTypes.None, BattleSideEnum battleSideEnum = BattleSideEnum.None, bool isSimulation = false);

		public abstract float GetTroopPowerToCalculateSecurity(CharacterObject troop);

		public enum PowerCalculationContext
		{
			FieldBattle,
			FieldBattleSimulation,
			RaidAsAttacker,
			RaidAsDefender,
			RaidSimulationAsAttacker,
			RaidSimulationAsDefender,
			SiegeSimulationAsAttacker,
			SiegeSimulationAsDefender,
			SiegeAsAttacker,
			SiegeAsDefender,
			ToCalculateSettlementSecurity,
			Hideout,
			Default
		}
	}
}
