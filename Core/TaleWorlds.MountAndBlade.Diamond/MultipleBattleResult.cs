using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MultipleBattleResult
	{
		public List<BattleResult> BattleResults { get; set; }

		public MultipleBattleResult()
		{
			this.BattleResults = new List<BattleResult>();
			this._currentBattleIndex = -1;
		}

		public void CreateNewBattleResult(string gameType)
		{
			BattleResult battleResult = new BattleResult();
			this.BattleResults.Add(battleResult);
			this._currentBattleIndex++;
			if (this._currentBattleIndex > 0)
			{
				foreach (KeyValuePair<string, BattlePlayerEntry> keyValuePair in this.BattleResults[this._currentBattleIndex - 1].PlayerEntries)
				{
					battleResult.AddOrUpdatePlayerEntry(PlayerId.FromString(keyValuePair.Key), keyValuePair.Value.TeamNo, gameType, Guid.Empty);
				}
			}
		}

		public BattleResult GetCurrentBattleResult()
		{
			return this.BattleResults[this._currentBattleIndex];
		}

		private int _currentBattleIndex;
	}
}
