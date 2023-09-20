using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000030 RID: 48
	[Serializable]
	public class GetPremadeGameListResult : FunctionResult
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600009E RID: 158 RVA: 0x000026F5 File Offset: 0x000008F5
		// (set) Token: 0x0600009F RID: 159 RVA: 0x000026FD File Offset: 0x000008FD
		public PremadeGameList GameList { get; private set; }

		// Token: 0x060000A0 RID: 160 RVA: 0x00002706 File Offset: 0x00000906
		public GetPremadeGameListResult(PremadeGameList gameList)
		{
			this.GameList = gameList;
		}
	}
}
