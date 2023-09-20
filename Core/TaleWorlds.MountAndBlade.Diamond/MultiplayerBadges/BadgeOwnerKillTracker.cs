using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x0200015D RID: 349
	public class BadgeOwnerKillTracker : GameBadgeTracker
	{
		// Token: 0x060008BB RID: 2235 RVA: 0x0000F2DC File Offset: 0x0000D4DC
		public BadgeOwnerKillTracker(string badgeId, BadgeCondition condition, Dictionary<ValueTuple<PlayerId, string, string>, int> dataDictionary)
		{
			this._badgeId = badgeId;
			this._condition = condition;
			this._playerBadgeMap = new Dictionary<PlayerId, bool>();
			this._dataDictionary = dataDictionary;
			this._requiredBadges = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in condition.Parameters)
			{
				if (keyValuePair.Key.StartsWith("required_badge."))
				{
					this._requiredBadges.Add(keyValuePair.Value);
				}
			}
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x0000F378 File Offset: 0x0000D578
		public override void OnPlayerJoin(PlayerData playerData)
		{
			this._playerBadgeMap[playerData.PlayerId] = this._requiredBadges.Contains(playerData.ShownBadgeId);
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0000F39C File Offset: 0x0000D59C
		public override void OnKill(KillData killData)
		{
			bool flag;
			if (killData.KillerId.IsValid && killData.VictimId.IsValid && !killData.KillerId.Equals(killData.VictimId) && this._playerBadgeMap.TryGetValue(killData.VictimId, out flag) && flag)
			{
				this._playerBadgeMap[killData.KillerId] = true;
				int num;
				if (!this._dataDictionary.TryGetValue(new ValueTuple<PlayerId, string, string>(killData.KillerId, this._badgeId, this._condition.StringId), out num))
				{
					num = 0;
				}
				this._dataDictionary[new ValueTuple<PlayerId, string, string>(killData.KillerId, this._badgeId, this._condition.StringId)] = num + 1;
			}
		}

		// Token: 0x040004A3 RID: 1187
		private readonly string _badgeId;

		// Token: 0x040004A4 RID: 1188
		private readonly BadgeCondition _condition;

		// Token: 0x040004A5 RID: 1189
		private readonly List<string> _requiredBadges;

		// Token: 0x040004A6 RID: 1190
		private readonly Dictionary<ValueTuple<PlayerId, string, string>, int> _dataDictionary;

		// Token: 0x040004A7 RID: 1191
		private readonly Dictionary<PlayerId, bool> _playerBadgeMap;
	}
}
