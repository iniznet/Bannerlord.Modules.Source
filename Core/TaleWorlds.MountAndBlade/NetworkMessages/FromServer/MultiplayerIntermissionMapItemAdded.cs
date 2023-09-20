using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000051 RID: 81
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionMapItemAdded : GameNetworkMessage
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002DB RID: 731 RVA: 0x00005C44 File Offset: 0x00003E44
		// (set) Token: 0x060002DC RID: 732 RVA: 0x00005C4C File Offset: 0x00003E4C
		public string MapId { get; private set; }

		// Token: 0x060002DD RID: 733 RVA: 0x00005C55 File Offset: 0x00003E55
		public MultiplayerIntermissionMapItemAdded()
		{
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00005C5D File Offset: 0x00003E5D
		public MultiplayerIntermissionMapItemAdded(string mapId)
		{
			this.MapId = mapId;
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00005C6C File Offset: 0x00003E6C
		protected override bool OnRead()
		{
			bool flag = true;
			this.MapId = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00005C89 File Offset: 0x00003E89
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.MapId);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00005C96 File Offset: 0x00003E96
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00005C9E File Offset: 0x00003E9E
		protected override string OnGetLogFormat()
		{
			return "Adding map for voting with id: " + this.MapId + ".";
		}
	}
}
