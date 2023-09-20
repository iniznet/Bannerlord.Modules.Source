using System;

namespace TaleWorlds.MountAndBlade
{
	public enum BoneBodyPartType : sbyte
	{
		None = -1,
		Head,
		Neck,
		Chest,
		Abdomen,
		ShoulderLeft,
		ShoulderRight,
		ArmLeft,
		ArmRight,
		Legs,
		NumOfBodyPartTypes,
		CriticalBodyPartsBegin = 0,
		CriticalBodyPartsEnd = 6
	}
}
