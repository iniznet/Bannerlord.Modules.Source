﻿using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class TauntSelected : GameNetworkMessage
	{
		public int IndexOfTaunt { get; private set; }

		public TauntSelected(int indexOfTaunt)
		{
			this.IndexOfTaunt = indexOfTaunt;
		}

		public TauntSelected()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.IndexOfTaunt = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TauntIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.IndexOfTaunt, CompressionMission.TauntIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		protected override string OnGetLogFormat()
		{
			return "FromClient.CheerSelected: " + this.IndexOfTaunt;
		}
	}
}
