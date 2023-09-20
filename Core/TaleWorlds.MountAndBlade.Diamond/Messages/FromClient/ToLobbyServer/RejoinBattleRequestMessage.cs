using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RejoinBattleRequestMessage : Message
	{
		public bool IsRejoinAccepted { get; private set; }

		public RejoinBattleRequestMessage(bool isRejoinAccepted)
		{
			this.IsRejoinAccepted = isRejoinAccepted;
		}
	}
}
