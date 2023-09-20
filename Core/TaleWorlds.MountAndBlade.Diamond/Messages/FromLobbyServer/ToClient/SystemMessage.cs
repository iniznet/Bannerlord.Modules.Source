using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class SystemMessage : Message
	{
		public ServerInfoMessage Message { get; private set; }

		public List<string> Parameters { get; private set; }

		public SystemMessage(ServerInfoMessage message, params string[] arguments)
		{
			this.Message = message;
			this.Parameters = new List<string>(arguments);
		}
	}
}
