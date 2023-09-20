using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000083 RID: 131
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RangedSiegeWeaponChangeProjectile : GameNetworkMessage
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x00009FBA File Offset: 0x000081BA
		// (set) Token: 0x06000542 RID: 1346 RVA: 0x00009FC2 File Offset: 0x000081C2
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x00009FCB File Offset: 0x000081CB
		// (set) Token: 0x06000544 RID: 1348 RVA: 0x00009FD3 File Offset: 0x000081D3
		public int Index { get; private set; }

		// Token: 0x06000545 RID: 1349 RVA: 0x00009FDC File Offset: 0x000081DC
		public RangedSiegeWeaponChangeProjectile(RangedSiegeWeapon rangedSiegeWeapon, int index)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.Index = index;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00009FF2 File Offset: 0x000081F2
		public RangedSiegeWeaponChangeProjectile()
		{
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00009FFC File Offset: 0x000081FC
		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as RangedSiegeWeapon;
			this.Index = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0000A030 File Offset: 0x00008230
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.RangedSiegeWeapon);
			GameNetworkMessage.WriteIntToPacket(this.Index, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0000A04D File Offset: 0x0000824D
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0000A058 File Offset: 0x00008258
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Changed Projectile Type Index to: ",
				this.Index,
				" on RangedSiegeWeapon with ID: ",
				this.RangedSiegeWeapon.Id,
				" and name: ",
				this.RangedSiegeWeapon.GameEntity.Name
			});
		}
	}
}
