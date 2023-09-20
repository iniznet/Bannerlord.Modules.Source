using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000326 RID: 806
	public class MultiplayerGameTypeInfo
	{
		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06002B8B RID: 11147 RVA: 0x000A9D34 File Offset: 0x000A7F34
		// (set) Token: 0x06002B8C RID: 11148 RVA: 0x000A9D3C File Offset: 0x000A7F3C
		public string GameModule { get; private set; }

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06002B8D RID: 11149 RVA: 0x000A9D45 File Offset: 0x000A7F45
		// (set) Token: 0x06002B8E RID: 11150 RVA: 0x000A9D4D File Offset: 0x000A7F4D
		public string GameType { get; private set; }

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06002B8F RID: 11151 RVA: 0x000A9D56 File Offset: 0x000A7F56
		// (set) Token: 0x06002B90 RID: 11152 RVA: 0x000A9D5E File Offset: 0x000A7F5E
		public List<string> Scenes { get; private set; }

		// Token: 0x06002B91 RID: 11153 RVA: 0x000A9D67 File Offset: 0x000A7F67
		public MultiplayerGameTypeInfo(string gameModule, string gameType)
		{
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.Scenes = new List<string>();
		}
	}
}
