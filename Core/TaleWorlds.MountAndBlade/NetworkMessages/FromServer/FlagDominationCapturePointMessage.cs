using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200007C RID: 124
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagDominationCapturePointMessage : GameNetworkMessage
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x000095FA File Offset: 0x000077FA
		// (set) Token: 0x060004E3 RID: 1251 RVA: 0x00009602 File Offset: 0x00007802
		public int FlagIndex { get; private set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0000960B File Offset: 0x0000780B
		// (set) Token: 0x060004E5 RID: 1253 RVA: 0x00009613 File Offset: 0x00007813
		public Team OwnerTeam { get; private set; }

		// Token: 0x060004E6 RID: 1254 RVA: 0x0000961C File Offset: 0x0000781C
		public FlagDominationCapturePointMessage()
		{
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00009624 File Offset: 0x00007824
		public FlagDominationCapturePointMessage(int flagIndex, Team owner)
		{
			this.FlagIndex = flagIndex;
			this.OwnerTeam = owner;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0000963A File Offset: 0x0000783A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FlagIndex, CompressionMission.FlagCapturePointIndexCompressionInfo);
			GameNetworkMessage.WriteTeamReferenceToPacket(this.OwnerTeam);
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00009658 File Offset: 0x00007858
		protected override bool OnRead()
		{
			bool flag = true;
			this.FlagIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagCapturePointIndexCompressionInfo, ref flag);
			this.OwnerTeam = GameNetworkMessage.ReadTeamReferenceFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x00009687 File Offset: 0x00007887
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0000968F File Offset: 0x0000788F
		protected override string OnGetLogFormat()
		{
			return "Flag owner changed.";
		}
	}
}
