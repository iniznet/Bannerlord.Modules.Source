using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class TeamChange : GameNetworkMessage
	{
		public bool AutoAssign { get; private set; }

		public int TeamIndex { get; private set; }

		public TeamChange(bool autoAssign, int teamIndex)
		{
			this.AutoAssign = autoAssign;
			this.TeamIndex = teamIndex;
		}

		public TeamChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AutoAssign = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (!this.AutoAssign)
			{
				this.TeamIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.AutoAssign);
			if (!this.AutoAssign)
			{
				GameNetworkMessage.WriteIntToPacket(this.TeamIndex, CompressionMission.TeamCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Changed team to: " + this.TeamIndex;
		}
	}
}
