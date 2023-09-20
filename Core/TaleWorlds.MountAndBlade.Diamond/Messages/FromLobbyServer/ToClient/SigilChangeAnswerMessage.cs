using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class SigilChangeAnswerMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public SigilChangeAnswerMessage()
		{
		}

		public SigilChangeAnswerMessage(bool answer)
		{
			this.Successful = answer;
		}
	}
}
