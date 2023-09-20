using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class PlayerDisconnectedMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public DisconnectType Type { get; private set; }

		public bool IsAllowedLeave { get; private set; }

		public BattleResult BattleResult { get; private set; }

		public PlayerDisconnectedMessage(PlayerId playerId, DisconnectType type, bool isAllowedLeave, BattleResult battleResult)
		{
			this.PlayerId = playerId;
			this.Type = type;
			this.IsAllowedLeave = isAllowedLeave;
			this.BattleResult = battleResult;
		}
	}
}
