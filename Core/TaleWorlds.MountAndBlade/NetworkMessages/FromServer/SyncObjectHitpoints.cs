using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000BA RID: 186
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncObjectHitpoints : GameNetworkMessage
	{
		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0000DFBD File Offset: 0x0000C1BD
		// (set) Token: 0x060007B7 RID: 1975 RVA: 0x0000DFC5 File Offset: 0x0000C1C5
		public MissionObject MissionObject { get; private set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x060007B8 RID: 1976 RVA: 0x0000DFCE File Offset: 0x0000C1CE
		// (set) Token: 0x060007B9 RID: 1977 RVA: 0x0000DFD6 File Offset: 0x0000C1D6
		public float Hitpoints { get; private set; }

		// Token: 0x060007BA RID: 1978 RVA: 0x0000DFDF File Offset: 0x0000C1DF
		public SyncObjectHitpoints(MissionObject missionObject, float hitpoints)
		{
			this.MissionObject = missionObject;
			this.Hitpoints = hitpoints;
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0000DFF5 File Offset: 0x0000C1F5
		public SyncObjectHitpoints()
		{
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0000E000 File Offset: 0x0000C200
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Hitpoints = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.UsableGameObjectHealthCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0000E02F File Offset: 0x0000C22F
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteFloatToPacket(MathF.Max(this.Hitpoints, 0f), CompressionMission.UsableGameObjectHealthCompressionInfo);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0000E056 File Offset: 0x0000C256
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0000E060 File Offset: 0x0000C260
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Synchronize HitPoints: ",
				this.Hitpoints,
				" of MissionObject with Id: ",
				this.MissionObject.Id,
				" and name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
