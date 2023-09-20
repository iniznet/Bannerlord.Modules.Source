using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200002E RID: 46
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SelectSiegeWeapon : GameNetworkMessage
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00003A14 File Offset: 0x00001C14
		// (set) Token: 0x06000166 RID: 358 RVA: 0x00003A1C File Offset: 0x00001C1C
		public SiegeWeapon SiegeWeapon { get; private set; }

		// Token: 0x06000167 RID: 359 RVA: 0x00003A25 File Offset: 0x00001C25
		public SelectSiegeWeapon(SiegeWeapon siegeWeapon)
		{
			this.SiegeWeapon = siegeWeapon;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00003A34 File Offset: 0x00001C34
		public SelectSiegeWeapon()
		{
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00003A3C File Offset: 0x00001C3C
		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SiegeWeapon;
			return flag;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00003A5E File Offset: 0x00001C5E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeWeapon);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00003A6B File Offset: 0x00001C6B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00003A74 File Offset: 0x00001C74
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Select SiegeWeapon with ID: ",
				this.SiegeWeapon.Id,
				" and with name: ",
				this.SiegeWeapon.GameEntity.Name
			});
		}
	}
}
