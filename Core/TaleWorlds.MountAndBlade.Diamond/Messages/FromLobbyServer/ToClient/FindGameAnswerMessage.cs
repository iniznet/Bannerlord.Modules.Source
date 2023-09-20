using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class FindGameAnswerMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		[JsonProperty]
		public string[] SelectedAndEnabledGameTypes { get; private set; }

		public FindGameAnswerMessage()
		{
		}

		public FindGameAnswerMessage(bool successful, string[] selectedAndEnabledGameTypes)
		{
			this.Successful = successful;
			this.SelectedAndEnabledGameTypes = selectedAndEnabledGameTypes;
		}
	}
}
