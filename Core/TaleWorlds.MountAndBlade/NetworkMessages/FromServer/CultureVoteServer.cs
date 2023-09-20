using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200003B RID: 59
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CultureVoteServer : GameNetworkMessage
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001DF RID: 479 RVA: 0x00004351 File Offset: 0x00002551
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x00004359 File Offset: 0x00002559
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x00004362 File Offset: 0x00002562
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x0000436A File Offset: 0x0000256A
		public BasicCultureObject VotedCulture { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x00004373 File Offset: 0x00002573
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x0000437B File Offset: 0x0000257B
		public CultureVoteTypes VotedType { get; private set; }

		// Token: 0x060001E5 RID: 485 RVA: 0x00004384 File Offset: 0x00002584
		public CultureVoteServer()
		{
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000438C File Offset: 0x0000258C
		public CultureVoteServer(NetworkCommunicator peer, CultureVoteTypes type, BasicCultureObject culture)
		{
			this.Peer = peer;
			this.VotedType = type;
			this.VotedCulture = culture;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x000043AC File Offset: 0x000025AC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket((int)this.VotedType, CompressionMission.TeamSideCompressionInfo);
			MBReadOnlyList<BasicCultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>();
			GameNetworkMessage.WriteIntToPacket((this.VotedCulture == null) ? (-1) : objectTypeList.IndexOf(this.VotedCulture), CompressionBasic.CultureIndexCompressionInfo);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00004400 File Offset: 0x00002600
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.VotedType = (CultureVoteTypes)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref flag);
			if (flag)
			{
				MBReadOnlyList<BasicCultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>();
				this.VotedCulture = ((num < 0) ? null : objectTypeList[num]);
			}
			return flag;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000445F File Offset: 0x0000265F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00004468 File Offset: 0x00002668
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Culture ",
				this.VotedCulture.Name,
				" has been ",
				this.VotedType.ToString().ToLower(),
				(this.VotedType == CultureVoteTypes.Ban) ? "ned." : "ed."
			});
		}
	}
}
