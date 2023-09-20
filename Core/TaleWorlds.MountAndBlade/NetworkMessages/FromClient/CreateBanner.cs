using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000025 RID: 37
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CreateBanner : GameNetworkMessage
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00003690 File Offset: 0x00001890
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00003698 File Offset: 0x00001898
		public string BannerCode { get; private set; }

		// Token: 0x06000127 RID: 295 RVA: 0x000036A1 File Offset: 0x000018A1
		public CreateBanner(string bannerCode)
		{
			this.BannerCode = bannerCode;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000036B0 File Offset: 0x000018B0
		public CreateBanner()
		{
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000036B8 File Offset: 0x000018B8
		protected override bool OnRead()
		{
			bool flag = true;
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000036D5 File Offset: 0x000018D5
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000036E2 File Offset: 0x000018E2
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000036EA File Offset: 0x000018EA
		protected override string OnGetLogFormat()
		{
			return "Clients has updated his banner";
		}
	}
}
