using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000033 RID: 51
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InitializeSessionResponse : LoginResultObject
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000276A File Offset: 0x0000096A
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00002772 File Offset: 0x00000972
		public PlayerData PlayerData { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000277B File Offset: 0x0000097B
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00002783 File Offset: 0x00000983
		public ServerStatus ServerStatus { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000278C File Offset: 0x0000098C
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00002794 File Offset: 0x00000994
		public AvailableScenes AvailableScenes { get; private set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000279D File Offset: 0x0000099D
		// (set) Token: 0x060000AF RID: 175 RVA: 0x000027A5 File Offset: 0x000009A5
		public SupportedFeatures SupportedFeatures { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x000027AE File Offset: 0x000009AE
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000027B6 File Offset: 0x000009B6
		public bool HasPendingRejoin { get; private set; }

		// Token: 0x060000B2 RID: 178 RVA: 0x000027BF File Offset: 0x000009BF
		public InitializeSessionResponse(PlayerData playerData, ServerStatus serverStatus, AvailableScenes availableScenes, SupportedFeatures supportedFeatures, bool hasPendingRejoin)
		{
			this.PlayerData = playerData;
			this.ServerStatus = serverStatus;
			this.AvailableScenes = availableScenes;
			this.SupportedFeatures = supportedFeatures;
			this.HasPendingRejoin = hasPendingRejoin;
		}
	}
}
