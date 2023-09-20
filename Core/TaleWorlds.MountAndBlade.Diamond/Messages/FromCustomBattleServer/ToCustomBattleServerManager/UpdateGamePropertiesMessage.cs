using System;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x02000061 RID: 97
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class UpdateGamePropertiesMessage : Message
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600018A RID: 394 RVA: 0x000031C0 File Offset: 0x000013C0
		// (set) Token: 0x0600018B RID: 395 RVA: 0x000031C8 File Offset: 0x000013C8
		public string GameType { get; private set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600018C RID: 396 RVA: 0x000031D1 File Offset: 0x000013D1
		// (set) Token: 0x0600018D RID: 397 RVA: 0x000031D9 File Offset: 0x000013D9
		public string Scene { get; private set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600018E RID: 398 RVA: 0x000031E2 File Offset: 0x000013E2
		// (set) Token: 0x0600018F RID: 399 RVA: 0x000031EA File Offset: 0x000013EA
		public string UniqueSceneId { get; private set; }

		// Token: 0x06000190 RID: 400 RVA: 0x000031F3 File Offset: 0x000013F3
		public UpdateGamePropertiesMessage(string gameType, string scene, string uniqueSceneId)
		{
			this.GameType = gameType;
			this.Scene = scene;
			this.UniqueSceneId = uniqueSceneId;
		}
	}
}
