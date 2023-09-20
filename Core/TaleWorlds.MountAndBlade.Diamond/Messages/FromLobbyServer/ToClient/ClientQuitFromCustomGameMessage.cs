using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class ClientQuitFromCustomGameMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public ClientQuitFromCustomGameMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
