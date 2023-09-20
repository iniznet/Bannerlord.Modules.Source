using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200005B RID: 91
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncPerksForCurrentlySelectedTroop : GameNetworkMessage
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600032C RID: 812 RVA: 0x000064BB File Offset: 0x000046BB
		// (set) Token: 0x0600032D RID: 813 RVA: 0x000064C3 File Offset: 0x000046C3
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600032E RID: 814 RVA: 0x000064CC File Offset: 0x000046CC
		// (set) Token: 0x0600032F RID: 815 RVA: 0x000064D4 File Offset: 0x000046D4
		public int[] PerkIndices { get; private set; }

		// Token: 0x06000330 RID: 816 RVA: 0x000064DD File Offset: 0x000046DD
		public SyncPerksForCurrentlySelectedTroop()
		{
		}

		// Token: 0x06000331 RID: 817 RVA: 0x000064E5 File Offset: 0x000046E5
		public SyncPerksForCurrentlySelectedTroop(NetworkCommunicator peer, int[] perkIndices)
		{
			this.Peer = peer;
			this.PerkIndices = perkIndices;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x000064FC File Offset: 0x000046FC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			for (int i = 0; i < 3; i++)
			{
				GameNetworkMessage.WriteIntToPacket(this.PerkIndices[i], CompressionMission.PerkIndexCompressionInfo);
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00006534 File Offset: 0x00004734
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.PerkIndices = new int[3];
			for (int i = 0; i < 3; i++)
			{
				this.PerkIndices[i] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref flag);
			}
			return flag;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000657E File Offset: 0x0000477E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00006588 File Offset: 0x00004788
		protected override string OnGetLogFormat()
		{
			string text = "";
			for (int i = 0; i < 3; i++)
			{
				text += string.Format("[{0}]", this.PerkIndices[i]);
			}
			return string.Concat(new string[]
			{
				"Selected perks for ",
				this.Peer.UserName,
				" has been updated as ",
				text,
				"."
			});
		}
	}
}
