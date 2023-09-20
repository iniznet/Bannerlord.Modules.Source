using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateShownBadgeIdMessage : Message
	{
		[JsonProperty]
		public string ShownBadgeId { get; private set; }

		public UpdateShownBadgeIdMessage()
		{
		}

		public UpdateShownBadgeIdMessage(string shownBadgeId)
		{
			this.ShownBadgeId = shownBadgeId;
		}
	}
}
