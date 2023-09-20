using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CheckClanNameValidMessage : Message
	{
		public string ClanName { get; private set; }

		public CheckClanNameValidMessage(string clanName)
		{
			this.ClanName = clanName;
		}
	}
}
