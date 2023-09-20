using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000049 RID: 73
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class GoldGain : GameNetworkMessage
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600027B RID: 635 RVA: 0x00005247 File Offset: 0x00003447
		// (set) Token: 0x0600027C RID: 636 RVA: 0x0000524F File Offset: 0x0000344F
		public List<KeyValuePair<ushort, int>> GoldChangeEventList { get; private set; }

		// Token: 0x0600027D RID: 637 RVA: 0x00005258 File Offset: 0x00003458
		public GoldGain(List<KeyValuePair<ushort, int>> goldChangeEventList)
		{
			this.GoldChangeEventList = goldChangeEventList;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00005267 File Offset: 0x00003467
		public GoldGain()
		{
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00005270 File Offset: 0x00003470
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.GoldChangeEventList.Count - 1, CompressionMission.TdmGoldGainTypeCompressionInfo);
			foreach (KeyValuePair<ushort, int> keyValuePair in this.GoldChangeEventList)
			{
				GameNetworkMessage.WriteIntToPacket((int)keyValuePair.Key, CompressionMission.TdmGoldGainTypeCompressionInfo);
				GameNetworkMessage.WriteIntToPacket(keyValuePair.Value, CompressionMission.TdmGoldChangeCompressionInfo);
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x000052F8 File Offset: 0x000034F8
		protected override bool OnRead()
		{
			bool flag = true;
			this.GoldChangeEventList = new List<KeyValuePair<ushort, int>>();
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TdmGoldGainTypeCompressionInfo, ref flag) + 1;
			for (int i = 0; i < num; i++)
			{
				ushort num2 = (ushort)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TdmGoldGainTypeCompressionInfo, ref flag);
				int num3 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TdmGoldChangeCompressionInfo, ref flag);
				this.GoldChangeEventList.Add(new KeyValuePair<ushort, int>(num2, num3));
			}
			return flag;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000535D File Offset: 0x0000355D
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00005365 File Offset: 0x00003565
		protected override string OnGetLogFormat()
		{
			return "Gold change events synced.";
		}
	}
}
