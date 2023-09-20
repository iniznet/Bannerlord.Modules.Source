using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PartyMessageReceivedMessage : Message
	{
		public string PlayerName { get; private set; }

		public string Message { get; private set; }

		public PartyMessageReceivedMessage(string playerName, string message)
		{
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
