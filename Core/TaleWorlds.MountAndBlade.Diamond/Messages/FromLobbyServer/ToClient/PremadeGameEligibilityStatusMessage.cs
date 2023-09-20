using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PremadeGameEligibilityStatusMessage : Message
	{
		[JsonProperty]
		public PremadeGameType[] EligibleGameTypes { get; private set; }

		public PremadeGameEligibilityStatusMessage()
		{
		}

		public PremadeGameEligibilityStatusMessage(PremadeGameType[] eligibleGameTypes)
		{
			this.EligibleGameTypes = eligibleGameTypes;
		}
	}
}
