using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeRegionMessage : Message
	{
		public string Region { get; private set; }

		public ChangeRegionMessage(string region)
		{
			this.Region = region;
		}
	}
}
