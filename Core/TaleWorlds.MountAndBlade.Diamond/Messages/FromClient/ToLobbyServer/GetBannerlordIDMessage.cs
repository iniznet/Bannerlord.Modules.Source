using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetBannerlordIDMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public GetBannerlordIDMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
