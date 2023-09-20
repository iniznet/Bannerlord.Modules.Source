using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CreatePremadeGameAnswerMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public CreatePremadeGameAnswerMessage()
		{
		}

		public CreatePremadeGameAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
