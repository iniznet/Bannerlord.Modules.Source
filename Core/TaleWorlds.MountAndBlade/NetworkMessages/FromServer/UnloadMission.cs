using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000BC RID: 188
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UnloadMission : GameNetworkMessage
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x060007CC RID: 1996 RVA: 0x0000E1FB File Offset: 0x0000C3FB
		// (set) Token: 0x060007CD RID: 1997 RVA: 0x0000E203 File Offset: 0x0000C403
		public bool UnloadingForBattleIndexMismatch { get; private set; }

		// Token: 0x060007CE RID: 1998 RVA: 0x0000E20C File Offset: 0x0000C40C
		public UnloadMission()
		{
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0000E214 File Offset: 0x0000C414
		public UnloadMission(bool unloadingForBattleIndexMismatch)
		{
			this.UnloadingForBattleIndexMismatch = unloadingForBattleIndexMismatch;
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x0000E224 File Offset: 0x0000C424
		protected override bool OnRead()
		{
			bool flag = true;
			this.UnloadingForBattleIndexMismatch = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0000E241 File Offset: 0x0000C441
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.UnloadingForBattleIndexMismatch);
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0000E24E File Offset: 0x0000C44E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0000E256 File Offset: 0x0000C456
		protected override string OnGetLogFormat()
		{
			return "Unload Mission";
		}
	}
}
