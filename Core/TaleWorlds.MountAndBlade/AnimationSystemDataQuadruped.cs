using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Animation_system_data_quadruped", false)]
	[Serializable]
	public struct AnimationSystemDataQuadruped
	{
		public Vec3 ReinHandleLeftLocalPosition;

		public Vec3 ReinHandleRightLocalPosition;

		public string ReinSkeleton;

		public string ReinCollisionBody;

		public sbyte IndexOfBoneToDetectGroundSlopeFront;

		public sbyte IndexOfBoneToDetectGroundSlopeBack;

		public AnimationSystemBoneDataQuadruped Bones;
	}
}
