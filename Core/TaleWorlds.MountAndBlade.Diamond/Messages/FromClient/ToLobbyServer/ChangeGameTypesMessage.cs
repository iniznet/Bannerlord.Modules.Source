using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeGameTypesMessage : Message
	{
		[JsonProperty]
		public string[] GameTypes { get; private set; }

		public ChangeGameTypesMessage()
		{
		}

		public ChangeGameTypesMessage(string[] gameTypes)
		{
			this.GameTypes = gameTypes;
		}
	}
}
