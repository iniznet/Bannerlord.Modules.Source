using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000059 RID: 89
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncGoldsForSkirmish : GameNetworkMessage
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0000635C File Offset: 0x0000455C
		// (set) Token: 0x0600031B RID: 795 RVA: 0x00006364 File Offset: 0x00004564
		public VirtualPlayer VirtualPlayer { get; private set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000636D File Offset: 0x0000456D
		// (set) Token: 0x0600031D RID: 797 RVA: 0x00006375 File Offset: 0x00004575
		public int GoldAmount { get; private set; }

		// Token: 0x0600031E RID: 798 RVA: 0x0000637E File Offset: 0x0000457E
		public SyncGoldsForSkirmish()
		{
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00006386 File Offset: 0x00004586
		public SyncGoldsForSkirmish(VirtualPlayer peer, int goldAmount)
		{
			this.VirtualPlayer = peer;
			this.GoldAmount = goldAmount;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0000639C File Offset: 0x0000459C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteVirtualPlayerReferenceToPacket(this.VirtualPlayer);
			GameNetworkMessage.WriteIntToPacket(this.GoldAmount, CompressionBasic.RoundGoldAmountCompressionInfo);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x000063BC File Offset: 0x000045BC
		protected override bool OnRead()
		{
			bool flag = true;
			this.VirtualPlayer = GameNetworkMessage.ReadVirtualPlayerReferenceToPacket(ref flag, false);
			this.GoldAmount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.RoundGoldAmountCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000322 RID: 802 RVA: 0x000063EC File Offset: 0x000045EC
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000063F4 File Offset: 0x000045F4
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
