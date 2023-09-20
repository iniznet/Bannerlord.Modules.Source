using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PSPlayerJoinedToPlayerSessionMessage : Message
	{
		public ulong InviterPlayerAccountId { get; }

		public PSPlayerJoinedToPlayerSessionMessage(ulong inviterPlayerAccountId)
		{
			this.InviterPlayerAccountId = inviterPlayerAccountId;
		}
	}
}
