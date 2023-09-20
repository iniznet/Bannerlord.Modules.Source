using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000031 RID: 49
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SyncRelevantGameOptionsToServer : GameNetworkMessage
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00003C4A File Offset: 0x00001E4A
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00003C52 File Offset: 0x00001E52
		public bool SendMeBloodEvents { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00003C5B File Offset: 0x00001E5B
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00003C63 File Offset: 0x00001E63
		public bool SendMeSoundEvents { get; private set; }

		// Token: 0x06000185 RID: 389 RVA: 0x00003C6C File Offset: 0x00001E6C
		public SyncRelevantGameOptionsToServer()
		{
			this.SendMeBloodEvents = true;
			this.SendMeSoundEvents = true;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00003C82 File Offset: 0x00001E82
		public void InitializeOptions()
		{
			this.SendMeBloodEvents = BannerlordConfig.ShowBlood;
			this.SendMeSoundEvents = NativeOptions.GetConfig(NativeOptions.NativeOptionsType.SoundVolume) > 0.01f && NativeOptions.GetConfig(NativeOptions.NativeOptionsType.MasterVolume) > 0.01f;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00003CB4 File Offset: 0x00001EB4
		protected override bool OnRead()
		{
			bool flag = true;
			this.SendMeBloodEvents = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.SendMeSoundEvents = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00003CDE File Offset: 0x00001EDE
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.SendMeBloodEvents);
			GameNetworkMessage.WriteBoolToPacket(this.SendMeSoundEvents);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00003CF6 File Offset: 0x00001EF6
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00003CFA File Offset: 0x00001EFA
		protected override string OnGetLogFormat()
		{
			return "SyncRelevantGameOptionsToServer";
		}
	}
}
