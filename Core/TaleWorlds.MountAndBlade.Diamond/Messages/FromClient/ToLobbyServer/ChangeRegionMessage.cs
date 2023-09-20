using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeRegionMessage : Message
	{
		[JsonProperty]
		public string Region { get; private set; }

		public ChangeRegionMessage()
		{
		}

		public ChangeRegionMessage(string region)
		{
			this.Region = region;
		}
	}
}
