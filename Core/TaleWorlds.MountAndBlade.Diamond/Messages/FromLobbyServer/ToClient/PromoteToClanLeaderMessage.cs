using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200004D RID: 77
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PromoteToClanLeaderMessage : Message
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00002CD2 File Offset: 0x00000ED2
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00002CDA File Offset: 0x00000EDA
		public PlayerId PromotedPlayerId { get; private set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00002CE3 File Offset: 0x00000EE3
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00002CEB File Offset: 0x00000EEB
		public bool DontUseNameForUnknownPlayer { get; private set; }

		// Token: 0x06000123 RID: 291 RVA: 0x00002CF4 File Offset: 0x00000EF4
		public PromoteToClanLeaderMessage(PlayerId promotedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.PromotedPlayerId = promotedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
