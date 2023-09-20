using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Bone_body_type_data")]
	public struct BoneBodyTypeData
	{
		public readonly BoneBodyPartType BodyPartType;

		public readonly sbyte Priority;

		public readonly SkeletonModelBoundsRecFlags DataFlags;
	}
}
