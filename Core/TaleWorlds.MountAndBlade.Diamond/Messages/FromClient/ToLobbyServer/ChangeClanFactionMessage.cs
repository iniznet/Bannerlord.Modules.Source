using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeClanFactionMessage : Message
	{
		[JsonProperty]
		public string NewFaction { get; private set; }

		public ChangeClanFactionMessage()
		{
		}

		public ChangeClanFactionMessage(string newFaction)
		{
			this.NewFaction = newFaction;
		}
	}
}
