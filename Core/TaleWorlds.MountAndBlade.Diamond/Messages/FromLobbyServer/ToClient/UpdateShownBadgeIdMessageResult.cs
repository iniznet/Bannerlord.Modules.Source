using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000057 RID: 87
	[Serializable]
	public class UpdateShownBadgeIdMessageResult : FunctionResult
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00002E47 File Offset: 0x00001047
		public bool Successful { get; }

		// Token: 0x06000142 RID: 322 RVA: 0x00002E4F File Offset: 0x0000104F
		public UpdateShownBadgeIdMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
