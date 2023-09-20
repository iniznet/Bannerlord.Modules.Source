using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class TeamInitialPerkInfoMessage : GameNetworkMessage
	{
		public int[] Perks { get; private set; }

		public TeamInitialPerkInfoMessage(int[] perks)
		{
			this.Perks = perks;
		}

		public TeamInitialPerkInfoMessage()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Perks = new int[3];
			for (int i = 0; i < 3; i++)
			{
				this.Perks[i] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override void OnWrite()
		{
			for (int i = 0; i < 3; i++)
			{
				GameNetworkMessage.WriteIntToPacket(this.Perks[i], CompressionMission.PerkIndexCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		protected override string OnGetLogFormat()
		{
			return "TeamInitialPerkInfoMessage";
		}
	}
}
