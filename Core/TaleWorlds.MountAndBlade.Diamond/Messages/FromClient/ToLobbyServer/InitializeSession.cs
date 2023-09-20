using System;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A3 RID: 163
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InitializeSession : LoginMessage
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000245 RID: 581 RVA: 0x00003991 File Offset: 0x00001B91
		// (set) Token: 0x06000246 RID: 582 RVA: 0x00003999 File Offset: 0x00001B99
		public PlayerId PlayerId { get; private set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000247 RID: 583 RVA: 0x000039A2 File Offset: 0x00001BA2
		// (set) Token: 0x06000248 RID: 584 RVA: 0x000039AA File Offset: 0x00001BAA
		public string PlayerName { get; private set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000249 RID: 585 RVA: 0x000039B3 File Offset: 0x00001BB3
		// (set) Token: 0x0600024A RID: 586 RVA: 0x000039BB File Offset: 0x00001BBB
		public object AccessObject { get; private set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600024B RID: 587 RVA: 0x000039C4 File Offset: 0x00001BC4
		// (set) Token: 0x0600024C RID: 588 RVA: 0x000039CC File Offset: 0x00001BCC
		public ApplicationVersion ApplicationVersion { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600024D RID: 589 RVA: 0x000039D5 File Offset: 0x00001BD5
		// (set) Token: 0x0600024E RID: 590 RVA: 0x000039DD File Offset: 0x00001BDD
		public string ConnectionPassword { get; private set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600024F RID: 591 RVA: 0x000039E6 File Offset: 0x00001BE6
		// (set) Token: 0x06000250 RID: 592 RVA: 0x000039EE File Offset: 0x00001BEE
		public ModuleInfoModel[] LoadedModules { get; private set; }

		// Token: 0x06000251 RID: 593 RVA: 0x000039F7 File Offset: 0x00001BF7
		public InitializeSession(PlayerId playerId, string playerName, object accessObject, ApplicationVersion applicationVersion, string connectionPassword, ModuleInfoModel[] loadedModules)
			: base(playerId.ConvertToPeerId())
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			this.AccessObject = accessObject;
			this.ApplicationVersion = applicationVersion;
			this.ConnectionPassword = connectionPassword;
			this.LoadedModules = loadedModules;
		}
	}
}
