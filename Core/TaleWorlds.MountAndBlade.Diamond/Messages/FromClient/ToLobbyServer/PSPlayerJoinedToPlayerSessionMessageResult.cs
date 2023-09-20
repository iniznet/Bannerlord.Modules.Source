using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000AE RID: 174
	[Serializable]
	public class PSPlayerJoinedToPlayerSessionMessageResult : FunctionResult
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000273 RID: 627 RVA: 0x00003B99 File Offset: 0x00001D99
		// (set) Token: 0x06000274 RID: 628 RVA: 0x00003BA1 File Offset: 0x00001DA1
		public bool Successful { get; private set; }

		// Token: 0x06000275 RID: 629 RVA: 0x00003BAA File Offset: 0x00001DAA
		public PSPlayerJoinedToPlayerSessionMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
