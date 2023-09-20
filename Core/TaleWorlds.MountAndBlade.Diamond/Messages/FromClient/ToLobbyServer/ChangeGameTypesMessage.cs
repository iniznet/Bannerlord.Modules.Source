using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeGameTypesMessage : Message
	{
		public string[] GameTypes { get; private set; }

		public ChangeGameTypesMessage(string[] gameTypes)
		{
			this.GameTypes = gameTypes;
		}
	}
}
