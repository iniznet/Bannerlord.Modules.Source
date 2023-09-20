using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_DeleteChatRoomMessage : Message
	{
		public Guid ChatRoomId { get; private set; }

		public Test_DeleteChatRoomMessage(Guid chatRoomId)
		{
			this.ChatRoomId = chatRoomId;
		}
	}
}
