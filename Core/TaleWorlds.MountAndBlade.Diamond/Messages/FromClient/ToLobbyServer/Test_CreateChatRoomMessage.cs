using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_CreateChatRoomMessage : Message
	{
		[JsonProperty]
		public string Name { get; private set; }

		public Test_CreateChatRoomMessage()
		{
		}

		public Test_CreateChatRoomMessage(string name)
		{
			this.Name = name;
		}
	}
}
