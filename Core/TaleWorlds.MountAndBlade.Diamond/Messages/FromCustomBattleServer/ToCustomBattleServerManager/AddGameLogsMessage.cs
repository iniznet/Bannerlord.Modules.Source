using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class AddGameLogsMessage : Message
	{
		public GameLog[] GameLogs { get; }

		public AddGameLogsMessage(GameLog[] gameLogs)
		{
			this.GameLogs = gameLogs;
		}
	}
}
