using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000C3 RID: 195
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateUsedCosmeticItemsMessage : Message
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x00003F38 File Offset: 0x00002138
		public List<CosmeticItemInfo> UsedCosmetics { get; }

		// Token: 0x060002C5 RID: 709 RVA: 0x00003F40 File Offset: 0x00002140
		public UpdateUsedCosmeticItemsMessage(List<CosmeticItemInfo> usedCosmetics)
		{
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
