using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000EF RID: 239
	[Serializable]
	public class MultipleBattleResult
	{
		// Token: 0x060003E2 RID: 994 RVA: 0x00004EE5 File Offset: 0x000030E5
		public MultipleBattleResult()
		{
			this.BattleResults = new List<BattleResult>();
			this._currentBattleIndex = -1;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00004F00 File Offset: 0x00003100
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

		// Token: 0x060003E4 RID: 996 RVA: 0x00004FA8 File Offset: 0x000031A8
		public BattleResult GetCurrentBattleResult()
		{
			return this.BattleResults[this._currentBattleIndex];
		}

		// Token: 0x04000190 RID: 400
		public List<BattleResult> BattleResults;

		// Token: 0x04000191 RID: 401
		private int _currentBattleIndex;
	}
}
