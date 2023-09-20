using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200009E RID: 158
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVertexAnimation : GameNetworkMessage
	{
		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0000BEC2 File Offset: 0x0000A0C2
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x0000BECA File Offset: 0x0000A0CA
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0000BED3 File Offset: 0x0000A0D3
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x0000BEDB File Offset: 0x0000A0DB
		public int BeginKey { get; private set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x0000BEE4 File Offset: 0x0000A0E4
		// (set) Token: 0x06000671 RID: 1649 RVA: 0x0000BEEC File Offset: 0x0000A0EC
		public int EndKey { get; private set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000672 RID: 1650 RVA: 0x0000BEF5 File Offset: 0x0000A0F5
		// (set) Token: 0x06000673 RID: 1651 RVA: 0x0000BEFD File Offset: 0x0000A0FD
		public float Speed { get; private set; }

		// Token: 0x06000674 RID: 1652 RVA: 0x0000BF06 File Offset: 0x0000A106
		public SetMissionObjectVertexAnimation(MissionObject missionObject, int beginKey, int endKey, float speed)
		{
			this.MissionObject = missionObject;
			this.BeginKey = beginKey;
			this.EndKey = endKey;
			this.Speed = speed;
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0000BF2B File Offset: 0x0000A12B
		public SetMissionObjectVertexAnimation()
		{
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0000BF34 File Offset: 0x0000A134
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BeginKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref flag);
			this.EndKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref flag);
			this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.VertexAnimationSpeedCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0000BF87 File Offset: 0x0000A187
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket(this.BeginKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.EndKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionBasic.VertexAnimationSpeedCompressionInfo);
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0000BFC4 File Offset: 0x0000A1C4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0000BFCC File Offset: 0x0000A1CC
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Vertex Animation on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
