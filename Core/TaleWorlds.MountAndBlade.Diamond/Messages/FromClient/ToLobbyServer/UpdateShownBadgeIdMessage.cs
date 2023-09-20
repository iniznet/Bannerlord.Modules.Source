using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateShownBadgeIdMessage : Message
	{
		public string ShownBadgeId { get; private set; }

		public UpdateShownBadgeIdMessage(string shownBadgeId)
		{
			this.ShownBadgeId = shownBadgeId;
		}
	}
}
