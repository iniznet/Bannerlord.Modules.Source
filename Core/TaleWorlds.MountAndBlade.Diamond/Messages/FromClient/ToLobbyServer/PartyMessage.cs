using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PartyMessage : Message
	{
		[JsonProperty]
		public string Message { get; private set; }

		public PartyMessage()
		{
		}

		public PartyMessage(string message)
		{
			this.Message = message;
		}
	}
}
