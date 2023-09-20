using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class MatchmakerDisabledMessage : Message
	{
		public int RemainingTime { get; }

		public MatchmakerDisabledMessage(int remainingTime)
		{
			this.RemainingTime = remainingTime;
		}
	}
}
