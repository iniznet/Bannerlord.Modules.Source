using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000099 RID: 153
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectFrame : GameNetworkMessage
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x0000B75F File Offset: 0x0000995F
		// (set) Token: 0x06000635 RID: 1589 RVA: 0x0000B767 File Offset: 0x00009967
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x0000B770 File Offset: 0x00009970
		// (set) Token: 0x06000637 RID: 1591 RVA: 0x0000B778 File Offset: 0x00009978
		public MatrixFrame Frame { get; private set; }

		// Token: 0x06000638 RID: 1592 RVA: 0x0000B781 File Offset: 0x00009981
		public SetMissionObjectFrame(MissionObject missionObject, ref MatrixFrame frame)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0000B79C File Offset: 0x0000999C
		public SetMissionObjectFrame()
		{
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0000B7A4 File Offset: 0x000099A4
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0000B7CE File Offset: 0x000099CE
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0000B7E6 File Offset: 0x000099E6
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0000B7F0 File Offset: 0x000099F0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Frame on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
