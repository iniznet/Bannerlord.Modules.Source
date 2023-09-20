using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class PlayerDisconnectedMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[JsonProperty]
		public DisconnectType Type { get; private set; }

		[JsonProperty]
		public bool IsAllowedLeave { get; private set; }

		[JsonProperty]
		public BattleResult BattleResult { get; private set; }

		public PlayerDisconnectedMessage()
		{
		}

		public PlayerDisconnectedMessage(PlayerId playerId, DisconnectType type, bool isAllowedLeave, BattleResult battleResult)
		{
			this.PlayerId = playerId;
			this.Type = type;
			this.IsAllowedLeave = isAllowedLeave;
			this.BattleResult = battleResult;
		}
	}
}
