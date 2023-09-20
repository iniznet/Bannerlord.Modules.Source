using System;
using TaleWorlds.Core;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000C0 RID: 192
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateCharacterMessage : Message
	{
		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00003EC9 File Offset: 0x000020C9
		// (set) Token: 0x060002BB RID: 699 RVA: 0x00003ED1 File Offset: 0x000020D1
		public BodyProperties BodyProperties { get; private set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060002BC RID: 700 RVA: 0x00003EDA File Offset: 0x000020DA
		// (set) Token: 0x060002BD RID: 701 RVA: 0x00003EE2 File Offset: 0x000020E2
		public bool IsFemale { get; private set; }

		// Token: 0x060002BE RID: 702 RVA: 0x00003EEB File Offset: 0x000020EB
		public UpdateCharacterMessage(BodyProperties bodyProperties, bool isFemale)
		{
			this.BodyProperties = bodyProperties;
			this.IsFemale = isFemale;
		}
	}
}
