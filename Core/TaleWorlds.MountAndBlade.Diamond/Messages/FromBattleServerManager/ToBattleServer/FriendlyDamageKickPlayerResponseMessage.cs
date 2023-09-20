using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class FriendlyDamageKickPlayerResponseMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public FriendlyDamageKickPlayerResponseMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
