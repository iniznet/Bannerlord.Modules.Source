using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class SigilChangeAnswerMessage : Message
	{
		public bool Successful { get; private set; }

		public SigilChangeAnswerMessage(bool answer)
		{
			this.Successful = answer;
		}
	}
}
