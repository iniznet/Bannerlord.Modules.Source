using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200002A RID: 42
	[Serializable]
	public class GetPlayerBadgesMessageResult : FunctionResult
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00002635 File Offset: 0x00000835
		// (set) Token: 0x0600008D RID: 141 RVA: 0x0000263D File Offset: 0x0000083D
		public string[] Badges { get; private set; }

		// Token: 0x0600008E RID: 142 RVA: 0x00002646 File Offset: 0x00000846
		public GetPlayerBadgesMessageResult(string[] badges)
		{
			this.Badges = badges;
		}
	}
}
