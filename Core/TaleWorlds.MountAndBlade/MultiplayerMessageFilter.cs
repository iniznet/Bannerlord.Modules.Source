using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E8 RID: 744
	[Flags]
	public enum MultiplayerMessageFilter : ulong
	{
		// Token: 0x04000F27 RID: 3879
		None = 0UL,
		// Token: 0x04000F28 RID: 3880
		Peers = 1UL,
		// Token: 0x04000F29 RID: 3881
		Messaging = 2UL,
		// Token: 0x04000F2A RID: 3882
		Items = 4UL,
		// Token: 0x04000F2B RID: 3883
		General = 8UL,
		// Token: 0x04000F2C RID: 3884
		Equipment = 16UL,
		// Token: 0x04000F2D RID: 3885
		EquipmentDetailed = 32UL,
		// Token: 0x04000F2E RID: 3886
		Formations = 64UL,
		// Token: 0x04000F2F RID: 3887
		Agents = 128UL,
		// Token: 0x04000F30 RID: 3888
		AgentsDetailed = 256UL,
		// Token: 0x04000F31 RID: 3889
		Mission = 512UL,
		// Token: 0x04000F32 RID: 3890
		MissionDetailed = 1024UL,
		// Token: 0x04000F33 RID: 3891
		AgentAnimations = 2048UL,
		// Token: 0x04000F34 RID: 3892
		SiegeWeapons = 4096UL,
		// Token: 0x04000F35 RID: 3893
		MissionObjects = 8192UL,
		// Token: 0x04000F36 RID: 3894
		MissionObjectsDetailed = 16384UL,
		// Token: 0x04000F37 RID: 3895
		SiegeWeaponsDetailed = 32768UL,
		// Token: 0x04000F38 RID: 3896
		Orders = 65536UL,
		// Token: 0x04000F39 RID: 3897
		GameMode = 131072UL,
		// Token: 0x04000F3A RID: 3898
		Administration = 262144UL,
		// Token: 0x04000F3B RID: 3899
		Particles = 524288UL,
		// Token: 0x04000F3C RID: 3900
		RPC = 1048576UL,
		// Token: 0x04000F3D RID: 3901
		All = 4294967295UL,
		// Token: 0x04000F3E RID: 3902
		LightLogging = 139913UL,
		// Token: 0x04000F3F RID: 3903
		NormalLogging = 1979037UL,
		// Token: 0x04000F40 RID: 3904
		AllWithoutDetails = 2044639UL
	}
}
