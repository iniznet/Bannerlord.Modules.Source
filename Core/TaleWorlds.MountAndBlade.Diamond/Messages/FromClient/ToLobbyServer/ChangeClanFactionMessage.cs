using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeClanFactionMessage : Message
	{
		public string NewFaction { get; private set; }

		public ChangeClanFactionMessage(string newFaction)
		{
			this.NewFaction = newFaction;
		}
	}
}
