using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Bone_body_part_type", false)]
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
