using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D4 RID: 724
	public static class MonsterExtensions
	{
		// Token: 0x06002807 RID: 10247 RVA: 0x0009AD9C File Offset: 0x00098F9C
		public static AnimationSystemData FillAnimationSystemData(this Monster monster, float stepSize, bool hasClippingPlane, bool isFemale)
		{
			MonsterMissionData monsterMissionData = (MonsterMissionData)monster.MonsterMissionData;
			MBActionSet mbactionSet = ((isFemale && monsterMissionData.FemaleActionSet.IsValid) ? monsterMissionData.FemaleActionSet : monsterMissionData.ActionSet);
			return monster.FillAnimationSystemData(mbactionSet, stepSize, hasClippingPlane);
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x0009ADE0 File Offset: 0x00098FE0
		public static AnimationSystemData FillAnimationSystemData(this Monster monster, MBActionSet actionSet, float stepSize, bool hasClippingPlane)
		{
			AnimationSystemData animationSystemData = new AnimationSystemData
			{
				ActionSet = actionSet,
				NumPaces = monster.NumPaces,
				MonsterUsageSetIndex = Agent.GetMonsterUsageIndex(monster.MonsterUsage),
				WalkingSpeedLimit = monster.WalkingSpeedLimit,
				CrouchWalkingSpeedLimit = monster.CrouchWalkingSpeedLimit,
				StepSize = stepSize,
				HasClippingPlane = hasClippingPlane,
				IndicesOfRagdollBonesToCheckForCorpses = new sbyte[11],
				CountOfRagdollBonesToCheckForCorpses = 0,
				RagdollFallSoundBoneIndices = new sbyte[4],
				RagdollFallSoundBoneIndexCount = 0,
				HeadLookDirectionBoneIndex = monster.HeadLookDirectionBoneIndex,
				SpineLowerBoneIndex = monster.SpineLowerBoneIndex,
				SpineUpperBoneIndex = monster.SpineUpperBoneIndex,
				ThoraxLookDirectionBoneIndex = monster.ThoraxLookDirectionBoneIndex,
				NeckRootBoneIndex = monster.NeckRootBoneIndex,
				PelvisBoneIndex = monster.PelvisBoneIndex,
				RightUpperArmBoneIndex = monster.RightUpperArmBoneIndex,
				LeftUpperArmBoneIndex = monster.LeftUpperArmBoneIndex,
				FallBlowDamageBoneIndex = monster.FallBlowDamageBoneIndex,
				TerrainDecalBone0Index = monster.TerrainDecalBone0Index,
				TerrainDecalBone1Index = monster.TerrainDecalBone1Index,
				RagdollStationaryCheckBoneIndices = new sbyte[8],
				RagdollStationaryCheckBoneCount = 0,
				MoveAdderBoneIndices = new sbyte[7],
				MoveAdderBoneCount = 0,
				SplashDecalBoneIndices = new sbyte[6],
				SplashDecalBoneCount = 0,
				BloodBurstBoneIndices = new sbyte[8],
				BloodBurstBoneCount = 0,
				MainHandBoneIndex = monster.MainHandBoneIndex,
				OffHandBoneIndex = monster.OffHandBoneIndex,
				MainHandItemBoneIndex = monster.MainHandItemBoneIndex,
				OffHandItemBoneIndex = monster.OffHandItemBoneIndex,
				MainHandItemSecondaryBoneIndex = monster.MainHandItemSecondaryBoneIndex,
				OffHandItemSecondaryBoneIndex = monster.OffHandItemSecondaryBoneIndex,
				OffHandShoulderBoneIndex = monster.OffHandShoulderBoneIndex,
				HandNumBonesForIk = monster.HandNumBonesForIk,
				PrimaryFootBoneIndex = monster.PrimaryFootBoneIndex,
				SecondaryFootBoneIndex = monster.SecondaryFootBoneIndex,
				RightFootIkEndEffectorBoneIndex = monster.RightFootIkEndEffectorBoneIndex,
				LeftFootIkEndEffectorBoneIndex = monster.LeftFootIkEndEffectorBoneIndex,
				RightFootIkTipBoneIndex = monster.RightFootIkTipBoneIndex,
				LeftFootIkTipBoneIndex = monster.LeftFootIkTipBoneIndex,
				FootNumBonesForIk = monster.FootNumBonesForIk,
				ReinHandleLeftLocalPosition = monster.ReinHandleLeftLocalPosition,
				ReinHandleRightLocalPosition = monster.ReinHandleRightLocalPosition,
				ReinSkeleton = monster.ReinSkeleton,
				ReinCollisionBody = monster.ReinCollisionBody,
				IndexOfBoneToDetectGroundSlopeFront = -1,
				IndexOfBoneToDetectGroundSlopeBack = -1,
				BoneIndicesToModifyOnSlopingGround = new sbyte[7],
				BoneIndicesToModifyOnSlopingGroundCount = 0,
				BodyRotationReferenceBoneIndex = monster.BodyRotationReferenceBoneIndex,
				RiderSitBoneIndex = monster.RiderSitBoneIndex,
				ReinHandleBoneIndex = monster.ReinHandleBoneIndex,
				ReinCollision1BoneIndex = monster.ReinCollision1BoneIndex,
				ReinCollision2BoneIndex = monster.ReinCollision2BoneIndex,
				ReinHeadBoneIndex = monster.ReinHeadBoneIndex,
				ReinHeadRightAttachmentBoneIndex = monster.ReinHeadRightAttachmentBoneIndex,
				ReinHeadLeftAttachmentBoneIndex = monster.ReinHeadLeftAttachmentBoneIndex,
				ReinRightHandBoneIndex = monster.ReinRightHandBoneIndex,
				ReinLeftHandBoneIndex = monster.ReinLeftHandBoneIndex
			};
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.IndicesOfRagdollBonesToCheckForCorpses, out animationSystemData.CountOfRagdollBonesToCheckForCorpses, 11, monster.IndicesOfRagdollBonesToCheckForCorpses);
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.RagdollFallSoundBoneIndices, out animationSystemData.RagdollFallSoundBoneIndexCount, 4, monster.RagdollFallSoundBoneIndices);
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.RagdollStationaryCheckBoneIndices, out animationSystemData.RagdollStationaryCheckBoneCount, 8, monster.RagdollStationaryCheckBoneIndices);
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.MoveAdderBoneIndices, out animationSystemData.MoveAdderBoneCount, 7, monster.MoveAdderBoneIndices);
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.SplashDecalBoneIndices, out animationSystemData.SplashDecalBoneCount, 6, monster.SplashDecalBoneIndices);
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.BloodBurstBoneIndices, out animationSystemData.BloodBurstBoneCount, 8, monster.BloodBurstBoneIndices);
			MonsterExtensions.CopyArrayAndTruncateSourceIfNecessary(ref animationSystemData.BoneIndicesToModifyOnSlopingGround, out animationSystemData.BoneIndicesToModifyOnSlopingGroundCount, 7, monster.BoneIndicesToModifyOnSlopingGround);
			if (animationSystemData.BoneIndicesToModifyOnSlopingGroundCount > 0 && monster.FrontBoneToDetectGroundSlopeIndex >= 0 && monster.FrontBoneToDetectGroundSlopeIndex < animationSystemData.BoneIndicesToModifyOnSlopingGroundCount && monster.BackBoneToDetectGroundSlopeIndex >= 0 && monster.BackBoneToDetectGroundSlopeIndex < animationSystemData.BoneIndicesToModifyOnSlopingGroundCount)
			{
				animationSystemData.IndexOfBoneToDetectGroundSlopeFront = animationSystemData.BoneIndicesToModifyOnSlopingGround[(int)monster.FrontBoneToDetectGroundSlopeIndex];
				animationSystemData.IndexOfBoneToDetectGroundSlopeBack = animationSystemData.BoneIndicesToModifyOnSlopingGround[(int)monster.BackBoneToDetectGroundSlopeIndex];
			}
			return animationSystemData;
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x0009B218 File Offset: 0x00099418
		private static bool CopyArrayAndTruncateSourceIfNecessary(ref sbyte[] destinationArray, out sbyte destinationArraySize, sbyte destinationArrayCapacity, sbyte[] sourceArray)
		{
			sbyte b = (sbyte)sourceArray.Length;
			bool flag = b <= destinationArrayCapacity;
			if (!flag)
			{
				Array.Resize<sbyte>(ref sourceArray, (int)destinationArrayCapacity);
				b = destinationArrayCapacity;
			}
			Array.Copy(sourceArray, destinationArray, (int)b);
			destinationArraySize = b;
			return flag;
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x0009B24C File Offset: 0x0009944C
		public static AgentCapsuleData FillCapsuleData(this Monster monster)
		{
			MonsterMissionData monsterMissionData = (MonsterMissionData)monster.MonsterMissionData;
			return new AgentCapsuleData
			{
				BodyCap = monsterMissionData.BodyCapsule,
				CrouchedBodyCap = monsterMissionData.CrouchedBodyCapsule
			};
		}

		// Token: 0x0600280B RID: 10251 RVA: 0x0009B288 File Offset: 0x00099488
		public static AgentSpawnData FillSpawnData(this Monster monster, ItemObject mountItem)
		{
			return new AgentSpawnData
			{
				HitPoints = monster.HitPoints,
				MonsterUsageIndex = Agent.GetMonsterUsageIndex(monster.MonsterUsage),
				Weight = ((mountItem != null) ? ((int)mountItem.Weight) : monster.Weight),
				StandingEyeHeight = monster.StandingEyeHeight,
				CrouchEyeHeight = monster.CrouchEyeHeight,
				MountedEyeHeight = monster.MountedEyeHeight,
				RiderEyeHeightAdder = monster.RiderEyeHeightAdder,
				JumpAcceleration = monster.JumpAcceleration,
				EyeOffsetWrtHead = monster.EyeOffsetWrtHead,
				FirstPersonCameraOffsetWrtHead = monster.FirstPersonCameraOffsetWrtHead,
				RiderCameraHeightAdder = monster.RiderCameraHeightAdder,
				RiderBodyCapsuleHeightAdder = monster.RiderBodyCapsuleHeightAdder,
				RiderBodyCapsuleForwardAdder = monster.RiderBodyCapsuleForwardAdder,
				ArmLength = monster.ArmLength,
				ArmWeight = monster.ArmWeight,
				JumpSpeedLimit = monster.JumpSpeedLimit,
				RelativeSpeedLimitForCharge = monster.RelativeSpeedLimitForCharge
			};
		}
	}
}
