using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200002E RID: 46
	[Serializable]
	public class GetPlayerGameTypeRankInfoMessageResult : FunctionResult
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000026B5 File Offset: 0x000008B5
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000026BD File Offset: 0x000008BD
		public GameTypeRankInfo[] GameTypeRankInfo { get; private set; }

		// Token: 0x0600009A RID: 154 RVA: 0x000026C6 File Offset: 0x000008C6
		public GetPlayerGameTypeRankInfoMessageResult(GameTypeRankInfo[] gameTypeRankInfo)
		{
			this.GameTypeRankInfo = gameTypeRankInfo;
		}
	}
}
