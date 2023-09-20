using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B8 RID: 184
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizeMissionObject : GameNetworkMessage
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x0600079C RID: 1948 RVA: 0x0000DCAD File Offset: 0x0000BEAD
		// (set) Token: 0x0600079D RID: 1949 RVA: 0x0000DCB5 File Offset: 0x0000BEB5
		public SynchedMissionObject MissionObject { get; private set; }

		// Token: 0x0600079E RID: 1950 RVA: 0x0000DCBE File Offset: 0x0000BEBE
		public SynchronizeMissionObject(SynchedMissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0000DCCD File Offset: 0x0000BECD
		public SynchronizeMissionObject()
		{
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0000DCD5 File Offset: 0x0000BED5
		protected override void OnWrite()
		{
			base.WriteSynchedMissionObjectToPacket(this.MissionObject);
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0000DCE4 File Offset: 0x0000BEE4
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadSynchedMissionObjectFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0000DD01 File Offset: 0x0000BF01
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0000DD0C File Offset: 0x0000BF0C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Synchronize MissionObject with Id: ",
				this.MissionObject.Id.Id,
				" and name: ",
				(this.MissionObject.GameEntity != null) ? this.MissionObject.GameEntity.Name : "null entity"
			});
		}
	}
}
