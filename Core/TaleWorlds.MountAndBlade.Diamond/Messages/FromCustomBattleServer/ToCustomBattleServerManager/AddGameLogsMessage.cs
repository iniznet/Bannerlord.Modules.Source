using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x0200005A RID: 90
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class AddGameLogsMessage : Message
	{
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00002EC5 File Offset: 0x000010C5
		public GameLog[] GameLogs { get; }

		// Token: 0x0600014D RID: 333 RVA: 0x00002ECD File Offset: 0x000010CD
		public AddGameLogsMessage(GameLog[] gameLogs)
		{
			this.GameLogs = gameLogs;
		}
	}
}
