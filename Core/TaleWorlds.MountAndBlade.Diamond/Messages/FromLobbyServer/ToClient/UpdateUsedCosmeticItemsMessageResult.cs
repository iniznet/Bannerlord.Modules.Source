using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000058 RID: 88
	[Serializable]
	public class UpdateUsedCosmeticItemsMessageResult : FunctionResult
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00002E5E File Offset: 0x0000105E
		public bool Successful { get; }

		// Token: 0x06000144 RID: 324 RVA: 0x00002E66 File Offset: 0x00001066
		public UpdateUsedCosmeticItemsMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
