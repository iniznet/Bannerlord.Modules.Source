using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitedPlayerOnlineMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; set; }

		public InvitedPlayerOnlineMessage()
		{
		}

		public InvitedPlayerOnlineMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
