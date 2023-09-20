using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameAnswerMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public JoinPremadeGameAnswerMessage()
		{
		}

		public JoinPremadeGameAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
