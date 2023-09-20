using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanCreationSuccessfulMessage : Message
	{
	}
}
