using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Animation_system_data")]
	[Serializable]
	public struct AnimationSystemData
	{
		public static AnimationSystemData GetHardcodedAnimationSystemDataForHumanSkeleton()
		{
			MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(0);
			return new AnimationSystemData
			{
				ActionSet = actionSetWithIndex,
				MonsterUsageSetIndex = -1,
				WalkingSpeedLimit = 1f,
				CrouchWalkingSpeedLimit = 1f,
				StepSize = 1f,
				HasClippingPlane = false,
				IndicesOfRagdollBonesToCheckForCorpses = new sbyte[]
				{
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "head"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "neck"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foretwist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foretwist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_upperarm_twist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_upperarm_twist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_clavicle"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_clavicle"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine"),
					-1,
					-1
				},
				CountOfRagdollBonesToCheckForCorpses = 9,
				RagdollFallSoundBoneIndices = new sbyte[]
				{
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine2"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_upperarm_twist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_upperarm_twist"),
					-1
				},
				RagdollFallSoundBoneIndexCount = 3,
				HeadLookDirectionBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "head"),
				SpineLowerBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine"),
				SpineUpperBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine1"),
				ThoraxLookDirectionBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine2"),
				NeckRootBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "neck"),
				PelvisBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "pelvis"),
				RightUpperArmBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_upperarm_twist"),
				LeftUpperArmBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_upperarm_twist"),
				FallBlowDamageBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_calf"),
				TerrainDecalBone0Index = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foot"),
				TerrainDecalBone1Index = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foot"),
				RagdollStationaryCheckBoneIndices = new sbyte[]
				{
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_upperarm_twist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_upperarm_twist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_thigh"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_thigh"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_calf"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_calf"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "pelvis"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "head")
				},
				RagdollStationaryCheckBoneCount = 8,
				MoveAdderBoneIndices = new sbyte[]
				{
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine1"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine2"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_clavicle"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_clavicle"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "neck"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "head")
				},
				MoveAdderBoneCount = 7,
				SplashDecalBoneIndices = new sbyte[]
				{
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_calf"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foot"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_toe0"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_calf"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foot"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_toe0")
				},
				SplashDecalBoneCount = 6,
				BloodBurstBoneIndices = new sbyte[]
				{
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_clavicle"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine1"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_clavicle"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foretwist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine1"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foretwist"),
					Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "spine")
				},
				BloodBurstBoneCount = 8,
				MainHandBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_hand"),
				OffHandBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_hand"),
				MainHandItemBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_finger0"),
				OffHandItemBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_finger0"),
				MainHandItemSecondaryBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foretwist1"),
				OffHandItemSecondaryBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foretwist1"),
				OffHandShoulderBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_clavicle"),
				HandNumBonesForIk = 6,
				PrimaryFootBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foot"),
				SecondaryFootBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foot"),
				RightFootIkEndEffectorBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_foot"),
				LeftFootIkEndEffectorBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_foot"),
				RightFootIkTipBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "r_toe0"),
				LeftFootIkTipBoneIndex = Skeleton.GetBoneIndexFromName(actionSetWithIndex.GetSkeletonName(), "l_toe0"),
				FootNumBonesForIk = 3
			};
		}

		public const sbyte InvalidBoneIndex = -1;

		public const sbyte NumBonesForIkMaxCount = 8;

		public const sbyte MaxCountOfRagdollBonesToCheckForCorpses = 11;

		public const sbyte RagdollFallSoundBoneIndexMaxCount = 4;

		public const sbyte RagdollStationaryCheckBoneMaxCount = 8;

		public const sbyte MoveAdderBoneMaxCount = 7;

		public const sbyte SplashDecalBoneMaxCount = 6;

		public const sbyte BloodBurstBoneMaxCount = 8;

		public const sbyte BoneIndicesToModifyOnSlopingGroundMaxCount = 7;

		public MBActionSet ActionSet;

		public int NumPaces;

		public int MonsterUsageSetIndex;

		public float WalkingSpeedLimit;

		public float CrouchWalkingSpeedLimit;

		public float StepSize;

		[MarshalAs(UnmanagedType.I1)]
		public bool HasClippingPlane;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		public sbyte[] IndicesOfRagdollBonesToCheckForCorpses;

		public sbyte CountOfRagdollBonesToCheckForCorpses;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] RagdollFallSoundBoneIndices;

		public sbyte RagdollFallSoundBoneIndexCount;

		public sbyte HeadLookDirectionBoneIndex;

		public sbyte SpineLowerBoneIndex;

		public sbyte SpineUpperBoneIndex;

		public sbyte ThoraxLookDirectionBoneIndex;

		public sbyte NeckRootBoneIndex;

		public sbyte PelvisBoneIndex;

		public sbyte RightUpperArmBoneIndex;

		public sbyte LeftUpperArmBoneIndex;

		public sbyte FallBlowDamageBoneIndex;

		public sbyte TerrainDecalBone0Index;

		public sbyte TerrainDecalBone1Index;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public sbyte[] RagdollStationaryCheckBoneIndices;

		public sbyte RagdollStationaryCheckBoneCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public sbyte[] MoveAdderBoneIndices;

		public sbyte MoveAdderBoneCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public sbyte[] SplashDecalBoneIndices;

		public sbyte SplashDecalBoneCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public sbyte[] BloodBurstBoneIndices;

		public sbyte BloodBurstBoneCount;

		public sbyte MainHandBoneIndex;

		public sbyte OffHandBoneIndex;

		public sbyte MainHandItemBoneIndex;

		public sbyte OffHandItemBoneIndex;

		public sbyte MainHandItemSecondaryBoneIndex;

		public sbyte OffHandItemSecondaryBoneIndex;

		public sbyte OffHandShoulderBoneIndex;

		public sbyte HandNumBonesForIk;

		public sbyte PrimaryFootBoneIndex;

		public sbyte SecondaryFootBoneIndex;

		public sbyte RightFootIkEndEffectorBoneIndex;

		public sbyte LeftFootIkEndEffectorBoneIndex;

		public sbyte RightFootIkTipBoneIndex;

		public sbyte LeftFootIkTipBoneIndex;

		public sbyte FootNumBonesForIk;

		public Vec3 ReinHandleLeftLocalPosition;

		public Vec3 ReinHandleRightLocalPosition;

		public string ReinSkeleton;

		public string ReinCollisionBody;

		public sbyte IndexOfBoneToDetectGroundSlopeFront;

		public sbyte IndexOfBoneToDetectGroundSlopeBack;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public sbyte[] BoneIndicesToModifyOnSlopingGround;

		public sbyte BoneIndicesToModifyOnSlopingGroundCount;

		public sbyte BodyRotationReferenceBoneIndex;

		public sbyte RiderSitBoneIndex;

		public sbyte ReinHandleBoneIndex;

		public sbyte ReinCollision1BoneIndex;

		public sbyte ReinCollision2BoneIndex;

		public sbyte ReinHeadBoneIndex;

		public sbyte ReinHeadRightAttachmentBoneIndex;

		public sbyte ReinHeadLeftAttachmentBoneIndex;

		public sbyte ReinRightHandBoneIndex;

		public sbyte ReinLeftHandBoneIndex;
	}
}
