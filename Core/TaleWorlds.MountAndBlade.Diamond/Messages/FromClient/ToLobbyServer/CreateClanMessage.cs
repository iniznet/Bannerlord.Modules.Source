using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class CreateClanMessage : Message
	{
		[JsonProperty]
		public string ClanName { get; private set; }

		[JsonProperty]
		public string ClanTag { get; private set; }

		[JsonProperty]
		public string ClanFaction { get; private set; }

		[JsonProperty]
		public string ClanSigil { get; private set; }

		public CreateClanMessage()
		{
		}

		public CreateClanMessage(string clanName, string clanTag, string clanFaction, string clanSigil)
		{
			this.ClanName = clanName;
			this.ClanTag = clanTag;
			this.ClanFaction = clanFaction;
			this.ClanSigil = clanSigil;
		}
	}
}
