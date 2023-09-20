using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class AdminMessage : Message
	{
		public string Message { get; private set; }

		public AdminMessage(string message)
		{
			this.Message = message;
		}
	}
}
