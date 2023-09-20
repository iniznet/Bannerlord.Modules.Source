using System;

namespace TaleWorlds.MountAndBlade.Network.Messages
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DeletePlayer : GameNetworkMessage
	{
		public int PlayerIndex { get; private set; }

		public bool AddToDisconnectList { get; private set; }

		public DeletePlayer(int playerIndex, bool addToDisconnectList)
		{
			this.PlayerIndex = playerIndex;
			this.AddToDisconnectList = addToDisconnectList;
		}

		public DeletePlayer()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.PlayerIndex, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.AddToDisconnectList);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.AddToDisconnectList = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		protected override string OnGetLogFormat()
		{
			return "Delete player with index" + this.PlayerIndex;
		}
	}
}
