using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddTeam : GameNetworkMessage
	{
		public MBTeam Team { get; private set; }

		public BattleSideEnum Side { get; private set; }

		public uint Color { get; private set; }

		public uint Color2 { get; private set; }

		public string BannerCode { get; private set; }

		public bool IsPlayerGeneral { get; private set; }

		public bool IsPlayerSergeant { get; private set; }

		public AddTeam(Team team)
		{
			this.Team = team.MBTeam;
			this.Side = team.Side;
			this.Color = team.Color;
			this.Color2 = team.Color2;
			this.BannerCode = ((team.Banner != null) ? TaleWorlds.Core.BannerCode.CreateFrom(team.Banner).Code : string.Empty);
			this.IsPlayerGeneral = team.IsPlayerGeneral;
			this.IsPlayerSergeant = team.IsPlayerSergeant;
		}

		public AddTeam()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Team = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			this.Side = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.IsPlayerGeneral = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsPlayerSergeant = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team, CompressionMission.TeamCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.Side, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayerGeneral);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayerSergeant);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Add team with side: " + this.Side;
		}
	}
}
