using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameRequestResultMessage : Message
	{
		public bool Successful { get; private set; }

		public JoinPremadeGameRequestResultMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
