using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000098 RID: 152
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectDisabled : GameNetworkMessage
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x0000B6A6 File Offset: 0x000098A6
		// (set) Token: 0x0600062D RID: 1581 RVA: 0x0000B6AE File Offset: 0x000098AE
		public MissionObject MissionObject { get; private set; }

		// Token: 0x0600062E RID: 1582 RVA: 0x0000B6B7 File Offset: 0x000098B7
		public SetMissionObjectDisabled(MissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0000B6C6 File Offset: 0x000098C6
		public SetMissionObjectDisabled()
		{
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0000B6D0 File Offset: 0x000098D0
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0000B6ED File Offset: 0x000098ED
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0000B6FA File Offset: 0x000098FA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0000B704 File Offset: 0x00009904
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Mission Object with ID: ",
				this.MissionObject.Id.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" has been disabled."
			});
		}
	}
}
