using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PremadeGameEligibilityStatusMessage : Message
	{
		public PremadeGameType[] EligibleGameTypes { get; private set; }

		public PremadeGameEligibilityStatusMessage(PremadeGameType[] eligibleGameTypes)
		{
			this.EligibleGameTypes = eligibleGameTypes;
		}
	}
}
