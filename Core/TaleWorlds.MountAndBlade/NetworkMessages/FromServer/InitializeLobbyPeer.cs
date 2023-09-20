using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlayerServices;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200004A RID: 74
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class InitializeLobbyPeer : GameNetworkMessage
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000283 RID: 643 RVA: 0x0000536C File Offset: 0x0000356C
		// (set) Token: 0x06000284 RID: 644 RVA: 0x00005374 File Offset: 0x00003574
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000285 RID: 645 RVA: 0x0000537D File Offset: 0x0000357D
		// (set) Token: 0x06000286 RID: 646 RVA: 0x00005385 File Offset: 0x00003585
		public PlayerId ProvidedId { get; private set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000287 RID: 647 RVA: 0x0000538E File Offset: 0x0000358E
		// (set) Token: 0x06000288 RID: 648 RVA: 0x00005396 File Offset: 0x00003596
		public string BannerCode { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000539F File Offset: 0x0000359F
		// (set) Token: 0x0600028A RID: 650 RVA: 0x000053A7 File Offset: 0x000035A7
		public BodyProperties BodyProperties { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600028B RID: 651 RVA: 0x000053B0 File Offset: 0x000035B0
		// (set) Token: 0x0600028C RID: 652 RVA: 0x000053B8 File Offset: 0x000035B8
		public int ChosenBadgeIndex { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600028D RID: 653 RVA: 0x000053C1 File Offset: 0x000035C1
		// (set) Token: 0x0600028E RID: 654 RVA: 0x000053C9 File Offset: 0x000035C9
		public int ForcedAvatarIndex { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600028F RID: 655 RVA: 0x000053D2 File Offset: 0x000035D2
		// (set) Token: 0x06000290 RID: 656 RVA: 0x000053DA File Offset: 0x000035DA
		public bool IsFemale { get; private set; }

		// Token: 0x06000291 RID: 657 RVA: 0x000053E4 File Offset: 0x000035E4
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

		// Token: 0x06000292 RID: 658 RVA: 0x00005450 File Offset: 0x00003650
		public InitializeLobbyPeer()
		{
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00005458 File Offset: 0x00003658
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

		// Token: 0x06000294 RID: 660 RVA: 0x0000551C File Offset: 0x0000371C
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

		// Token: 0x06000295 RID: 661 RVA: 0x000055E3 File Offset: 0x000037E3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000055E7 File Offset: 0x000037E7
		protected override string OnGetLogFormat()
		{
			return "Initialize LobbyPeer from Peer: " + this.Peer.UserName;
		}
	}
}
