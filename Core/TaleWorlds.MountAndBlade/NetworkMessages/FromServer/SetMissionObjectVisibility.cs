using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A0 RID: 160
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVisibility : GameNetworkMessage
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x0000C114 File Offset: 0x0000A314
		// (set) Token: 0x06000685 RID: 1669 RVA: 0x0000C11C File Offset: 0x0000A31C
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0000C125 File Offset: 0x0000A325
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x0000C12D File Offset: 0x0000A32D
		public bool Visible { get; private set; }

		// Token: 0x06000688 RID: 1672 RVA: 0x0000C136 File Offset: 0x0000A336
		public SetMissionObjectVisibility(MissionObject missionObject, bool visible)
		{
			this.MissionObject = missionObject;
			this.Visible = visible;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x0000C14C File Offset: 0x0000A34C
		public SetMissionObjectVisibility()
		{
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0000C154 File Offset: 0x0000A354
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Visible = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0000C17E File Offset: 0x0000A37E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.Visible);
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0000C196 File Offset: 0x0000A396
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0000C1A0 File Offset: 0x0000A3A0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Visibility of MissionObject with ID: ",
				this.MissionObject.Id.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" to: ",
				this.Visible ? "True" : "False"
			});
		}
	}
}
