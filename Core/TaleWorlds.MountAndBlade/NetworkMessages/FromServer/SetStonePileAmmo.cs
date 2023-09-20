using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000AA RID: 170
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetStonePileAmmo : GameNetworkMessage
	{
		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0000C9F9 File Offset: 0x0000ABF9
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0000CA01 File Offset: 0x0000AC01
		public StonePile StonePile { get; private set; }

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0000CA0A File Offset: 0x0000AC0A
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x0000CA12 File Offset: 0x0000AC12
		public int AmmoCount { get; private set; }

		// Token: 0x060006E8 RID: 1768 RVA: 0x0000CA1B File Offset: 0x0000AC1B
		public SetStonePileAmmo(StonePile stonePile, int ammoCount)
		{
			this.StonePile = stonePile;
			this.AmmoCount = ammoCount;
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0000CA31 File Offset: 0x0000AC31
		public SetStonePileAmmo()
		{
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0000CA3C File Offset: 0x0000AC3C
		protected override bool OnRead()
		{
			bool flag = true;
			this.StonePile = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as StonePile;
			this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0000CA70 File Offset: 0x0000AC70
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.StonePile);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0000CA8D File Offset: 0x0000AC8D
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0000CA98 File Offset: 0x0000AC98
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set ammo left to: ",
				this.AmmoCount,
				" on StonePile with ID: ",
				this.StonePile.Id,
				" and name: ",
				this.StonePile.GameEntity.Name
			});
		}
	}
}
