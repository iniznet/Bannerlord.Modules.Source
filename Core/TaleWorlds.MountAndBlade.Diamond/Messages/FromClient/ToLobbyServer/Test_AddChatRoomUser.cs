using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_AddChatRoomUser : Message
	{
		public string Name { get; private set; }

		public Test_AddChatRoomUser(string name)
		{
			this.Name = name;
		}
	}
}
