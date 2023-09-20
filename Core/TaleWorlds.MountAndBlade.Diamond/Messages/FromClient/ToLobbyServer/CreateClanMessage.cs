using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class CreateClanMessage : Message
	{
		public string ClanName { get; private set; }

		public string ClanTag { get; private set; }

		public string ClanFaction { get; private set; }

		public string ClanSigil { get; private set; }

		public CreateClanMessage(string clanName, string clanTag, string clanFaction, string clanSigil)
		{
			this.ClanName = clanName;
			this.ClanTag = clanTag;
			this.ClanFaction = clanFaction;
			this.ClanSigil = clanSigil;
		}
	}
}
