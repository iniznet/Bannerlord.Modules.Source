using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class PlayerFledBattleMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public PlayerFledBattleMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
