using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	public class BadgeOwnerKillTracker : GameBadgeTracker
	{
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

		public override void OnPlayerJoin(PlayerData playerData)
		{
			this._playerBadgeMap[playerData.PlayerId] = this._requiredBadges.Contains(playerData.ShownBadgeId);
		}

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

		private readonly string _badgeId;

		private readonly BadgeCondition _condition;

		private readonly List<string> _requiredBadges;

		private readonly Dictionary<ValueTuple<PlayerId, string, string>, int> _dataDictionary;

		private readonly Dictionary<PlayerId, bool> _playerBadgeMap;
	}
}
