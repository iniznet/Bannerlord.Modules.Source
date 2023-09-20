using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200003C RID: 60
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagRaisingStatus : GameNetworkMessage
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001EB RID: 491 RVA: 0x000044D1 File Offset: 0x000026D1
		// (set) Token: 0x060001EC RID: 492 RVA: 0x000044D9 File Offset: 0x000026D9
		public float Progress { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001ED RID: 493 RVA: 0x000044E2 File Offset: 0x000026E2
		// (set) Token: 0x060001EE RID: 494 RVA: 0x000044EA File Offset: 0x000026EA
		public CaptureTheFlagFlagDirection Direction { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001EF RID: 495 RVA: 0x000044F3 File Offset: 0x000026F3
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x000044FB File Offset: 0x000026FB
		public float Speed { get; private set; }

		// Token: 0x060001F1 RID: 497 RVA: 0x00004504 File Offset: 0x00002704
		public FlagRaisingStatus()
		{
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000450C File Offset: 0x0000270C
		public FlagRaisingStatus(float currProgress, CaptureTheFlagFlagDirection direction, float speed)
		{
			this.Progress = currProgress;
			this.Direction = direction;
			this.Speed = speed;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000452C File Offset: 0x0000272C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagClassicProgressCompressionInfo, ref flag);
			this.Direction = (CaptureTheFlagFlagDirection)GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagDirectionEnumCompressionInfo, ref flag);
			if (flag && this.Direction != CaptureTheFlagFlagDirection.None && this.Direction != CaptureTheFlagFlagDirection.Static)
			{
				this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagSpeedCompressionInfo, ref flag);
			}
			return flag;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00004588 File Offset: 0x00002788
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionMission.FlagClassicProgressCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.Direction, CompressionMission.FlagDirectionEnumCompressionInfo);
			if (this.Direction != CaptureTheFlagFlagDirection.None && this.Direction != CaptureTheFlagFlagDirection.Static)
			{
				GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionMission.FlagSpeedCompressionInfo);
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x000045D7 File Offset: 0x000027D7
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x000045E0 File Offset: 0x000027E0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Updating flag movement: Progress: ", this.Progress, ", Direction: ", this.Direction, ", Speed: ", this.Speed });
		}
	}
}
