using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000020 RID: 32
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplySiegeWeaponOrder : GameNetworkMessage
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000103 RID: 259 RVA: 0x000034F2 File Offset: 0x000016F2
		// (set) Token: 0x06000104 RID: 260 RVA: 0x000034FA File Offset: 0x000016FA
		public SiegeWeaponOrderType OrderType { get; private set; }

		// Token: 0x06000105 RID: 261 RVA: 0x00003503 File Offset: 0x00001703
		public ApplySiegeWeaponOrder(SiegeWeaponOrderType orderType)
		{
			this.OrderType = orderType;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00003512 File Offset: 0x00001712
		public ApplySiegeWeaponOrder()
		{
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000351C File Offset: 0x0000171C
		protected override bool OnRead()
		{
			bool flag = true;
			this.OrderType = (SiegeWeaponOrderType)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000353E File Offset: 0x0000173E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00003550 File Offset: 0x00001750
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed | MultiplayerMessageFilter.Orders;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00003558 File Offset: 0x00001758
		protected override string OnGetLogFormat()
		{
			return "Apply siege order: " + this.OrderType;
		}
	}
}
