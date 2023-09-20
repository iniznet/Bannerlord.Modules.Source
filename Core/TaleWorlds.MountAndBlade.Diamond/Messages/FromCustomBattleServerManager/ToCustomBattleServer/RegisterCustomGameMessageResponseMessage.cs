using System;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[Serializable]
	public class RegisterCustomGameMessageResponseMessage : FunctionResult
	{
		public bool ShouldReportActivities { get; private set; }

		public RegisterCustomGameMessageResponseMessage(bool shouldReportActivities)
		{
			this.ShouldReportActivities = shouldReportActivities;
		}
	}
}
