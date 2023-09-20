using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class WhisperReceivedMessage : Message
	{
		public string FromPlayer { get; private set; }

		public string ToPlayer { get; private set; }

		public string Message { get; private set; }

		public WhisperReceivedMessage(string fromPlayer, string toPlayer, string message)
		{
			this.FromPlayer = fromPlayer;
			this.ToPlayer = toPlayer;
			this.Message = message;
		}
	}
}
