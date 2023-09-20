using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200017B RID: 379
	[EngineStruct("Animation_system_data")]
	[Serializable]
	public struct AnimationSystemData
	{
		// Token: 0x06001375 RID: 4981 RVA: 0x0004C7A8 File Offset: 0x0004A9A8
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

		// Token: 0x04000597 RID: 1431
		public const sbyte InvalidBoneIndex = -1;

		// Token: 0x04000598 RID: 1432
		public const sbyte NumBonesForIkMaxCount = 8;

		// Token: 0x04000599 RID: 1433
		public const sbyte MaxCountOfRagdollBonesToCheckForCorpses = 11;

		// Token: 0x0400059A RID: 1434
		public const sbyte RagdollFallSoundBoneIndexMaxCount = 4;

		// Token: 0x0400059B RID: 1435
		public const sbyte RagdollStationaryCheckBoneMaxCount = 8;

		// Token: 0x0400059C RID: 1436
		public const sbyte MoveAdderBoneMaxCount = 7;

		// Token: 0x0400059D RID: 1437
		public const sbyte SplashDecalBoneMaxCount = 6;

		// Token: 0x0400059E RID: 1438
		public const sbyte BloodBurstBoneMaxCount = 8;

		// Token: 0x0400059F RID: 1439
		public const sbyte BoneIndicesToModifyOnSlopingGroundMaxCount = 7;

		// Token: 0x040005A0 RID: 1440
		public MBActionSet ActionSet;

		// Token: 0x040005A1 RID: 1441
		public int NumPaces;

		// Token: 0x040005A2 RID: 1442
		public int MonsterUsageSetIndex;

		// Token: 0x040005A3 RID: 1443
		public float WalkingSpeedLimit;

		// Token: 0x040005A4 RID: 1444
		public float CrouchWalkingSpeedLimit;

		// Token: 0x040005A5 RID: 1445
		public float StepSize;

		// Token: 0x040005A6 RID: 1446
		[MarshalAs(UnmanagedType.I1)]
		public bool HasClippingPlane;

		// Token: 0x040005A7 RID: 1447
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		public sbyte[] IndicesOfRagdollBonesToCheckForCorpses;

		// Token: 0x040005A8 RID: 1448
		public sbyte CountOfRagdollBonesToCheckForCorpses;

		// Token: 0x040005A9 RID: 1449
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] RagdollFallSoundBoneIndices;

		// Token: 0x040005AA RID: 1450
		public sbyte RagdollFallSoundBoneIndexCount;

		// Token: 0x040005AB RID: 1451
		public sbyte HeadLookDirectionBoneIndex;

		// Token: 0x040005AC RID: 1452
		public sbyte SpineLowerBoneIndex;

		// Token: 0x040005AD RID: 1453
		public sbyte SpineUpperBoneIndex;

		// Token: 0x040005AE RID: 1454
		public sbyte ThoraxLookDirectionBoneIndex;

		// Token: 0x040005AF RID: 1455
		public sbyte NeckRootBoneIndex;

		// Token: 0x040005B0 RID: 1456
		public sbyte PelvisBoneIndex;

		// Token: 0x040005B1 RID: 1457
		public sbyte RightUpperArmBoneIndex;

		// Token: 0x040005B2 RID: 1458
		public sbyte LeftUpperArmBoneIndex;

		// Token: 0x040005B3 RID: 1459
		public sbyte FallBlowDamageBoneIndex;

		// Token: 0x040005B4 RID: 1460
		public sbyte TerrainDecalBone0Index;

		// Token: 0x040005B5 RID: 1461
		public sbyte TerrainDecalBone1Index;

		// Token: 0x040005B6 RID: 1462
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public sbyte[] RagdollStationaryCheckBoneIndices;

		// Token: 0x040005B7 RID: 1463
		public sbyte RagdollStationaryCheckBoneCount;

		// Token: 0x040005B8 RID: 1464
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public sbyte[] MoveAdderBoneIndices;

		// Token: 0x040005B9 RID: 1465
		public sbyte MoveAdderBoneCount;

		// Token: 0x040005BA RID: 1466
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public sbyte[] SplashDecalBoneIndices;

		// Token: 0x040005BB RID: 1467
		public sbyte SplashDecalBoneCount;

		// Token: 0x040005BC RID: 1468
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public sbyte[] BloodBurstBoneIndices;

		// Token: 0x040005BD RID: 1469
		public sbyte BloodBurstBoneCount;

		// Token: 0x040005BE RID: 1470
		public sbyte MainHandBoneIndex;

		// Token: 0x040005BF RID: 1471
		public sbyte OffHandBoneIndex;

		// Token: 0x040005C0 RID: 1472
		public sbyte MainHandItemBoneIndex;

		// Token: 0x040005C1 RID: 1473
		public sbyte OffHandItemBoneIndex;

		// Token: 0x040005C2 RID: 1474
		public sbyte MainHandItemSecondaryBoneIndex;

		// Token: 0x040005C3 RID: 1475
		public sbyte OffHandItemSecondaryBoneIndex;

		// Token: 0x040005C4 RID: 1476
		public sbyte OffHandShoulderBoneIndex;

		// Token: 0x040005C5 RID: 1477
		public sbyte HandNumBonesForIk;

		// Token: 0x040005C6 RID: 1478
		public sbyte PrimaryFootBoneIndex;

		// Token: 0x040005C7 RID: 1479
		public sbyte SecondaryFootBoneIndex;

		// Token: 0x040005C8 RID: 1480
		public sbyte RightFootIkEndEffectorBoneIndex;

		// Token: 0x040005C9 RID: 1481
		public sbyte LeftFootIkEndEffectorBoneIndex;

		// Token: 0x040005CA RID: 1482
		public sbyte RightFootIkTipBoneIndex;

		// Token: 0x040005CB RID: 1483
		public sbyte LeftFootIkTipBoneIndex;

		// Token: 0x040005CC RID: 1484
		public sbyte FootNumBonesForIk;

		// Token: 0x040005CD RID: 1485
		public Vec3 ReinHandleLeftLocalPosition;

		// Token: 0x040005CE RID: 1486
		public Vec3 ReinHandleRightLocalPosition;

		// Token: 0x040005CF RID: 1487
		public string ReinSkeleton;

		// Token: 0x040005D0 RID: 1488
		public string ReinCollisionBody;

		// Token: 0x040005D1 RID: 1489
		public sbyte IndexOfBoneToDetectGroundSlopeFront;

		// Token: 0x040005D2 RID: 1490
		public sbyte IndexOfBoneToDetectGroundSlopeBack;

		// Token: 0x040005D3 RID: 1491
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public sbyte[] BoneIndicesToModifyOnSlopingGround;

		// Token: 0x040005D4 RID: 1492
		public sbyte BoneIndicesToModifyOnSlopingGroundCount;

		// Token: 0x040005D5 RID: 1493
		public sbyte BodyRotationReferenceBoneIndex;

		// Token: 0x040005D6 RID: 1494
		public sbyte RiderSitBoneIndex;

		// Token: 0x040005D7 RID: 1495
		public sbyte ReinHandleBoneIndex;

		// Token: 0x040005D8 RID: 1496
		public sbyte ReinCollision1BoneIndex;

		// Token: 0x040005D9 RID: 1497
		public sbyte ReinCollision2BoneIndex;

		// Token: 0x040005DA RID: 1498
		public sbyte ReinHeadBoneIndex;

		// Token: 0x040005DB RID: 1499
		public sbyte ReinHeadRightAttachmentBoneIndex;

		// Token: 0x040005DC RID: 1500
		public sbyte ReinHeadLeftAttachmentBoneIndex;

		// Token: 0x040005DD RID: 1501
		public sbyte ReinRightHandBoneIndex;

		// Token: 0x040005DE RID: 1502
		public sbyte ReinLeftHandBoneIndex;
	}
}
