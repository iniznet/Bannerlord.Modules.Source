using System;

namespace TaleWorlds.MountAndBlade.Network.Messages
{
	// Token: 0x020003B5 RID: 949
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DeletePlayer : GameNetworkMessage
	{
		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06003339 RID: 13113 RVA: 0x000D4AB9 File Offset: 0x000D2CB9
		// (set) Token: 0x0600333A RID: 13114 RVA: 0x000D4AC1 File Offset: 0x000D2CC1
		public int PlayerIndex { get; private set; }

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x0600333B RID: 13115 RVA: 0x000D4ACA File Offset: 0x000D2CCA
		// (set) Token: 0x0600333C RID: 13116 RVA: 0x000D4AD2 File Offset: 0x000D2CD2
		public bool AddToDisconnectList { get; private set; }

		// Token: 0x0600333D RID: 13117 RVA: 0x000D4ADB File Offset: 0x000D2CDB
		public DeletePlayer(int playerIndex, bool addToDisconnectList)
		{
			this.PlayerIndex = playerIndex;
			this.AddToDisconnectList = addToDisconnectList;
		}

		// Token: 0x0600333E RID: 13118 RVA: 0x000D4AF1 File Offset: 0x000D2CF1
		public DeletePlayer()
		{
		}

		// Token: 0x0600333F RID: 13119 RVA: 0x000D4AF9 File Offset: 0x000D2CF9
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.PlayerIndex, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.AddToDisconnectList);
		}

		// Token: 0x06003340 RID: 13120 RVA: 0x000D4B18 File Offset: 0x000D2D18
		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.AddToDisconnectList = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06003341 RID: 13121 RVA: 0x000D4B47 File Offset: 0x000D2D47
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x06003342 RID: 13122 RVA: 0x000D4B4B File Offset: 0x000D2D4B
		protected override string OnGetLogFormat()
		{
			return "Delete player with index" + this.PlayerIndex;
		}
	}
}
