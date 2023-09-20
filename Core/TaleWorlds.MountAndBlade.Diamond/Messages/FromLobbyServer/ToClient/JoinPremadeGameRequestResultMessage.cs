using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameRequestResultMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public JoinPremadeGameRequestResultMessage()
		{
		}

		public JoinPremadeGameRequestResultMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
