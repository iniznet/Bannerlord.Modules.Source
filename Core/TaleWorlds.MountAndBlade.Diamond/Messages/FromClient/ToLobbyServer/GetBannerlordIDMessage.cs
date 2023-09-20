using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetBannerlordIDMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public GetBannerlordIDMessage()
		{
		}

		public GetBannerlordIDMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
