using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F4 RID: 244
	[Serializable]
	public class ChatRoomInformationForClient
	{
		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x000064B8 File Offset: 0x000046B8
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x000064C0 File Offset: 0x000046C0
		public Guid RoomId { get; private set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x000064C9 File Offset: 0x000046C9
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x000064D1 File Offset: 0x000046D1
		public string Name { get; private set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x000064DA File Offset: 0x000046DA
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x000064E2 File Offset: 0x000046E2
		public string Endpoint { get; private set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x000064EB File Offset: 0x000046EB
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x000064F3 File Offset: 0x000046F3
		public string RoomColor { get; private set; }

		// Token: 0x0600044A RID: 1098 RVA: 0x000064FC File Offset: 0x000046FC
		public ChatRoomInformationForClient(Guid roomId, string name, string endpoint, string color)
		{
			this.RoomId = roomId;
			this.Name = name;
			this.Endpoint = endpoint;
			this.RoomColor = color;
		}
	}
}
