using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class WhisperMessage : Message
	{
		[JsonProperty]
		public string TargetPlayerName { get; private set; }

		[JsonProperty]
		public string Message { get; private set; }

		public WhisperMessage()
		{
		}

		public WhisperMessage(string targetPlayerName, string message)
		{
			this.TargetPlayerName = targetPlayerName;
			this.Message = message;
		}
	}
}
