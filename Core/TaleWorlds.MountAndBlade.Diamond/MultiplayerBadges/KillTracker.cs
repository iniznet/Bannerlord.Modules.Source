using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x02000161 RID: 353
	public class KillTracker : GameBadgeTracker
	{
		// Token: 0x060008D2 RID: 2258 RVA: 0x0000F594 File Offset: 0x0000D794
		public KillTracker(string badgeId, BadgeCondition condition, Dictionary<ValueTuple<PlayerId, string, string>, int> dataDictionary)
		{
			this._badgeId = badgeId;
			this._condition = condition;
			this._dataDictionary = dataDictionary;
			this._faction = null;
			this._troop = null;
			string text;
			if (condition.Parameters.TryGetValue("faction", out text))
			{
				this._faction = text;
			}
			string text2;
			if (condition.Parameters.TryGetValue("troop", out text2))
			{
				this._troop = text2;
			}
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x0000F600 File Offset: 0x0000D800
		public override void OnKill(KillData killData)
		{
			if (killData.KillerId.IsValid && killData.VictimId.IsValid && !killData.KillerId.Equals(killData.VictimId) && (this._faction == null || this._faction == killData.KillerFaction) && (this._troop == null || this._troop == killData.KillerTroop))
			{
				int num;
				if (!this._dataDictionary.TryGetValue(new ValueTuple<PlayerId, string, string>(killData.KillerId, this._badgeId, this._condition.StringId), out num))
				{
					num = 0;
				}
				this._dataDictionary[new ValueTuple<PlayerId, string, string>(killData.KillerId, this._badgeId, this._condition.StringId)] = num + 1;
			}
		}

		// Token: 0x040004AF RID: 1199
		private readonly string _badgeId;

		// Token: 0x040004B0 RID: 1200
		private readonly BadgeCondition _condition;

		// Token: 0x040004B1 RID: 1201
		private readonly Dictionary<ValueTuple<PlayerId, string, string>, int> _dataDictionary;

		// Token: 0x040004B2 RID: 1202
		private readonly string _faction;

		// Token: 0x040004B3 RID: 1203
		private readonly string _troop;
	}
}
