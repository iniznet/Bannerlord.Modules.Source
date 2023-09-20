using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RejoinBattleRequestMessage : Message
	{
		[JsonProperty]
		public bool IsRejoinAccepted { get; private set; }

		public RejoinBattleRequestMessage()
		{
		}

		public RejoinBattleRequestMessage(bool isRejoinAccepted)
		{
			this.IsRejoinAccepted = isRejoinAccepted;
		}
	}
}
