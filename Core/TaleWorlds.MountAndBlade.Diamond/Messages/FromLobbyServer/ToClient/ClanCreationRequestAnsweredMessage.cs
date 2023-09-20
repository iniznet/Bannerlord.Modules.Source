using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanCreationRequestAnsweredMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[JsonProperty]
		public ClanCreationAnswer ClanCreationAnswer { get; private set; }

		public ClanCreationRequestAnsweredMessage()
		{
		}

		public ClanCreationRequestAnsweredMessage(PlayerId playerId, ClanCreationAnswer clanCreationAnswer)
		{
			this.PlayerId = playerId;
			this.ClanCreationAnswer = clanCreationAnswer;
		}
	}
}
