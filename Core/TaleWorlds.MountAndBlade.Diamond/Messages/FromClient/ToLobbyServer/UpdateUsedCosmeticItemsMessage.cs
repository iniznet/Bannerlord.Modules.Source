using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateUsedCosmeticItemsMessage : Message
	{
		public List<CosmeticItemInfo> UsedCosmetics { get; }

		public UpdateUsedCosmeticItemsMessage(List<CosmeticItemInfo> usedCosmetics)
		{
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
