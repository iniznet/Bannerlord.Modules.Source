using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ServerStatusMessage : Message
	{
		[JsonProperty]
		public ServerStatus ServerStatus { get; private set; }

		public ServerStatusMessage()
		{
		}

		public ServerStatusMessage(ServerStatus serverStatus)
		{
			this.ServerStatus = serverStatus;
		}
	}
}
