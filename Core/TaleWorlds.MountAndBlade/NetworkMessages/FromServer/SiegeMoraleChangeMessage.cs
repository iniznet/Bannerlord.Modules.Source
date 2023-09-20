using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000060 RID: 96
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SiegeMoraleChangeMessage : GameNetworkMessage
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600035A RID: 858 RVA: 0x0000689F File Offset: 0x00004A9F
		// (set) Token: 0x0600035B RID: 859 RVA: 0x000068A7 File Offset: 0x00004AA7
		public int AttackerMorale { get; private set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600035C RID: 860 RVA: 0x000068B0 File Offset: 0x00004AB0
		// (set) Token: 0x0600035D RID: 861 RVA: 0x000068B8 File Offset: 0x00004AB8
		public int DefenderMorale { get; private set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600035E RID: 862 RVA: 0x000068C1 File Offset: 0x00004AC1
		// (set) Token: 0x0600035F RID: 863 RVA: 0x000068C9 File Offset: 0x00004AC9
		public int[] CapturePointRemainingMoraleGains { get; private set; }

		// Token: 0x06000360 RID: 864 RVA: 0x000068D2 File Offset: 0x00004AD2
		public SiegeMoraleChangeMessage()
		{
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000068DA File Offset: 0x00004ADA
		public SiegeMoraleChangeMessage(int attackerMorale, int defenderMorale, int[] capturePointRemainingMoraleGains)
		{
			this.AttackerMorale = attackerMorale;
			this.DefenderMorale = defenderMorale;
			this.CapturePointRemainingMoraleGains = capturePointRemainingMoraleGains;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x000068F8 File Offset: 0x00004AF8
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.AttackerMorale, CompressionMission.SiegeMoraleCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DefenderMorale, CompressionMission.SiegeMoraleCompressionInfo);
			int[] capturePointRemainingMoraleGains = this.CapturePointRemainingMoraleGains;
			for (int i = 0; i < capturePointRemainingMoraleGains.Length; i++)
			{
				GameNetworkMessage.WriteIntToPacket(capturePointRemainingMoraleGains[i], CompressionMission.SiegeMoralePerFlagCompressionInfo);
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00006948 File Offset: 0x00004B48
		protected override bool OnRead()
		{
			bool flag = true;
			this.AttackerMorale = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoraleCompressionInfo, ref flag);
			this.DefenderMorale = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoraleCompressionInfo, ref flag);
			this.CapturePointRemainingMoraleGains = new int[7];
			for (int i = 0; i < this.CapturePointRemainingMoraleGains.Length; i++)
			{
				this.CapturePointRemainingMoraleGains[i] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoralePerFlagCompressionInfo, ref flag);
			}
			return flag;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x000069AF File Offset: 0x00004BAF
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000069B7 File Offset: 0x00004BB7
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Morale synched. A: ", this.AttackerMorale, " D: ", this.DefenderMorale });
		}
	}
}
