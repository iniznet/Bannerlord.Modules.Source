using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanMessageReceivedMessage : Message
	{
		public string PlayerName { get; private set; }

		public string Message { get; private set; }

		public ClanMessageReceivedMessage(string playerName, string message)
		{
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
