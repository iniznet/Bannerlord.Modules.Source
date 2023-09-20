using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000C2 RID: 194
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateShownBadgeIdMessage : Message
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00003F18 File Offset: 0x00002118
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x00003F20 File Offset: 0x00002120
		public string ShownBadgeId { get; private set; }

		// Token: 0x060002C3 RID: 707 RVA: 0x00003F29 File Offset: 0x00002129
		public UpdateShownBadgeIdMessage(string shownBadgeId)
		{
			this.ShownBadgeId = shownBadgeId;
		}
	}
}
