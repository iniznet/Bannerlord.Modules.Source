using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class FriendlyDamageKickPlayerMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[TupleElementNames(new string[] { "killCount", "damage" })]
		[JsonProperty]
		public Dictionary<int, ValueTuple<int, float>> RoundDamageMap
		{
			[return: TupleElementNames(new string[] { "killCount", "damage" })]
			get;
			[param: TupleElementNames(new string[] { "killCount", "damage" })]
			private set;
		}

		public FriendlyDamageKickPlayerMessage()
		{
		}

		public FriendlyDamageKickPlayerMessage(PlayerId playerId, [TupleElementNames(new string[] { "killCount", "damage" })] Dictionary<int, ValueTuple<int, float>> roundDamageMap)
		{
			this.PlayerId = playerId;
			this.RoundDamageMap = roundDamageMap;
		}
	}
}
