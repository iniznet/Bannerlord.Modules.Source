using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class KickFromClanMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public KickFromClanMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
