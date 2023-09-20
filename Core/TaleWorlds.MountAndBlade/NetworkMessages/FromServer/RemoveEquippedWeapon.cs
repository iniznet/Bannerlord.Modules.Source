using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000086 RID: 134
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveEquippedWeapon : GameNetworkMessage
	{
		// Token: 0x17000130 RID: 304
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x0000A1FE File Offset: 0x000083FE
		// (set) Token: 0x0600055E RID: 1374 RVA: 0x0000A206 File Offset: 0x00008406
		public EquipmentIndex SlotIndex { get; private set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x0000A20F File Offset: 0x0000840F
		// (set) Token: 0x06000560 RID: 1376 RVA: 0x0000A217 File Offset: 0x00008417
		public Agent Agent { get; private set; }

		// Token: 0x06000561 RID: 1377 RVA: 0x0000A220 File Offset: 0x00008420
		public RemoveEquippedWeapon(Agent a, EquipmentIndex slot)
		{
			this.Agent = a;
			this.SlotIndex = slot;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x0000A236 File Offset: 0x00008436
		public RemoveEquippedWeapon()
		{
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0000A23E File Offset: 0x0000843E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0000A25C File Offset: 0x0000845C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0000A28C File Offset: 0x0000848C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0000A294 File Offset: 0x00008494
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Remove equipped weapon from SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
