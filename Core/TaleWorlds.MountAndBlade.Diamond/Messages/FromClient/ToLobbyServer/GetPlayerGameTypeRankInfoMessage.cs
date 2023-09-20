using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerGameTypeRankInfoMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public GetPlayerGameTypeRankInfoMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
