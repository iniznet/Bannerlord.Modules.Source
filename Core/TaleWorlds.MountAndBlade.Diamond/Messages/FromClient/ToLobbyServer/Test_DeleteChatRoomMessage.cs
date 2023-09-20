using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_DeleteChatRoomMessage : Message
	{
		[JsonProperty]
		public Guid ChatRoomId { get; private set; }

		public Test_DeleteChatRoomMessage()
		{
		}

		public Test_DeleteChatRoomMessage(Guid chatRoomId)
		{
			this.ChatRoomId = chatRoomId;
		}
	}
}
