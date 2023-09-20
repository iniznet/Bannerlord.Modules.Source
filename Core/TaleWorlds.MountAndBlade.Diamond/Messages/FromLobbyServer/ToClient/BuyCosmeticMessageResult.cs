using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public class BuyCosmeticMessageResult : FunctionResult
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002103 File Offset: 0x00000303
		public bool Successful { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000210B File Offset: 0x0000030B
		public int Gold { get; }

		// Token: 0x06000015 RID: 21 RVA: 0x00002113 File Offset: 0x00000313
		public BuyCosmeticMessageResult(bool successful, int gold)
		{
			this.Successful = successful;
			this.Gold = gold;
		}
	}
}
