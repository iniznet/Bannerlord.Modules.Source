using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000CF RID: 207
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class FriendlyDamageKickPlayerMessage : Message
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x00004197 File Offset: 0x00002397
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x0000419F File Offset: 0x0000239F
		public PlayerId PlayerId { get; private set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x000041A8 File Offset: 0x000023A8
		// (set) Token: 0x060002FA RID: 762 RVA: 0x000041B0 File Offset: 0x000023B0
		[TupleElementNames(new string[] { "killCount", "damage" })]
		public Dictionary<int, ValueTuple<int, float>> RoundDamageMap
		{
			[return: TupleElementNames(new string[] { "killCount", "damage" })]
			get;
			[param: TupleElementNames(new string[] { "killCount", "damage" })]
			private set;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x000041B9 File Offset: 0x000023B9
		public FriendlyDamageKickPlayerMessage(PlayerId playerId, [TupleElementNames(new string[] { "killCount", "damage" })] Dictionary<int, ValueTuple<int, float>> roundDamageMap)
		{
			this.PlayerId = playerId;
			this.RoundDamageMap = roundDamageMap;
		}
	}
}
