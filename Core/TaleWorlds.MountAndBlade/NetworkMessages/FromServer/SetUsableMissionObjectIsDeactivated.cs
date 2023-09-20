using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000AB RID: 171
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetUsableMissionObjectIsDeactivated : GameNetworkMessage
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0000CAFC File Offset: 0x0000ACFC
		// (set) Token: 0x060006EF RID: 1775 RVA: 0x0000CB04 File Offset: 0x0000AD04
		public UsableMissionObject UsableGameObject { get; private set; }

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060006F0 RID: 1776 RVA: 0x0000CB0D File Offset: 0x0000AD0D
		// (set) Token: 0x060006F1 RID: 1777 RVA: 0x0000CB15 File Offset: 0x0000AD15
		public bool IsDeactivated { get; private set; }

		// Token: 0x060006F2 RID: 1778 RVA: 0x0000CB1E File Offset: 0x0000AD1E
		public SetUsableMissionObjectIsDeactivated(UsableMissionObject usableGameObject, bool isDeactivated)
		{
			this.UsableGameObject = usableGameObject;
			this.IsDeactivated = isDeactivated;
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0000CB34 File Offset: 0x0000AD34
		public SetUsableMissionObjectIsDeactivated()
		{
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0000CB3C File Offset: 0x0000AD3C
		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			this.IsDeactivated = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0000CB6B File Offset: 0x0000AD6B
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
			GameNetworkMessage.WriteBoolToPacket(this.IsDeactivated);
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0000CB83 File Offset: 0x0000AD83
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0000CB8C File Offset: 0x0000AD8C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set IsDeactivated: ",
				this.IsDeactivated ? "True" : "False",
				" on UsableMissionObject with ID: ",
				this.UsableGameObject.Id,
				" and with name: ",
				this.UsableGameObject.GameEntity.Name
			});
		}
	}
}
