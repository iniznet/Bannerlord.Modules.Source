using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerFinishingMessage : Message
	{
		[JsonProperty]
		public GameLog[] GameLogs { get; private set; }

		[JsonProperty]
		public List<BadgeDataEntry> BadgeDataEntries { get; private set; }

		[JsonProperty]
		public MultipleBattleResult BattleResult { get; private set; }

		public CustomBattleServerFinishingMessage()
		{
		}

		public CustomBattleServerFinishingMessage(GameLog[] gameLogs, Dictionary<ValueTuple<PlayerId, string, string>, int> badgeDataDictionary, MultipleBattleResult battleResult)
		{
			this.GameLogs = gameLogs;
			this.BadgeDataEntries = BadgeDataEntry.ToList(badgeDataDictionary);
			this.BattleResult = battleResult;
		}
	}
}
