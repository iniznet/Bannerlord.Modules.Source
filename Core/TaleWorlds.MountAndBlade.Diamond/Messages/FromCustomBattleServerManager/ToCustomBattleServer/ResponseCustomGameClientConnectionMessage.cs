using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class ResponseCustomGameClientConnectionMessage : Message
	{
		public PlayerJoinGameResponseDataFromHost[] PlayerJoinData { get; private set; }

		public ResponseCustomGameClientConnectionMessage(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			this.PlayerJoinData = playerJoinData;
		}
	}
}
