using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000087 RID: 135
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveMissionObject : GameNetworkMessage
	{
		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x0000A2F3 File Offset: 0x000084F3
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x0000A2FB File Offset: 0x000084FB
		public MissionObjectId ObjectId { get; private set; }

		// Token: 0x06000569 RID: 1385 RVA: 0x0000A304 File Offset: 0x00008504
		public RemoveMissionObject(MissionObjectId objectId)
		{
			this.ObjectId = objectId;
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0000A313 File Offset: 0x00008513
		public RemoveMissionObject()
		{
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0000A31C File Offset: 0x0000851C
		protected override bool OnRead()
		{
			bool flag = true;
			this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0000A339 File Offset: 0x00008539
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0000A346 File Offset: 0x00008546
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0000A34E File Offset: 0x0000854E
		protected override string OnGetLogFormat()
		{
			return "Remove MissionObject with ID: " + this.ObjectId;
		}
	}
}
