using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class PlayerDisconnectedMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public DisconnectType Type { get; private set; }

		public PlayerDisconnectedMessage(PlayerId playerId, DisconnectType type)
		{
			this.PlayerId = playerId;
			this.Type = type;
		}
	}
}
