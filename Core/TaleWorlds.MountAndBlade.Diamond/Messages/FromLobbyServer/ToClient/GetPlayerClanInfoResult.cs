using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200002C RID: 44
	[Serializable]
	public class GetPlayerClanInfoResult : FunctionResult
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00002675 File Offset: 0x00000875
		// (set) Token: 0x06000093 RID: 147 RVA: 0x0000267D File Offset: 0x0000087D
		public ClanInfo ClanInfo { get; private set; }

		// Token: 0x06000094 RID: 148 RVA: 0x00002686 File Offset: 0x00000886
		public GetPlayerClanInfoResult(ClanInfo clanInfo)
		{
			this.ClanInfo = clanInfo;
		}
	}
}
