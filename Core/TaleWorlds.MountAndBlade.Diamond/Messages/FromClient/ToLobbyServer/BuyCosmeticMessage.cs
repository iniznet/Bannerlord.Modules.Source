using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class BuyCosmeticMessage : Message
	{
		public string CosmeticId { get; }

		public BuyCosmeticMessage(string cosmeticId)
		{
			this.CosmeticId = cosmeticId;
		}
	}
}
