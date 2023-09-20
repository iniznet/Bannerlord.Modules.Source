using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B9 RID: 185
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncObjectDestructionLevel : GameNetworkMessage
	{
		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x0000DD79 File Offset: 0x0000BF79
		// (set) Token: 0x060007A5 RID: 1957 RVA: 0x0000DD81 File Offset: 0x0000BF81
		public MissionObject MissionObject { get; private set; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060007A6 RID: 1958 RVA: 0x0000DD8A File Offset: 0x0000BF8A
		// (set) Token: 0x060007A7 RID: 1959 RVA: 0x0000DD92 File Offset: 0x0000BF92
		public int DestructionLevel { get; private set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060007A8 RID: 1960 RVA: 0x0000DD9B File Offset: 0x0000BF9B
		// (set) Token: 0x060007A9 RID: 1961 RVA: 0x0000DDA3 File Offset: 0x0000BFA3
		public int ForcedIndex { get; private set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060007AA RID: 1962 RVA: 0x0000DDAC File Offset: 0x0000BFAC
		// (set) Token: 0x060007AB RID: 1963 RVA: 0x0000DDB4 File Offset: 0x0000BFB4
		public float BlowMagnitude { get; private set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x0000DDBD File Offset: 0x0000BFBD
		// (set) Token: 0x060007AD RID: 1965 RVA: 0x0000DDC5 File Offset: 0x0000BFC5
		public Vec3 BlowPosition { get; private set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060007AE RID: 1966 RVA: 0x0000DDCE File Offset: 0x0000BFCE
		// (set) Token: 0x060007AF RID: 1967 RVA: 0x0000DDD6 File Offset: 0x0000BFD6
		public Vec3 BlowDirection { get; private set; }

		// Token: 0x060007B0 RID: 1968 RVA: 0x0000DDDF File Offset: 0x0000BFDF
		public SyncObjectDestructionLevel(MissionObject missionObject, int destructionLevel, int forcedIndex, float blowMagnitude, Vec3 blowPosition, Vec3 blowDirection)
		{
			this.MissionObject = missionObject;
			this.DestructionLevel = destructionLevel;
			this.ForcedIndex = forcedIndex;
			this.BlowMagnitude = blowMagnitude;
			this.BlowPosition = blowPosition;
			this.BlowDirection = blowDirection;
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x0000DE14 File Offset: 0x0000C014
		public SyncObjectDestructionLevel()
		{
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x0000DE1C File Offset: 0x0000C01C
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.DestructionLevel = GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsableGameObjectDestructionStateCompressionInfo, ref flag);
			this.ForcedIndex = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag) : (-1));
			this.BlowMagnitude = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.UsableGameObjectBlowMagnitude, ref flag);
			this.BlowPosition = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.BlowDirection = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.UsableGameObjectBlowDirection, ref flag);
			return flag;
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x0000DEA0 File Offset: 0x0000C0A0
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket(this.DestructionLevel, CompressionMission.UsableGameObjectDestructionStateCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.ForcedIndex != -1);
			if (this.ForcedIndex != -1)
			{
				GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			}
			GameNetworkMessage.WriteFloatToPacket(this.BlowMagnitude, CompressionMission.UsableGameObjectBlowMagnitude);
			GameNetworkMessage.WriteVec3ToPacket(this.BlowPosition, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.BlowDirection, CompressionMission.UsableGameObjectBlowDirection);
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x0000DF22 File Offset: 0x0000C122
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x0000DF2C File Offset: 0x0000C12C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Synchronize DestructionLevel: ",
				this.DestructionLevel,
				" of MissionObject with Id: ",
				this.MissionObject.Id,
				" and name: ",
				this.MissionObject.GameEntity.Name,
				(this.ForcedIndex != -1) ? (" (New object will have ID: " + this.ForcedIndex + ")") : ""
			});
		}
	}
}
