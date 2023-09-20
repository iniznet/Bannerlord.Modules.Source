using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000007 RID: 7
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CultureVoteClient : GameNetworkMessage
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000022F9 File Offset: 0x000004F9
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002301 File Offset: 0x00000501
		public BasicCultureObject VotedCulture { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000020 RID: 32 RVA: 0x0000230A File Offset: 0x0000050A
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002312 File Offset: 0x00000512
		public CultureVoteTypes VotedType { get; private set; }

		// Token: 0x06000022 RID: 34 RVA: 0x0000231B File Offset: 0x0000051B
		public CultureVoteClient()
		{
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002323 File Offset: 0x00000523
		public CultureVoteClient(CultureVoteTypes type, BasicCultureObject culture)
		{
			this.VotedType = type;
			this.VotedCulture = culture;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002339 File Offset: 0x00000539
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.VotedType, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().IndexOf(this.VotedCulture), CompressionBasic.CultureIndexCompressionInfo);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000236C File Offset: 0x0000056C
		protected override bool OnRead()
		{
			bool flag = true;
			this.VotedType = (CultureVoteTypes)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref flag);
			if (flag)
			{
				this.VotedCulture = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()[num];
			}
			return flag;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000023B4 File Offset: 0x000005B4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000023BC File Offset: 0x000005BC
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
