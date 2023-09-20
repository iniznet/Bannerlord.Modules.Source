using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000AC RID: 172
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetUsableMissionObjectIsDisabledForPlayers : GameNetworkMessage
	{
		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060006F8 RID: 1784 RVA: 0x0000CBF9 File Offset: 0x0000ADF9
		// (set) Token: 0x060006F9 RID: 1785 RVA: 0x0000CC01 File Offset: 0x0000AE01
		public UsableMissionObject UsableGameObject { get; private set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060006FA RID: 1786 RVA: 0x0000CC0A File Offset: 0x0000AE0A
		// (set) Token: 0x060006FB RID: 1787 RVA: 0x0000CC12 File Offset: 0x0000AE12
		public bool IsDisabledForPlayers { get; private set; }

		// Token: 0x060006FC RID: 1788 RVA: 0x0000CC1B File Offset: 0x0000AE1B
		public SetUsableMissionObjectIsDisabledForPlayers(UsableMissionObject usableGameObject, bool isDisabledForPlayers)
		{
			this.UsableGameObject = usableGameObject;
			this.IsDisabledForPlayers = isDisabledForPlayers;
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0000CC31 File Offset: 0x0000AE31
		public SetUsableMissionObjectIsDisabledForPlayers()
		{
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0000CC3C File Offset: 0x0000AE3C
		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			this.IsDisabledForPlayers = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0000CC6B File Offset: 0x0000AE6B
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
			GameNetworkMessage.WriteBoolToPacket(this.IsDisabledForPlayers);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0000CC83 File Offset: 0x0000AE83
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0000CC8C File Offset: 0x0000AE8C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set IsDisabled for player: ",
				this.IsDisabledForPlayers ? "True" : "False",
				" on UsableMissionObject with ID: ",
				this.UsableGameObject.Id,
				" and with name: ",
				this.UsableGameObject.GameEntity.Name
			});
		}
	}
}
