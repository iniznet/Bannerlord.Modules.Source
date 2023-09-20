using System;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class RequestMaxAllowedPriorityResponse : FunctionResult
	{
		public sbyte Priority { get; private set; }

		public RequestMaxAllowedPriorityResponse(sbyte priority)
		{
			this.Priority = priority;
		}
	}
}
