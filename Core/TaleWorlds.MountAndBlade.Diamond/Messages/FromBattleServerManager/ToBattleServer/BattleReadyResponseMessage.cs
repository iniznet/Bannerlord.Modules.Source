using System;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class BattleReadyResponseMessage : FunctionResult
	{
		public bool ShouldReportActivities { get; private set; }

		public BattleReadyResponseMessage(bool shouldReportActivities)
		{
			this.ShouldReportActivities = shouldReportActivities;
		}
	}
}
