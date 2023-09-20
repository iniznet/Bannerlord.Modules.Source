using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x0200005C RID: 92
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerFinishingMessage : Message
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00002F11 File Offset: 0x00001111
		public GameLog[] GameLogs { get; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00002F19 File Offset: 0x00001119
		public Dictionary<ValueTuple<PlayerId, string, string>, int> BadgeDataDictionary { get; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00002F21 File Offset: 0x00001121
		public MultipleBattleResult BattleResult { get; }

		// Token: 0x06000155 RID: 341 RVA: 0x00002F29 File Offset: 0x00001129
		public CustomBattleServerFinishingMessage(GameLog[] gameLogs, Dictionary<ValueTuple<PlayerId, string, string>, int> badgeDataDictionary, MultipleBattleResult battleResult)
		{
			this.GameLogs = gameLogs;
			this.BadgeDataDictionary = badgeDataDictionary;
			this.BattleResult = battleResult;
		}
	}
}
