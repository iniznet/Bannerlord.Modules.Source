using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PSPlayerJoinedToPlayerSessionMessage : Message
	{
		[JsonProperty]
		public ulong InviterPlayerAccountId { get; private set; }

		public PSPlayerJoinedToPlayerSessionMessage()
		{
		}

		public PSPlayerJoinedToPlayerSessionMessage(ulong inviterPlayerAccountId)
		{
			this.InviterPlayerAccountId = inviterPlayerAccountId;
		}
	}
}
