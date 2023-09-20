using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000038 RID: 56
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinChatRoomMessage : Message
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x000028CD File Offset: 0x00000ACD
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x000028D5 File Offset: 0x00000AD5
		public ChatRoomInformationForClient ChatRoomInformaton { get; private set; }

		// Token: 0x060000C9 RID: 201 RVA: 0x000028DE File Offset: 0x00000ADE
		public JoinChatRoomMessage(ChatRoomInformationForClient chatRoomInformation)
		{
			this.ChatRoomInformaton = chatRoomInformation;
		}
	}
}
