using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameAnswerMessage : Message
	{
		public bool Successful { get; private set; }

		public JoinPremadeGameAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
