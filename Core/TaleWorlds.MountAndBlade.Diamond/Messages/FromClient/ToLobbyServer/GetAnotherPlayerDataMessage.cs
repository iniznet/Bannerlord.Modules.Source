using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetAnotherPlayerDataMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public GetAnotherPlayerDataMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
