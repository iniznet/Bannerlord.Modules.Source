using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[Serializable]
	public class RegisterCustomGameMessageResponseMessage : FunctionResult
	{
		[JsonProperty]
		public bool ShouldReportActivities { get; private set; }

		public RegisterCustomGameMessageResponseMessage()
		{
		}

		public RegisterCustomGameMessageResponseMessage(bool shouldReportActivities)
		{
			this.ShouldReportActivities = shouldReportActivities;
		}
	}
}
