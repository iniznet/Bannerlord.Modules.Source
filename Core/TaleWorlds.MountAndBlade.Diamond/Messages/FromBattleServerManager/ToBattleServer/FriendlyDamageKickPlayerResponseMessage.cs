using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class FriendlyDamageKickPlayerResponseMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public FriendlyDamageKickPlayerResponseMessage()
		{
		}

		public FriendlyDamageKickPlayerResponseMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
