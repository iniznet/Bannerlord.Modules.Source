using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanMessageReceivedMessage : Message
	{
		[JsonProperty]
		public string PlayerName { get; private set; }

		[JsonProperty]
		public string Message { get; private set; }

		public ClanMessageReceivedMessage()
		{
		}

		public ClanMessageReceivedMessage(string playerName, string message)
		{
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
