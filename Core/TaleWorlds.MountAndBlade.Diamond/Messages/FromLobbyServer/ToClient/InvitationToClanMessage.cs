using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitationToClanMessage : Message
	{
		public PlayerId InviterId { get; private set; }

		public string ClanName { get; private set; }

		public string ClanTag { get; private set; }

		public int ClanPlayerCount { get; private set; }

		public InvitationToClanMessage(PlayerId inviterId, string clanName, string clanTag, int clanPlayerCount)
		{
			this.InviterId = inviterId;
			this.ClanName = clanName;
			this.ClanTag = clanTag;
			this.ClanPlayerCount = clanPlayerCount;
		}
	}
}
