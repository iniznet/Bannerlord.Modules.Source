using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200006D RID: 109
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BurstMissionObjectParticles : GameNetworkMessage
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00007936 File Offset: 0x00005B36
		// (set) Token: 0x060003F7 RID: 1015 RVA: 0x0000793E File Offset: 0x00005B3E
		public MissionObject MissionObject { get; private set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00007947 File Offset: 0x00005B47
		// (set) Token: 0x060003F9 RID: 1017 RVA: 0x0000794F File Offset: 0x00005B4F
		public bool DoChildren { get; private set; }

		// Token: 0x060003FA RID: 1018 RVA: 0x00007958 File Offset: 0x00005B58
		public BurstMissionObjectParticles(MissionObject missionObject, bool doChildren)
		{
			this.MissionObject = missionObject;
			this.DoChildren = doChildren;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0000796E File Offset: 0x00005B6E
		public BurstMissionObjectParticles()
		{
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00007978 File Offset: 0x00005B78
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.DoChildren = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x000079A2 File Offset: 0x00005BA2
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.DoChildren);
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x000079BA File Offset: 0x00005BBA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.Particles;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000079C4 File Offset: 0x00005BC4
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Burst MissionObject particles on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
