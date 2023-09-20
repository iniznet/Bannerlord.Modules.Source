using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestChangePreferredTroopType : GameNetworkMessage
	{
		public TroopType TroopType { get; private set; }

		public RequestChangePreferredTroopType(TroopType troopType)
		{
			this.TroopType = troopType;
		}

		public RequestChangePreferredTroopType()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.TroopType, CompressionBasic.TroopTypeCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.TroopType = (TroopType)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.TroopTypeCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Peer requesting preferred troop type change to " + this.TroopType;
		}
	}
}
