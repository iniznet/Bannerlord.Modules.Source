using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000025 RID: 37
	[Serializable]
	public class GetClanHomeInfoResult : FunctionResult
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000025A7 File Offset: 0x000007A7
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000025AF File Offset: 0x000007AF
		public ClanHomeInfo ClanHomeInfo { get; private set; }

		// Token: 0x06000081 RID: 129 RVA: 0x000025B8 File Offset: 0x000007B8
		public GetClanHomeInfoResult(ClanHomeInfo clanHomeInfo)
		{
			this.ClanHomeInfo = clanHomeInfo;
		}
	}
}
