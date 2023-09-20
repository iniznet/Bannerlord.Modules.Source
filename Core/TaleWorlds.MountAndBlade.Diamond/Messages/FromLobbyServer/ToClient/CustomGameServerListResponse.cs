using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CustomGameServerListResponse : FunctionResult
	{
		public AvailableCustomGames AvailableCustomGames { get; private set; }

		public CustomGameServerListResponse(AvailableCustomGames availableCustomGames)
		{
			this.AvailableCustomGames = availableCustomGames;
		}
	}
}
