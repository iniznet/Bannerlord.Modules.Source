using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MultipleBattleResult
	{
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
				foreach (KeyValuePair<PlayerId, BattlePlayerEntry> keyValuePair in this.BattleResults[this._currentBattleIndex - 1].PlayerEntries)
				{
					battleResult.AddOrUpdatePlayerEntry(keyValuePair.Key, keyValuePair.Value.TeamNo, gameType, Guid.Empty);
				}
			}
		}

		public BattleResult GetCurrentBattleResult()
		{
			return this.BattleResults[this._currentBattleIndex];
		}

		public List<BattleResult> BattleResults;

		private int _currentBattleIndex;
	}
}
