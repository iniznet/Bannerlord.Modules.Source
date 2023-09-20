using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CheckClanTagValidMessage : Message
	{
		[JsonProperty]
		public string ClanTag { get; private set; }

		public CheckClanTagValidMessage()
		{
		}

		public CheckClanTagValidMessage(string clanTag)
		{
			this.ClanTag = clanTag;
		}
	}
}
