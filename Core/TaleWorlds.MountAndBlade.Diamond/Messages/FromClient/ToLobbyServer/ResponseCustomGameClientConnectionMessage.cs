using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ResponseCustomGameClientConnectionMessage : Message
	{
		public PlayerJoinGameResponseDataFromHost[] PlayerJoinData { get; private set; }

		public ResponseCustomGameClientConnectionMessage(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			this.PlayerJoinData = playerJoinData;
		}
	}
}
