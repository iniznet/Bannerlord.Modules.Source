using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200008F RID: 143
	[Flags]
	public enum ItemFlags : uint
	{
		// Token: 0x040003FF RID: 1023
		ForceAttachOffHandPrimaryItemBone = 256U,
		// Token: 0x04000400 RID: 1024
		ForceAttachOffHandSecondaryItemBone = 512U,
		// Token: 0x04000401 RID: 1025
		AttachmentMask = 768U,
		// Token: 0x04000402 RID: 1026
		NotUsableByFemale = 1024U,
		// Token: 0x04000403 RID: 1027
		NotUsableByMale = 2048U,
		// Token: 0x04000404 RID: 1028
		DropOnWeaponChange = 4096U,
		// Token: 0x04000405 RID: 1029
		DropOnAnyAction = 8192U,
		// Token: 0x04000406 RID: 1030
		CannotBePickedUp = 16384U,
		// Token: 0x04000407 RID: 1031
		CanBePickedUpFromCorpse = 32768U,
		// Token: 0x04000408 RID: 1032
		QuickFadeOut = 65536U,
		// Token: 0x04000409 RID: 1033
		WoodenAttack = 131072U,
		// Token: 0x0400040A RID: 1034
		WoodenParry = 262144U,
		// Token: 0x0400040B RID: 1035
		HeldInOffHand = 524288U,
		// Token: 0x0400040C RID: 1036
		HasToBeHeldUp = 1048576U,
		// Token: 0x0400040D RID: 1037
		UseTeamColor = 2097152U,
		// Token: 0x0400040E RID: 1038
		Civilian = 4194304U,
		// Token: 0x0400040F RID: 1039
		DoNotScaleBodyAccordingToWeaponLength = 8388608U,
		// Token: 0x04000410 RID: 1040
		DoesNotHideChest = 16777216U,
		// Token: 0x04000411 RID: 1041
		NotStackable = 33554432U
	}
}
