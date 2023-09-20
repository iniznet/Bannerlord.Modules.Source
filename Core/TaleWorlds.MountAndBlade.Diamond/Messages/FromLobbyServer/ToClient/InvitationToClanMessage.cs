using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitationToClanMessage : Message
	{
		[JsonProperty]
		public PlayerId InviterId { get; private set; }

		[JsonProperty]
		public string ClanName { get; private set; }

		[JsonProperty]
		public string ClanTag { get; private set; }

		[JsonProperty]
		public int ClanPlayerCount { get; private set; }

		public InvitationToClanMessage()
		{
		}

		public InvitationToClanMessage(PlayerId inviterId, string clanName, string clanTag, int clanPlayerCount)
		{
			this.InviterId = inviterId;
			this.ClanName = clanName;
			this.ClanTag = clanTag;
			this.ClanPlayerCount = clanPlayerCount;
		}
	}
}
