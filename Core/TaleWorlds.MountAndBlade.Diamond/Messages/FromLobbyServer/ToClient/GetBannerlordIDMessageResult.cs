using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000024 RID: 36
	[Serializable]
	public class GetBannerlordIDMessageResult : FunctionResult
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00002590 File Offset: 0x00000790
		public string BannerlordID { get; }

		// Token: 0x0600007E RID: 126 RVA: 0x00002598 File Offset: 0x00000798
		public GetBannerlordIDMessageResult(string bannerlordID)
		{
			this.BannerlordID = bannerlordID;
		}
	}
}
