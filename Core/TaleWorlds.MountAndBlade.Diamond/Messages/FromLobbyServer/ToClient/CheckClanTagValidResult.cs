using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200000C RID: 12
	public class CheckClanTagValidResult : FunctionResult
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000021F9 File Offset: 0x000003F9
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002201 File Offset: 0x00000401
		public bool TagExists { get; private set; }

		// Token: 0x0600002B RID: 43 RVA: 0x0000220A File Offset: 0x0000040A
		public CheckClanTagValidResult(bool tagExists)
		{
			this.TagExists = tagExists;
		}
	}
}
