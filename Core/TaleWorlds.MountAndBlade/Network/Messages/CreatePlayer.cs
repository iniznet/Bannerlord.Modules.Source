using System;

namespace TaleWorlds.MountAndBlade.Network.Messages
{
	// Token: 0x020003B4 RID: 948
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreatePlayer : GameNetworkMessage
	{
		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06003329 RID: 13097 RVA: 0x000D4901 File Offset: 0x000D2B01
		// (set) Token: 0x0600332A RID: 13098 RVA: 0x000D4909 File Offset: 0x000D2B09
		public int PlayerIndex { get; private set; }

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x0600332B RID: 13099 RVA: 0x000D4912 File Offset: 0x000D2B12
		// (set) Token: 0x0600332C RID: 13100 RVA: 0x000D491A File Offset: 0x000D2B1A
		public string PlayerName { get; private set; }

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x0600332D RID: 13101 RVA: 0x000D4923 File Offset: 0x000D2B23
		// (set) Token: 0x0600332E RID: 13102 RVA: 0x000D492B File Offset: 0x000D2B2B
		public int DisconnectedPeerIndex { get; private set; }

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x0600332F RID: 13103 RVA: 0x000D4934 File Offset: 0x000D2B34
		// (set) Token: 0x06003330 RID: 13104 RVA: 0x000D493C File Offset: 0x000D2B3C
		public bool IsNonExistingDisconnectedPeer { get; private set; }

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06003331 RID: 13105 RVA: 0x000D4945 File Offset: 0x000D2B45
		// (set) Token: 0x06003332 RID: 13106 RVA: 0x000D494D File Offset: 0x000D2B4D
		public bool IsReceiverPeer { get; private set; }

		// Token: 0x06003333 RID: 13107 RVA: 0x000D4956 File Offset: 0x000D2B56
		public CreatePlayer(int playerIndex, string playerName, int disconnectedPeerIndex, bool isNonExistingDisconnectedPeer = false, bool isReceiverPeer = false)
		{
			this.PlayerIndex = playerIndex;
			this.PlayerName = playerName;
			this.DisconnectedPeerIndex = disconnectedPeerIndex;
			this.IsNonExistingDisconnectedPeer = isNonExistingDisconnectedPeer;
			this.IsReceiverPeer = isReceiverPeer;
		}

		// Token: 0x06003334 RID: 13108 RVA: 0x000D4983 File Offset: 0x000D2B83
		public CreatePlayer()
		{
		}

		// Token: 0x06003335 RID: 13109 RVA: 0x000D498C File Offset: 0x000D2B8C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.PlayerIndex, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteStringToPacket(this.PlayerName);
			GameNetworkMessage.WriteIntToPacket(this.DisconnectedPeerIndex, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsNonExistingDisconnectedPeer);
			GameNetworkMessage.WriteBoolToPacket(this.IsReceiverPeer);
		}

		// Token: 0x06003336 RID: 13110 RVA: 0x000D49DC File Offset: 0x000D2BDC
		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.PlayerName = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.DisconnectedPeerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.IsNonExistingDisconnectedPeer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsReceiverPeer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06003337 RID: 13111 RVA: 0x000D4A37 File Offset: 0x000D2C37
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x06003338 RID: 13112 RVA: 0x000D4A3C File Offset: 0x000D2C3C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create a new player with name: ",
				this.PlayerName,
				" and index: ",
				this.PlayerIndex,
				" and dcedIndex: ",
				this.DisconnectedPeerIndex,
				" which is ",
				(!this.IsNonExistingDisconnectedPeer) ? "not" : "",
				" a NonExistingDisconnectedPeer"
			});
		}
	}
}
