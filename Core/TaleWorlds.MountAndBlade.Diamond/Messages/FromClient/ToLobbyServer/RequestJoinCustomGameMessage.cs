using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RequestJoinCustomGameMessage : Message
	{
		[JsonProperty]
		public CustomBattleId CustomBattleId { get; private set; }

		[JsonProperty]
		public string Password { get; private set; }

		public RequestJoinCustomGameMessage()
		{
		}

		public RequestJoinCustomGameMessage(CustomBattleId customBattleId, string password = "")
		{
			this.CustomBattleId = customBattleId;
			this.Password = password;
		}
	}
}
