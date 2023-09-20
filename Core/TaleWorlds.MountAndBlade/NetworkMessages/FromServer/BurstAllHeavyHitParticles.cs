using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200006C RID: 108
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BurstAllHeavyHitParticles : GameNetworkMessage
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0000788B File Offset: 0x00005A8B
		// (set) Token: 0x060003EF RID: 1007 RVA: 0x00007893 File Offset: 0x00005A93
		public MissionObject MissionObject { get; private set; }

		// Token: 0x060003F0 RID: 1008 RVA: 0x0000789C File Offset: 0x00005A9C
		public BurstAllHeavyHitParticles(MissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x000078AB File Offset: 0x00005AAB
		public BurstAllHeavyHitParticles()
		{
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x000078B4 File Offset: 0x00005AB4
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x000078D1 File Offset: 0x00005AD1
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x000078DE File Offset: 0x00005ADE
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x000078E8 File Offset: 0x00005AE8
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Bursting all heavy-hit particles for the DestructableComponent of MissionObject with Id: ",
				this.MissionObject.Id,
				" and name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
