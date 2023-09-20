using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CreatePremadeGameAnswerMessage : Message
	{
		public bool Successful { get; private set; }

		public CreatePremadeGameAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
