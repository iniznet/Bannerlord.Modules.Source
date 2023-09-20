using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class BattleReadyResponseMessage : FunctionResult
	{
		[JsonProperty]
		public bool ShouldReportActivities { get; private set; }

		public BattleReadyResponseMessage()
		{
		}

		public BattleReadyResponseMessage(bool shouldReportActivities)
		{
			this.ShouldReportActivities = shouldReportActivities;
		}
	}
}
