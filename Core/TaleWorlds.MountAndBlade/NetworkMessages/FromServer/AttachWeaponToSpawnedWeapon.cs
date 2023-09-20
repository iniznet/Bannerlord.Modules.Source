using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000069 RID: 105
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AttachWeaponToSpawnedWeapon : GameNetworkMessage
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060003CA RID: 970 RVA: 0x0000748C File Offset: 0x0000568C
		// (set) Token: 0x060003CB RID: 971 RVA: 0x00007494 File Offset: 0x00005694
		public MissionWeapon Weapon { get; private set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060003CC RID: 972 RVA: 0x0000749D File Offset: 0x0000569D
		// (set) Token: 0x060003CD RID: 973 RVA: 0x000074A5 File Offset: 0x000056A5
		public MissionObject MissionObject { get; private set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060003CE RID: 974 RVA: 0x000074AE File Offset: 0x000056AE
		// (set) Token: 0x060003CF RID: 975 RVA: 0x000074B6 File Offset: 0x000056B6
		public MatrixFrame AttachLocalFrame { get; private set; }

		// Token: 0x060003D0 RID: 976 RVA: 0x000074BF File Offset: 0x000056BF
		public AttachWeaponToSpawnedWeapon(MissionWeapon weapon, MissionObject missionObject, MatrixFrame attachLocalFrame)
		{
			this.Weapon = weapon;
			this.MissionObject = missionObject;
			this.AttachLocalFrame = attachLocalFrame;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x000074DC File Offset: 0x000056DC
		public AttachWeaponToSpawnedWeapon()
		{
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x000074E4 File Offset: 0x000056E4
		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00007524 File Offset: 0x00005724
		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			Mat3 mat = GameNetworkMessage.ReadRotationMatrixFromPacket(ref flag);
			if (flag)
			{
				this.AttachLocalFrame = new MatrixFrame(mat, vec);
			}
			return flag;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00007578 File Offset: 0x00005778
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00007580 File Offset: 0x00005780
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"AttachWeaponToSpawnedWeapon with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" to MissionObject: ",
				this.MissionObject.Id.Id
			});
		}
	}
}
