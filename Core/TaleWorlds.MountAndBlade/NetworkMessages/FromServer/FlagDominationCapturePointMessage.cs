using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagDominationCapturePointMessage : GameNetworkMessage
	{
		public int FlagIndex { get; private set; }

		public Team OwnerTeam { get; private set; }

		public FlagDominationCapturePointMessage()
		{
		}

		public FlagDominationCapturePointMessage(int flagIndex, Team owner)
		{
			this.FlagIndex = flagIndex;
			this.OwnerTeam = owner;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FlagIndex, CompressionMission.FlagCapturePointIndexCompressionInfo);
			GameNetworkMessage.WriteTeamReferenceToPacket(this.OwnerTeam);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.FlagIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagCapturePointIndexCompressionInfo, ref flag);
			this.OwnerTeam = GameNetworkMessage.ReadTeamReferenceFromPacket(ref flag);
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
