using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerFinishingMessage : Message
	{
		public GameLog[] GameLogs { get; }

		public Dictionary<ValueTuple<PlayerId, string, string>, int> BadgeDataDictionary { get; }

		public MultipleBattleResult BattleResult { get; }

		public CustomBattleServerFinishingMessage(GameLog[] gameLogs, Dictionary<ValueTuple<PlayerId, string, string>, int> badgeDataDictionary, MultipleBattleResult battleResult)
		{
			this.GameLogs = gameLogs;
			this.BadgeDataDictionary = badgeDataDictionary;
			this.BattleResult = battleResult;
		}
	}
}
