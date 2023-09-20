using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000008 RID: 8
	[Flags]
	public enum AgentFlag : uint
	{
		// Token: 0x0400009C RID: 156
		None = 0U,
		// Token: 0x0400009D RID: 157
		Mountable = 1U,
		// Token: 0x0400009E RID: 158
		CanJump = 2U,
		// Token: 0x0400009F RID: 159
		CanRear = 4U,
		// Token: 0x040000A0 RID: 160
		CanAttack = 8U,
		// Token: 0x040000A1 RID: 161
		CanDefend = 16U,
		// Token: 0x040000A2 RID: 162
		RunsAwayWhenHit = 32U,
		// Token: 0x040000A3 RID: 163
		CanCharge = 64U,
		// Token: 0x040000A4 RID: 164
		CanBeCharged = 128U,
		// Token: 0x040000A5 RID: 165
		CanClimbLadders = 256U,
		// Token: 0x040000A6 RID: 166
		CanBeInGroup = 512U,
		// Token: 0x040000A7 RID: 167
		CanSprint = 1024U,
		// Token: 0x040000A8 RID: 168
		IsHumanoid = 2048U,
		// Token: 0x040000A9 RID: 169
		CanGetScared = 4096U,
		// Token: 0x040000AA RID: 170
		CanRide = 8192U,
		// Token: 0x040000AB RID: 171
		CanWieldWeapon = 16384U,
		// Token: 0x040000AC RID: 172
		CanCrouch = 32768U,
		// Token: 0x040000AD RID: 173
		CanGetAlarmed = 65536U,
		// Token: 0x040000AE RID: 174
		CanWander = 131072U,
		// Token: 0x040000AF RID: 175
		CanKick = 524288U,
		// Token: 0x040000B0 RID: 176
		CanRetreat = 1048576U,
		// Token: 0x040000B1 RID: 177
		MoveAsHerd = 2097152U,
		// Token: 0x040000B2 RID: 178
		MoveForwardOnly = 4194304U,
		// Token: 0x040000B3 RID: 179
		IsUnique = 8388608U,
		// Token: 0x040000B4 RID: 180
		CanUseAllBowsMounted = 16777216U,
		// Token: 0x040000B5 RID: 181
		CanReloadAllXBowsMounted = 33554432U,
		// Token: 0x040000B6 RID: 182
		CanDeflectArrowsWith2HSword = 67108864U
	}
}
