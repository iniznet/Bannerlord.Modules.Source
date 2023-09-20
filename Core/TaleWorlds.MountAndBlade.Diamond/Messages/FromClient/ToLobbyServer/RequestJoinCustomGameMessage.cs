using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RequestJoinCustomGameMessage : Message
	{
		public CustomBattleId CustomBattleId { get; private set; }

		public string Password { get; private set; }

		public RequestJoinCustomGameMessage(CustomBattleId customBattleId, string password = "")
		{
			this.CustomBattleId = customBattleId;
			this.Password = password;
		}
	}
}
