using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class FindGameAnswerMessage : Message
	{
		public bool Successful { get; private set; }

		public string[] SelectedAndEnabledGameTypes { get; private set; }

		public FindGameAnswerMessage(bool successful, string[] selectedAndEnabledGameTypes)
		{
			this.Successful = successful;
			this.SelectedAndEnabledGameTypes = selectedAndEnabledGameTypes;
		}
	}
}
