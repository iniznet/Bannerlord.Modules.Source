using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleCancelledDueToPlayerQuitMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public BattleCancelledDueToPlayerQuitMessage()
		{
		}

		public BattleCancelledDueToPlayerQuitMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
