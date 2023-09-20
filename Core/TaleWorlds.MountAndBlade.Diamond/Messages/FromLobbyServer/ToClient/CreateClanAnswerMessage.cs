using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CreateClanAnswerMessage : Message
	{
		public bool Successful { get; private set; }

		public CreateClanAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
