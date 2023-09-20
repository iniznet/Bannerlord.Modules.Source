using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000033 RID: 51
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class UnselectSiegeWeapon : GameNetworkMessage
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00003D7C File Offset: 0x00001F7C
		// (set) Token: 0x06000194 RID: 404 RVA: 0x00003D84 File Offset: 0x00001F84
		public SiegeWeapon SiegeWeapon { get; private set; }

		// Token: 0x06000195 RID: 405 RVA: 0x00003D8D File Offset: 0x00001F8D
		public UnselectSiegeWeapon(SiegeWeapon siegeWeapon)
		{
			this.SiegeWeapon = siegeWeapon;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00003D9C File Offset: 0x00001F9C
		public UnselectSiegeWeapon()
		{
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00003DA4 File Offset: 0x00001FA4
		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SiegeWeapon;
			return flag;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00003DC6 File Offset: 0x00001FC6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeWeapon);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00003DD3 File Offset: 0x00001FD3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00003DDC File Offset: 0x00001FDC
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Deselect SiegeWeapon with ID: ",
				this.SiegeWeapon.Id,
				" and with name: ",
				this.SiegeWeapon.GameEntity.Name
			});
		}
	}
}
