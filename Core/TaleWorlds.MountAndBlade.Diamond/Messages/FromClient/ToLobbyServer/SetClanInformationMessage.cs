using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class SetClanInformationMessage : Message
	{
		public string Information { get; private set; }

		public SetClanInformationMessage(string information)
		{
			this.Information = information;
		}
	}
}
