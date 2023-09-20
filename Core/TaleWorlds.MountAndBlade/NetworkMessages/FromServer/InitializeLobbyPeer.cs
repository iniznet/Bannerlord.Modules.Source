using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlayerServices;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class InitializeLobbyPeer : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public PlayerId ProvidedId { get; private set; }

		public string BannerCode { get; private set; }

		public BodyProperties BodyProperties { get; private set; }

		public int ChosenBadgeIndex { get; private set; }

		public int ForcedAvatarIndex { get; private set; }

		public bool IsFemale { get; private set; }

		public InitializeLobbyPeer(NetworkCommunicator peer, VirtualPlayer virtualPlayer, int forcedAvatarIndex)
		{
			this.Peer = peer;
			this.ProvidedId = virtualPlayer.Id;
			this.BannerCode = ((virtualPlayer.BannerCode != null) ? virtualPlayer.BannerCode : string.Empty);
			this.BodyProperties = virtualPlayer.BodyProperties;
			this.ChosenBadgeIndex = virtualPlayer.ChosenBadgeIndex;
			this.IsFemale = virtualPlayer.IsFemale;
			this.ForcedAvatarIndex = forcedAvatarIndex;
		}

		public InitializeLobbyPeer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			ulong num = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref flag);
			ulong num2 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref flag);
			ulong num3 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref flag);
			ulong num4 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref flag);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			string text = GameNetworkMessage.ReadStringFromPacket(ref flag);
			if (flag)
			{
				this.ProvidedId = new PlayerId(num, num2, num3, num4);
				BodyProperties bodyProperties;
				if (BodyProperties.FromString(text, out bodyProperties))
				{
					this.BodyProperties = bodyProperties;
				}
				else
				{
					flag = false;
				}
			}
			this.ChosenBadgeIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerChosenBadgeCompressionInfo, ref flag);
			this.ForcedAvatarIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ForcedAvatarIndexCompressionInfo, ref flag);
			this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part1, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part2, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part3, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part4, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
			GameNetworkMessage.WriteStringToPacket(this.BodyProperties.ToString());
			GameNetworkMessage.WriteIntToPacket(this.ChosenBadgeIndex, CompressionBasic.PlayerChosenBadgeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedAvatarIndex, CompressionBasic.ForcedAvatarIndexCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		protected override string OnGetLogFormat()
		{
			return "Initialize LobbyPeer from Peer: " + this.Peer.UserName;
		}
	}
}
