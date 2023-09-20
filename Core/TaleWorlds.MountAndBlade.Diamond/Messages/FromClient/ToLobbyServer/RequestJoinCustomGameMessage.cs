using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B8 RID: 184
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RequestJoinCustomGameMessage : Message
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600029E RID: 670 RVA: 0x00003D99 File Offset: 0x00001F99
		// (set) Token: 0x0600029F RID: 671 RVA: 0x00003DA1 File Offset: 0x00001FA1
		public CustomBattleId CustomBattleId { get; private set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x00003DAA File Offset: 0x00001FAA
		// (set) Token: 0x060002A1 RID: 673 RVA: 0x00003DB2 File Offset: 0x00001FB2
		public string Password { get; private set; }

		// Token: 0x060002A2 RID: 674 RVA: 0x00003DBB File Offset: 0x00001FBB
		public RequestJoinCustomGameMessage(CustomBattleId customBattleId, string password = "")
		{
			this.CustomBattleId = customBattleId;
			this.Password = password;
		}
	}
}
