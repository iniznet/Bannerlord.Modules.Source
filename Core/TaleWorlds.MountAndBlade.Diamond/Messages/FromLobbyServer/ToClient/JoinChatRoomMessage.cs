using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinChatRoomMessage : Message
	{
		public ChatRoomInformationForClient ChatRoomInformaton { get; private set; }

		public JoinChatRoomMessage(ChatRoomInformationForClient chatRoomInformation)
		{
			this.ChatRoomInformaton = chatRoomInformation;
		}
	}
}
