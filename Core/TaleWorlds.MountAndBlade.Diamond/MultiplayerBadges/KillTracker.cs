using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	public class KillTracker : GameBadgeTracker
	{
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

		private readonly string _badgeId;

		private readonly BadgeCondition _condition;

		private readonly Dictionary<ValueTuple<PlayerId, string, string>, int> _dataDictionary;

		private readonly string _faction;

		private readonly string _troop;
	}
}
