using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class GoldGain : GameNetworkMessage
	{
		public List<KeyValuePair<ushort, int>> GoldChangeEventList { get; private set; }

		public GoldGain(List<KeyValuePair<ushort, int>> goldChangeEventList)
		{
			this.GoldChangeEventList = goldChangeEventList;
		}

		public GoldGain()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.GoldChangeEventList.Count - 1, CompressionMission.TdmGoldGainTypeCompressionInfo);
			foreach (KeyValuePair<ushort, int> keyValuePair in this.GoldChangeEventList)
			{
				GameNetworkMessage.WriteIntToPacket((int)keyValuePair.Key, CompressionMission.TdmGoldGainTypeCompressionInfo);
				GameNetworkMessage.WriteIntToPacket(keyValuePair.Value, CompressionMission.TdmGoldChangeCompressionInfo);
			}
		}

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

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Gold change events synced.";
		}
	}
}
