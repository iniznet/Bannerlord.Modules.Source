using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200002B RID: 43
	[Serializable]
	public class GetPlayerByUsernameAndIdMessageResult : FunctionResult
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00002655 File Offset: 0x00000855
		// (set) Token: 0x06000090 RID: 144 RVA: 0x0000265D File Offset: 0x0000085D
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000091 RID: 145 RVA: 0x00002666 File Offset: 0x00000866
		public GetPlayerByUsernameAndIdMessageResult(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
