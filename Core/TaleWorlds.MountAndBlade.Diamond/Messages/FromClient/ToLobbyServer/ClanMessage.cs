using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanMessage : Message
	{
		public string Message { get; private set; }

		public ClanMessage(string message)
		{
			this.Message = message;
		}
	}
}
