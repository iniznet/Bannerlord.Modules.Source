using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class BuyCosmeticMessage : Message
	{
		[JsonProperty]
		public string CosmeticId { get; private set; }

		public BuyCosmeticMessage()
		{
		}

		public BuyCosmeticMessage(string cosmeticId)
		{
			this.CosmeticId = cosmeticId;
		}
	}
}
