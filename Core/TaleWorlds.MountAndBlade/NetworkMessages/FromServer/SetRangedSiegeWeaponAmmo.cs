using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A3 RID: 163
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRangedSiegeWeaponAmmo : GameNetworkMessage
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0000C3F8 File Offset: 0x0000A5F8
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0000C400 File Offset: 0x0000A600
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x0000C409 File Offset: 0x0000A609
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x0000C411 File Offset: 0x0000A611
		public int AmmoCount { get; private set; }

		// Token: 0x060006A6 RID: 1702 RVA: 0x0000C41A File Offset: 0x0000A61A
		public SetRangedSiegeWeaponAmmo(RangedSiegeWeapon rangedSiegeWeapon, int ammoCount)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.AmmoCount = ammoCount;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0000C430 File Offset: 0x0000A630
		public SetRangedSiegeWeaponAmmo()
		{
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0000C438 File Offset: 0x0000A638
		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as RangedSiegeWeapon;
			this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0000C46C File Offset: 0x0000A66C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.RangedSiegeWeapon);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0000C489 File Offset: 0x0000A689
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0000C494 File Offset: 0x0000A694
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set ammo left to: ",
				this.AmmoCount,
				" on RangedSiegeWeapon with ID: ",
				this.RangedSiegeWeapon.Id,
				" and name: ",
				this.RangedSiegeWeapon.GameEntity.Name
			});
		}
	}
}
