using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class InitializeFormation : GameNetworkMessage
	{
		public int FormationIndex { get; private set; }

		public int TeamIndex { get; private set; }

		public string BannerCode { get; private set; }

		public InitializeFormation(Formation formation, int teamIndex, string bannerCode)
		{
			this.FormationIndex = (int)formation.FormationIndex;
			this.TeamIndex = teamIndex;
			this.BannerCode = ((!string.IsNullOrEmpty(bannerCode)) ? bannerCode : string.Empty);
		}

		public InitializeFormation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FormationClassCompressionInfo, ref flag);
			this.TeamIndex = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionMission.FormationClassCompressionInfo);
			GameNetworkMessage.WriteTeamIndexToPacket(this.TeamIndex);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Initialize formation with index: ", this.FormationIndex, ", for team: ", this.TeamIndex });
		}
	}
}
