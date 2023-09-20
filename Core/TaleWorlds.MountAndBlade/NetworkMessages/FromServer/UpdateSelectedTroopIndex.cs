using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000044 RID: 68
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UpdateSelectedTroopIndex : GameNetworkMessage
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00004CD5 File Offset: 0x00002ED5
		// (set) Token: 0x06000242 RID: 578 RVA: 0x00004CDD File Offset: 0x00002EDD
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000243 RID: 579 RVA: 0x00004CE6 File Offset: 0x00002EE6
		// (set) Token: 0x06000244 RID: 580 RVA: 0x00004CEE File Offset: 0x00002EEE
		public int SelectedTroopIndex { get; private set; }

		// Token: 0x06000245 RID: 581 RVA: 0x00004CF7 File Offset: 0x00002EF7
		public UpdateSelectedTroopIndex(NetworkCommunicator peer, int selectedTroopIndex)
		{
			this.Peer = peer;
			this.SelectedTroopIndex = selectedTroopIndex;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00004D0D File Offset: 0x00002F0D
		public UpdateSelectedTroopIndex()
		{
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00004D18 File Offset: 0x00002F18
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.SelectedTroopIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SelectedTroopIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00004D48 File Offset: 0x00002F48
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.SelectedTroopIndex, CompressionMission.SelectedTroopIndexCompressionInfo);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00004D65 File Offset: 0x00002F65
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00004D6C File Offset: 0x00002F6C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Update SelectedTroopIndex to: ",
				this.SelectedTroopIndex,
				", on peer: ",
				this.Peer.UserName,
				" with peer-index:",
				this.Peer.Index
			});
		}
	}
}
