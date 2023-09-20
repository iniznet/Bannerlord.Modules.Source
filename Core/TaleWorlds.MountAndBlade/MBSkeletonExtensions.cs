using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C6 RID: 454
	public static class MBSkeletonExtensions
	{
		// Token: 0x060019E4 RID: 6628 RVA: 0x0005C1CA File Offset: 0x0005A3CA
		public static Skeleton CreateWithActionSet(ref AnimationSystemData animationSystemData)
		{
			return MBAPI.IMBSkeletonExtensions.CreateWithActionSet(ref animationSystemData);
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x0005C1D7 File Offset: 0x0005A3D7
		public static float GetSkeletonFaceAnimationTime(Skeleton skeleton)
		{
			return MBAPI.IMBSkeletonExtensions.GetSkeletonFaceAnimationTime(skeleton.Pointer);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x0005C1E9 File Offset: 0x0005A3E9
		public static void SetSkeletonFaceAnimationTime(Skeleton skeleton, float time)
		{
			MBAPI.IMBSkeletonExtensions.SetSkeletonFaceAnimationTime(skeleton.Pointer, time);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x0005C1FC File Offset: 0x0005A3FC
		public static string GetSkeletonFaceAnimationName(Skeleton skeleton)
		{
			return MBAPI.IMBSkeletonExtensions.GetSkeletonFaceAnimationName(skeleton.Pointer);
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x0005C210 File Offset: 0x0005A410
		public static MatrixFrame GetBoneEntitialFrameAtAnimationProgress(this Skeleton skeleton, sbyte boneIndex, int animationIndex, float progress)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBSkeletonExtensions.GetBoneEntitialFrameAtAnimationProgress(skeleton.Pointer, boneIndex, animationIndex, progress, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x0005C23C File Offset: 0x0005A43C
		public static MatrixFrame GetBoneEntitialFrame(this Skeleton skeleton, sbyte boneNumber, bool forceToUpdate = false)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBSkeletonExtensions.GetBoneEntitialFrame(skeleton.Pointer, boneNumber, false, forceToUpdate, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x0005C267 File Offset: 0x0005A467
		public static void SetFacialAnimation(this Skeleton skeleton, Agent.FacialAnimChannel channel, string faceAnimation, bool playSound, bool loop)
		{
			MBAPI.IMBSkeletonExtensions.SetFacialAnimationOfChannel(skeleton.Pointer, (int)channel, faceAnimation, playSound, loop);
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x0005C27E File Offset: 0x0005A47E
		public static void SetAgentActionChannel(this Skeleton skeleton, int actionChannelNo, ActionIndexCache actionIndex, float channelParameter = 0f, float blendPeriodOverride = -0.2f, bool forceFaceMorphRestart = true)
		{
			MBAPI.IMBSkeletonExtensions.SetAgentActionChannel(skeleton.Pointer, actionChannelNo, actionIndex.Index, channelParameter, blendPeriodOverride, forceFaceMorphRestart);
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x0005C29C File Offset: 0x0005A49C
		public static bool DoesActionContinueWithCurrentActionAtChannel(this Skeleton skeleton, int actionChannelNo, ActionIndexCache actionIndex)
		{
			return MBAPI.IMBSkeletonExtensions.DoesActionContinueWithCurrentActionAtChannel(skeleton.Pointer, actionChannelNo, actionIndex.Index);
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x0005C2B5 File Offset: 0x0005A4B5
		public static void TickActionChannels(this Skeleton skeleton)
		{
			MBAPI.IMBSkeletonExtensions.TickActionChannels(skeleton.Pointer);
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0005C2C8 File Offset: 0x0005A4C8
		public static void SetAnimationAtChannel(this Skeleton skeleton, string animationName, int channelNo, float animationSpeedMultiplier = 1f, float blendInPeriod = -1f, float startProgress = 0f)
		{
			int indexWithID = MBAPI.IMBAnimation.GetIndexWithID(animationName);
			skeleton.SetAnimationAtChannel(indexWithID, channelNo, animationSpeedMultiplier, blendInPeriod, startProgress);
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x0005C2EE File Offset: 0x0005A4EE
		public static void SetAnimationAtChannel(this Skeleton skeleton, int animationIndex, int channelNo, float animationSpeedMultiplier = 1f, float blendInPeriod = -1f, float startProgress = 0f)
		{
			MBAPI.IMBSkeletonExtensions.SetAnimationAtChannel(skeleton.Pointer, animationIndex, channelNo, animationSpeedMultiplier, blendInPeriod, startProgress);
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x0005C307 File Offset: 0x0005A507
		public static ActionIndexCache GetActionAtChannel(this Skeleton skeleton, int channelNo)
		{
			return new ActionIndexCache(MBAPI.IMBSkeletonExtensions.GetActionAtChannel(skeleton.Pointer, channelNo));
		}
	}
}
