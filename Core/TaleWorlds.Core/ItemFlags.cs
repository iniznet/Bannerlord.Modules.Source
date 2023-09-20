using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum ItemFlags : uint
	{
		ForceAttachOffHandPrimaryItemBone = 256U,
		ForceAttachOffHandSecondaryItemBone = 512U,
		AttachmentMask = 768U,
		NotUsableByFemale = 1024U,
		NotUsableByMale = 2048U,
		DropOnWeaponChange = 4096U,
		DropOnAnyAction = 8192U,
		CannotBePickedUp = 16384U,
		CanBePickedUpFromCorpse = 32768U,
		QuickFadeOut = 65536U,
		WoodenAttack = 131072U,
		WoodenParry = 262144U,
		HeldInOffHand = 524288U,
		HasToBeHeldUp = 1048576U,
		UseTeamColor = 2097152U,
		Civilian = 4194304U,
		DoNotScaleBodyAccordingToWeaponLength = 8388608U,
		DoesNotHideChest = 16777216U,
		NotStackable = 33554432U
	}
}
