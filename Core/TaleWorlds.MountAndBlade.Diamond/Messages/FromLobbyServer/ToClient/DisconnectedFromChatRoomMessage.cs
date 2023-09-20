using System;
using TaleWorlds.Diamond;

namespace TaleWorlds.MountAndBlade.Diamond.Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000163 RID: 355
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DisconnectedFromChatRoomMessage : Message
	{
		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060008D5 RID: 2261 RVA: 0x0000F6F4 File Offset: 0x0000D8F4
		// (set) Token: 0x060008D6 RID: 2262 RVA: 0x0000F6FC File Offset: 0x0000D8FC
		public Guid RoomId { get; private set; }

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x0000F705 File Offset: 0x0000D905
		// (set) Token: 0x060008D8 RID: 2264 RVA: 0x0000F70D File Offset: 0x0000D90D
		public string RoomName { get; private set; }

		// Token: 0x060008D9 RID: 2265 RVA: 0x0000F716 File Offset: 0x0000D916
		public DisconnectedFromChatRoomMessage(Guid roomId, string roomName)
		{
			this.RoomId = roomId;
			this.RoomName = roomName;
		}
	}
}
