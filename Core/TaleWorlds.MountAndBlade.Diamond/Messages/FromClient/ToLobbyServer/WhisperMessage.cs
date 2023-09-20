using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class WhisperMessage : Message
	{
		public string TargetPlayerName { get; private set; }

		public string Message { get; private set; }

		public WhisperMessage(string targetPlayerName, string message)
		{
			this.TargetPlayerName = targetPlayerName;
			this.Message = message;
		}
	}
}
