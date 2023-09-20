using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanMessage : Message
	{
		[JsonProperty]
		public string Message { get; private set; }

		public ClanMessage()
		{
		}

		public ClanMessage(string message)
		{
			this.Message = message;
		}
	}
}
