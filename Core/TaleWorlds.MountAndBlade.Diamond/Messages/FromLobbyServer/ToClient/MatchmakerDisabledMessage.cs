using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class MatchmakerDisabledMessage : Message
	{
		[JsonProperty]
		public int RemainingTime { get; private set; }

		public MatchmakerDisabledMessage()
		{
		}

		public MatchmakerDisabledMessage(int remainingTime)
		{
			this.RemainingTime = remainingTime;
		}
	}
}
