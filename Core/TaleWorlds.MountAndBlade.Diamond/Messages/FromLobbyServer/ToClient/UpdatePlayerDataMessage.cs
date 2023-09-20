using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class UpdatePlayerDataMessage : Message
	{
		[JsonProperty]
		public PlayerData PlayerData { get; private set; }

		public UpdatePlayerDataMessage()
		{
		}

		public UpdatePlayerDataMessage(PlayerData playerData)
		{
			this.PlayerData = playerData;
		}
	}
}
