using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_AddChatRoomUser : Message
	{
		[JsonProperty]
		public string Name { get; private set; }

		public Test_AddChatRoomUser()
		{
		}

		public Test_AddChatRoomUser(string name)
		{
			this.Name = name;
		}
	}
}
