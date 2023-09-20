using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CheckClanTagValidMessage : Message
	{
		public string ClanTag { get; private set; }

		public CheckClanTagValidMessage(string clanTag)
		{
			this.ClanTag = clanTag;
		}
	}
}
