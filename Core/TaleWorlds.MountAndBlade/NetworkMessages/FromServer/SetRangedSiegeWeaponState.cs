using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A2 RID: 162
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRangedSiegeWeaponState : GameNetworkMessage
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0000C2F6 File Offset: 0x0000A4F6
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x0000C2FE File Offset: 0x0000A4FE
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0000C307 File Offset: 0x0000A507
		// (set) Token: 0x0600069B RID: 1691 RVA: 0x0000C30F File Offset: 0x0000A50F
		public RangedSiegeWeapon.WeaponState State { get; private set; }

		// Token: 0x0600069C RID: 1692 RVA: 0x0000C318 File Offset: 0x0000A518
		public SetRangedSiegeWeaponState(RangedSiegeWeapon rangedSiegeWeapon, RangedSiegeWeapon.WeaponState state)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.State = state;
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0000C32E File Offset: 0x0000A52E
		public SetRangedSiegeWeaponState()
		{
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0000C338 File Offset: 0x0000A538
		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.RangedSiegeWeapon = missionObject as RangedSiegeWeapon;
			this.State = (RangedSiegeWeapon.WeaponState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0000C36E File Offset: 0x0000A56E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.RangedSiegeWeapon);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0000C38B File Offset: 0x0000A58B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0000C394 File Offset: 0x0000A594
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set RangedSiegeWeapon State to: ",
				this.State,
				" on RangedSiegeWeapon with ID: ",
				this.RangedSiegeWeapon.Id,
				" and name: ",
				this.RangedSiegeWeapon.GameEntity.Name
			});
		}
	}
}
