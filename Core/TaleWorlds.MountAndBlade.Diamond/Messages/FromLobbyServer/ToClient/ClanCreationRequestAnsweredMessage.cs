using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanCreationRequestAnsweredMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public ClanCreationAnswer ClanCreationAnswer { get; private set; }

		public ClanCreationRequestAnsweredMessage(PlayerId playerId, ClanCreationAnswer clanCreationAnswer)
		{
			this.PlayerId = playerId;
			this.ClanCreationAnswer = clanCreationAnswer;
		}
	}
}
