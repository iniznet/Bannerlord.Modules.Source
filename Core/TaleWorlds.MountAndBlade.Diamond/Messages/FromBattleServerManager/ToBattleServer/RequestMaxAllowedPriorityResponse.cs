using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class RequestMaxAllowedPriorityResponse : FunctionResult
	{
		[JsonProperty]
		public sbyte Priority { get; private set; }

		public RequestMaxAllowedPriorityResponse()
		{
		}

		public RequestMaxAllowedPriorityResponse(sbyte priority)
		{
			this.Priority = priority;
		}
	}
}
