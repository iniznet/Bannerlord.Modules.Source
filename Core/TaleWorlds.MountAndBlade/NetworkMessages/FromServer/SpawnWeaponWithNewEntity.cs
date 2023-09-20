using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B4 RID: 180
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnWeaponWithNewEntity : GameNetworkMessage
	{
		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x0000D73E File Offset: 0x0000B93E
		// (set) Token: 0x06000765 RID: 1893 RVA: 0x0000D746 File Offset: 0x0000B946
		public MissionWeapon Weapon { get; private set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x0000D74F File Offset: 0x0000B94F
		// (set) Token: 0x06000767 RID: 1895 RVA: 0x0000D757 File Offset: 0x0000B957
		public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x0000D760 File Offset: 0x0000B960
		// (set) Token: 0x06000769 RID: 1897 RVA: 0x0000D768 File Offset: 0x0000B968
		public int ForcedIndex { get; private set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x0600076A RID: 1898 RVA: 0x0000D771 File Offset: 0x0000B971
		// (set) Token: 0x0600076B RID: 1899 RVA: 0x0000D779 File Offset: 0x0000B979
		public MatrixFrame Frame { get; private set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x0000D782 File Offset: 0x0000B982
		// (set) Token: 0x0600076D RID: 1901 RVA: 0x0000D78A File Offset: 0x0000B98A
		public MissionObject ParentMissionObject { get; private set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x0000D793 File Offset: 0x0000B993
		// (set) Token: 0x0600076F RID: 1903 RVA: 0x0000D79B File Offset: 0x0000B99B
		public bool IsVisible { get; private set; }

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x0000D7A4 File Offset: 0x0000B9A4
		// (set) Token: 0x06000771 RID: 1905 RVA: 0x0000D7AC File Offset: 0x0000B9AC
		public bool HasLifeTime { get; private set; }

		// Token: 0x06000772 RID: 1906 RVA: 0x0000D7B5 File Offset: 0x0000B9B5
		public SpawnWeaponWithNewEntity(MissionWeapon weapon, Mission.WeaponSpawnFlags weaponSpawnFlags, int forcedIndex, MatrixFrame frame, MissionObject parentMissionObject, bool isVisible, bool hasLifeTime)
		{
			this.Weapon = weapon;
			this.WeaponSpawnFlags = weaponSpawnFlags;
			this.ForcedIndex = forcedIndex;
			this.Frame = frame;
			this.ParentMissionObject = parentMissionObject;
			this.IsVisible = isVisible;
			this.HasLifeTime = hasLifeTime;
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x0000D7F2 File Offset: 0x0000B9F2
		public SpawnWeaponWithNewEntity()
		{
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x0000D7FC File Offset: 0x0000B9FC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			this.WeaponSpawnFlags = (Mission.WeaponSpawnFlags)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			this.ParentMissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.IsVisible = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.HasLifeTime = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x0000D878 File Offset: 0x0000BA78
		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponSpawnFlags, CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.ParentMissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.IsVisible);
			GameNetworkMessage.WriteBoolToPacket(this.HasLifeTime);
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x0000D8DC File Offset: 0x0000BADC
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x0000D8E0 File Offset: 0x0000BAE0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Spawn Weapon with name: ",
				this.Weapon.Item.Name,
				", and with ID: ",
				this.ForcedIndex
			});
		}
	}
}
