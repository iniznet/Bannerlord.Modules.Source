using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200000B RID: 11
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestChangePreferredTroopType : GameNetworkMessage
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000025E1 File Offset: 0x000007E1
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000025E9 File Offset: 0x000007E9
		public TroopType TroopType { get; private set; }

		// Token: 0x06000044 RID: 68 RVA: 0x000025F2 File Offset: 0x000007F2
		public RequestChangePreferredTroopType(TroopType troopType)
		{
			this.TroopType = troopType;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002601 File Offset: 0x00000801
		public RequestChangePreferredTroopType()
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002609 File Offset: 0x00000809
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.TroopType, CompressionBasic.TroopTypeCompressionInfo);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000261C File Offset: 0x0000081C
		protected override bool OnRead()
		{
			bool flag = true;
			this.TroopType = (TroopType)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.TroopTypeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000263E File Offset: 0x0000083E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002646 File Offset: 0x00000846
		protected override string OnGetLogFormat()
		{
			return "Peer requesting preferred troop type change to " + this.TroopType;
		}
	}
}
