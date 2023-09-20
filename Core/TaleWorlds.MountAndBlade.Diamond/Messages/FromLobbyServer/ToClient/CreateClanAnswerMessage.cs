using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CreateClanAnswerMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public CreateClanAnswerMessage()
		{
		}

		public CreateClanAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
