using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddTeam : GameNetworkMessage
	{
		public int TeamIndex { get; private set; }

		public BattleSideEnum Side { get; private set; }

		public uint Color { get; private set; }

		public uint Color2 { get; private set; }

		public string BannerCode { get; private set; }

		public bool IsPlayerGeneral { get; private set; }

		public bool IsPlayerSergeant { get; private set; }

		public AddTeam(int teamIndex, BattleSideEnum side, uint color, uint color2, string bannerCode, bool isPlayerGeneral, bool isPlayerSergeant)
		{
			this.TeamIndex = teamIndex;
			this.Side = side;
			this.Color = color;
			this.Color2 = color2;
			this.BannerCode = bannerCode;
			this.IsPlayerGeneral = isPlayerGeneral;
			this.IsPlayerSergeant = isPlayerSergeant;
		}

		public AddTeam()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.TeamIndex = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			this.Side = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.ColorCompressionInfo, ref flag);
			this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.ColorCompressionInfo, ref flag);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.IsPlayerGeneral = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsPlayerSergeant = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteTeamIndexToPacket(this.TeamIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.Side, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color, CompressionBasic.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionBasic.ColorCompressionInfo);
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
