using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CheckClanNameValidMessage : Message
	{
		[JsonProperty]
		public string ClanName { get; private set; }

		public CheckClanNameValidMessage()
		{
		}

		public CheckClanNameValidMessage(string clanName)
		{
			this.ClanName = clanName;
		}
	}
}
