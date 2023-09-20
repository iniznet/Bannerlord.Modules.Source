using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class SetClanInformationMessage : Message
	{
		[JsonProperty]
		public string Information { get; private set; }

		public SetClanInformationMessage()
		{
		}

		public SetClanInformationMessage(string information)
		{
			this.Information = information;
		}
	}
}
