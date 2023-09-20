using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000020 RID: 32
	[Serializable]
	public class GetAnotherPlayerDataMessageResult : FunctionResult
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000071 RID: 113 RVA: 0x0000250A File Offset: 0x0000070A
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00002512 File Offset: 0x00000712
		public PlayerData AnotherPlayerData { get; private set; }

		// Token: 0x06000073 RID: 115 RVA: 0x0000251B File Offset: 0x0000071B
		public GetAnotherPlayerDataMessageResult(PlayerData playerData)
		{
			this.AnotherPlayerData = playerData;
		}
	}
}
