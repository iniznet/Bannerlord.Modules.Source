using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanCreationRequestMessage : Message
	{
		public string CreatorPlayerName { get; private set; }

		public PlayerId CreatorPlayerId { get; private set; }

		public string ClanName { get; private set; }

		public string ClanTag { get; private set; }

		public ClanCreationRequestMessage(PlayerId creatorPlayerId, string creatorPlayerName, string clanName, string clanTag)
		{
			this.CreatorPlayerId = creatorPlayerId;
			this.CreatorPlayerName = creatorPlayerName;
			this.ClanName = clanName;
			this.ClanTag = clanTag;
		}
	}
}
