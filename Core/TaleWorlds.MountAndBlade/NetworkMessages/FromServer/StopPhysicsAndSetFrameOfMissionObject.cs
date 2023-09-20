using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B6 RID: 182
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StopPhysicsAndSetFrameOfMissionObject : GameNetworkMessage
	{
		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x0000DAAD File Offset: 0x0000BCAD
		// (set) Token: 0x06000787 RID: 1927 RVA: 0x0000DAB5 File Offset: 0x0000BCB5
		public MissionObjectId ObjectId { get; private set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x0000DABE File Offset: 0x0000BCBE
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x0000DAC6 File Offset: 0x0000BCC6
		public MissionObject Parent { get; private set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x0000DACF File Offset: 0x0000BCCF
		// (set) Token: 0x0600078B RID: 1931 RVA: 0x0000DAD7 File Offset: 0x0000BCD7
		public MatrixFrame Frame { get; private set; }

		// Token: 0x0600078C RID: 1932 RVA: 0x0000DAE0 File Offset: 0x0000BCE0
		public StopPhysicsAndSetFrameOfMissionObject(MissionObjectId objectId, MissionObject parent, MatrixFrame frame)
		{
			this.ObjectId = objectId;
			this.Parent = parent;
			this.Frame = frame;
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x0000DAFD File Offset: 0x0000BCFD
		public StopPhysicsAndSetFrameOfMissionObject()
		{
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x0000DB08 File Offset: 0x0000BD08
		protected override bool OnRead()
		{
			bool flag = true;
			this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Parent = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x0000DB49 File Offset: 0x0000BD49
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.Parent);
			GameNetworkMessage.WriteNonUniformTransformToPacket(this.Frame, CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0000DB76 File Offset: 0x0000BD76
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x0000DB80 File Offset: 0x0000BD80
		protected override string OnGetLogFormat()
		{
			object[] array = new object[4];
			array[0] = "Stop physics and set frame of MissionObject with ID: ";
			array[1] = this.ObjectId;
			array[2] = " Parent Index: ";
			int num = 3;
			MissionObject parent = this.Parent;
			array[num] = ((parent != null) ? parent.Id.ToString() : null) ?? "-1";
			return string.Concat(array);
		}
	}
}
