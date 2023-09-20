using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000068 RID: 104
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AttachWeaponToAgent : GameNetworkMessage
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060003BC RID: 956 RVA: 0x000072BF File Offset: 0x000054BF
		// (set) Token: 0x060003BD RID: 957 RVA: 0x000072C7 File Offset: 0x000054C7
		public MissionWeapon Weapon { get; private set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000072D0 File Offset: 0x000054D0
		// (set) Token: 0x060003BF RID: 959 RVA: 0x000072D8 File Offset: 0x000054D8
		public Agent Agent { get; private set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x000072E1 File Offset: 0x000054E1
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x000072E9 File Offset: 0x000054E9
		public sbyte BoneIndex { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x000072F2 File Offset: 0x000054F2
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x000072FA File Offset: 0x000054FA
		public MatrixFrame AttachLocalFrame { get; private set; }

		// Token: 0x060003C4 RID: 964 RVA: 0x00007303 File Offset: 0x00005503
		public AttachWeaponToAgent(MissionWeapon weapon, Agent agent, sbyte boneIndex, MatrixFrame attachLocalFrame)
		{
			this.Weapon = weapon;
			this.Agent = agent;
			this.BoneIndex = boneIndex;
			this.AttachLocalFrame = attachLocalFrame;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00007328 File Offset: 0x00005528
		public AttachWeaponToAgent()
		{
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00007330 File Offset: 0x00005530
		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.BoneIndex, CompressionMission.BoneIndexCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00007388 File Offset: 0x00005588
		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.BoneIndex = (sbyte)GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref flag);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			Mat3 mat = GameNetworkMessage.ReadRotationMatrixFromPacket(ref flag);
			if (flag)
			{
				this.AttachLocalFrame = new MatrixFrame(mat, vec);
			}
			return flag;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000073F0 File Offset: 0x000055F0
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x000073F8 File Offset: 0x000055F8
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"AttachWeaponToAgent with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" to bone index: ",
				this.BoneIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
