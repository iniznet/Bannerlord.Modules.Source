using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class PlayerFledBattleAnswerMessage : Message
	{
		[JsonProperty]
		public BattleResult BattleResult { get; private set; }

		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[JsonProperty]
		public bool IsAllowedLeave { get; private set; }

		public PlayerFledBattleAnswerMessage()
		{
		}

		public PlayerFledBattleAnswerMessage(PlayerId playerId, BattleResult battleResult, bool isAllowedLeave)
		{
			this.PlayerId = playerId;
			this.BattleResult = battleResult;
			this.IsAllowedLeave = isAllowedLeave;
		}
	}
}
