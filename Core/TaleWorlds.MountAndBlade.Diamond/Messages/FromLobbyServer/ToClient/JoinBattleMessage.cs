using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000037 RID: 55
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinBattleMessage : Message
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000028AD File Offset: 0x00000AAD
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x000028B5 File Offset: 0x00000AB5
		public BattleServerInformationForClient BattleServerInformation { get; private set; }

		// Token: 0x060000C6 RID: 198 RVA: 0x000028BE File Offset: 0x00000ABE
		public JoinBattleMessage(BattleServerInformationForClient battleServerInformation)
		{
			this.BattleServerInformation = battleServerInformation;
		}
	}
}
