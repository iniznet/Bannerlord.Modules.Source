using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200001D RID: 29
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithMissionObject : GameNetworkMessage
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000323A File Offset: 0x0000143A
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00003242 File Offset: 0x00001442
		public MissionObject MissionObject { get; private set; }

		// Token: 0x060000E7 RID: 231 RVA: 0x0000324B File Offset: 0x0000144B
		public ApplyOrderWithMissionObject(MissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000325A File Offset: 0x0000145A
		public ApplyOrderWithMissionObject()
		{
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00003264 File Offset: 0x00001464
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00003281 File Offset: 0x00001481
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000328E File Offset: 0x0000148E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.Orders;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00003298 File Offset: 0x00001498
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Apply order to MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
