using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200009D RID: 157
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectImpulse : GameNetworkMessage
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0000BDA4 File Offset: 0x00009FA4
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x0000BDAC File Offset: 0x00009FAC
		public MissionObject MissionObject { get; private set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x0000BDB5 File Offset: 0x00009FB5
		// (set) Token: 0x06000663 RID: 1635 RVA: 0x0000BDBD File Offset: 0x00009FBD
		public Vec3 Position { get; private set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x0000BDC6 File Offset: 0x00009FC6
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x0000BDCE File Offset: 0x00009FCE
		public Vec3 Impulse { get; private set; }

		// Token: 0x06000666 RID: 1638 RVA: 0x0000BDD7 File Offset: 0x00009FD7
		public SetMissionObjectImpulse(MissionObject missionObject, Vec3 position, Vec3 impulse)
		{
			this.MissionObject = missionObject;
			this.Position = position;
			this.Impulse = impulse;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0000BDF4 File Offset: 0x00009FF4
		public SetMissionObjectImpulse()
		{
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0000BDFC File Offset: 0x00009FFC
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			this.Impulse = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ImpulseCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0000BE3D File Offset: 0x0000A03D
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Impulse, CompressionBasic.ImpulseCompressionInfo);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0000BE6A File Offset: 0x0000A06A
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0000BE74 File Offset: 0x0000A074
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set impulse on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
