using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncGoldsForSkirmish : GameNetworkMessage
	{
		public VirtualPlayer VirtualPlayer { get; private set; }

		public int GoldAmount { get; private set; }

		public SyncGoldsForSkirmish()
		{
		}

		public SyncGoldsForSkirmish(VirtualPlayer peer, int goldAmount)
		{
			this.VirtualPlayer = peer;
			this.GoldAmount = goldAmount;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteVirtualPlayerReferenceToPacket(this.VirtualPlayer);
			GameNetworkMessage.WriteIntToPacket(this.GoldAmount, CompressionBasic.RoundGoldAmountCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.VirtualPlayer = GameNetworkMessage.ReadVirtualPlayerReferenceToPacket(ref flag, false);
			this.GoldAmount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.RoundGoldAmountCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Gold amount set to ",
				this.GoldAmount,
				" for ",
				this.VirtualPlayer.UserName,
				"."
			});
		}
	}
}
