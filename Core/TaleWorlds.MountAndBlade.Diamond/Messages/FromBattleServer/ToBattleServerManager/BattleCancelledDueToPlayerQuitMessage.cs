using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleCancelledDueToPlayerQuitMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public BattleCancelledDueToPlayerQuitMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
