using System;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x0200005D RID: 93
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerReadyMessage : LoginMessage
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00002F46 File Offset: 0x00001146
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00002F4E File Offset: 0x0000114E
		public ApplicationVersion ApplicationVersion { get; private set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00002F57 File Offset: 0x00001157
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00002F5F File Offset: 0x0000115F
		public string AuthToken { get; private set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00002F68 File Offset: 0x00001168
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00002F70 File Offset: 0x00001170
		public ModuleInfoModel[] LoadedModules { get; private set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00002F79 File Offset: 0x00001179
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00002F81 File Offset: 0x00001181
		public bool AllowsOptionalModules { get; private set; }

		// Token: 0x0600015E RID: 350 RVA: 0x00002F8A File Offset: 0x0000118A
		public CustomBattleServerReadyMessage(PeerId peerId, ApplicationVersion applicationVersion, string authToken, ModuleInfoModel[] loadedModules, bool allowsOptionalModules)
			: base(peerId)
		{
			this.ApplicationVersion = applicationVersion;
			this.AuthToken = authToken;
			this.LoadedModules = loadedModules;
			this.AllowsOptionalModules = allowsOptionalModules;
		}
	}
}
