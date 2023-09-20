using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PartyMessage : Message
	{
		public string Message { get; private set; }

		public PartyMessage(string message)
		{
			this.Message = message;
		}
	}
}
