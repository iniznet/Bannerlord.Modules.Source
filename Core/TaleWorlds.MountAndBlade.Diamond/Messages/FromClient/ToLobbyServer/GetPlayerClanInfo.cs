using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerClanInfo : Message
	{
		public PlayerId PlayerId { get; private set; }

		public GetPlayerClanInfo(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
