using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200000D RID: 13
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestTroopIndexChange : GameNetworkMessage
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002734 File Offset: 0x00000934
		// (set) Token: 0x06000055 RID: 85 RVA: 0x0000273C File Offset: 0x0000093C
		public int SelectedTroopIndex { get; private set; }

		// Token: 0x06000056 RID: 86 RVA: 0x00002745 File Offset: 0x00000945
		public RequestTroopIndexChange(int selectedTroopIndex)
		{
			this.SelectedTroopIndex = selectedTroopIndex;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002754 File Offset: 0x00000954
		public RequestTroopIndexChange()
		{
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000275C File Offset: 0x0000095C
		protected override bool OnRead()
		{
			bool flag = true;
			this.SelectedTroopIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SelectedTroopIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000277E File Offset: 0x0000097E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.SelectedTroopIndex, CompressionMission.SelectedTroopIndexCompressionInfo);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002790 File Offset: 0x00000990
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002795 File Offset: 0x00000995
		protected override string OnGetLogFormat()
		{
			return "Requesting selected troop change to " + this.SelectedTroopIndex;
		}
	}
}
