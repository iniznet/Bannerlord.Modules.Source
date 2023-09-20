using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	// Token: 0x02000069 RID: 105
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class ResponseCustomGameClientConnectionMessage : Message
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00003320 File Offset: 0x00001520
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00003328 File Offset: 0x00001528
		public PlayerJoinGameResponseDataFromHost[] PlayerJoinData { get; private set; }

		// Token: 0x060001AC RID: 428 RVA: 0x00003331 File Offset: 0x00001531
		public ResponseCustomGameClientConnectionMessage(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			this.PlayerJoinData = playerJoinData;
		}
	}
}
