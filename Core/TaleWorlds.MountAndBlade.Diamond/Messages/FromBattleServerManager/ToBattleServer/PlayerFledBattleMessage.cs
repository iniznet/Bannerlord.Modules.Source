using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class PlayerFledBattleMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public PlayerFledBattleMessage()
		{
		}

		public PlayerFledBattleMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
