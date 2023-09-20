using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class PlayerDisconnectedFromLobbyMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public PlayerDisconnectedFromLobbyMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
