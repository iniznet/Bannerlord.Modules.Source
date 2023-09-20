using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ServerStatusMessage : Message
	{
		public ServerStatus ServerStatus { get; private set; }

		public ServerStatusMessage(ServerStatus serverStatus)
		{
			this.ServerStatus = serverStatus;
		}
	}
}
