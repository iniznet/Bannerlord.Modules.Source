using System;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class SiegeStrategyActionModel : GameModel
	{
		public abstract void GetLogicalActionForStrategy(ISiegeEventSide side, out SiegeStrategyActionModel.SiegeAction siegeAction, out SiegeEngineType siegeEngineType, out int deploymentIndex, out int reserveIndex);

		public enum SiegeAction
		{
			ConstructNewSiegeEngine,
			DeploySiegeEngineFromReserve,
			MoveSiegeEngineToReserve,
			RemoveDeployedSiegeEngine,
			Hold
		}
	}
}
