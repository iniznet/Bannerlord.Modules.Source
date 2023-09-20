using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000096 RID: 150
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationPaused : GameNetworkMessage
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0000B48A File Offset: 0x0000968A
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x0000B492 File Offset: 0x00009692
		public MissionObject MissionObject { get; private set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x0000B49B File Offset: 0x0000969B
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x0000B4A3 File Offset: 0x000096A3
		public bool IsPaused { get; private set; }

		// Token: 0x0600061A RID: 1562 RVA: 0x0000B4AC File Offset: 0x000096AC
		public SetMissionObjectAnimationPaused(MissionObject missionObject, bool isPaused)
		{
			this.MissionObject = missionObject;
			this.IsPaused = isPaused;
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0000B4C2 File Offset: 0x000096C2
		public SetMissionObjectAnimationPaused()
		{
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0000B4CC File Offset: 0x000096CC
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.IsPaused = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0000B4F6 File Offset: 0x000096F6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.IsPaused);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0000B50E File Offset: 0x0000970E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0000B518 File Offset: 0x00009718
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set animation to be: ",
				this.IsPaused ? "Paused" : "Not paused",
				" on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
