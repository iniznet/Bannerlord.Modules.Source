using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetAnotherPlayerDataMessageResult : FunctionResult
	{
		public PlayerData AnotherPlayerData { get; private set; }

		public GetAnotherPlayerDataMessageResult(PlayerData playerData)
		{
			this.AnotherPlayerData = playerData;
		}
	}
}
