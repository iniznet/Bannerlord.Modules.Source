using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_RemoveChatRoomUser : Message
	{
		[JsonProperty]
		public string Name { get; private set; }

		public Test_RemoveChatRoomUser()
		{
		}

		public Test_RemoveChatRoomUser(string name)
		{
			this.Name = name;
		}
	}
}
