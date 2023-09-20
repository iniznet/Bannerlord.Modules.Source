using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000073 RID: 115
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class BuyCosmeticMessage : Message
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000344E File Offset: 0x0000164E
		public string CosmeticId { get; }

		// Token: 0x060001C7 RID: 455 RVA: 0x00003456 File Offset: 0x00001656
		public BuyCosmeticMessage(string cosmeticId)
		{
			this.CosmeticId = cosmeticId;
		}
	}
}
