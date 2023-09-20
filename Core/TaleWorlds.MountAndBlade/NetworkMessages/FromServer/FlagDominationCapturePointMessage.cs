using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagDominationCapturePointMessage : GameNetworkMessage
	{
		public int FlagIndex { get; private set; }

		public int OwnerTeamIndex { get; private set; }

		public FlagDominationCapturePointMessage()
		{
		}

		public FlagDominationCapturePointMessage(int flagIndex, int ownerTeamIndex)
		{
			this.FlagIndex = flagIndex;
			this.OwnerTeamIndex = ownerTeamIndex;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FlagIndex, CompressionMission.FlagCapturePointIndexCompressionInfo);
			GameNetworkMessage.WriteTeamIndexToPacket(this.OwnerTeamIndex);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.FlagIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagCapturePointIndexCompressionInfo, ref flag);
			this.OwnerTeamIndex = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Flag owner changed.";
		}
	}
}
