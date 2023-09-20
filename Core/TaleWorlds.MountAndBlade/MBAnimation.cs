using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200018A RID: 394
	public struct MBAnimation
	{
		// Token: 0x060014AD RID: 5293 RVA: 0x0004F2D1 File Offset: 0x0004D4D1
		public MBAnimation(MBAnimation animation)
		{
			this._index = animation._index;
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x0004F2DF File Offset: 0x0004D4DF
		internal MBAnimation(int i)
		{
			this._index = i;
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x0004F2E8 File Offset: 0x0004D4E8
		public bool Equals(MBAnimation a)
		{
			return this._index == a._index;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x0004F2F8 File Offset: 0x0004D4F8
		public override int GetHashCode()
		{
			return this._index;
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x0004F300 File Offset: 0x0004D500
		public static int GetAnimationIndexWithName(string animationName)
		{
			if (string.IsNullOrEmpty(animationName))
			{
				return -1;
			}
			return MBAPI.IMBAnimation.GetIndexWithID(animationName);
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x0004F317 File Offset: 0x0004D517
		public static Agent.ActionCodeType GetActionType(ActionIndexCache actionIndex)
		{
			if (!(actionIndex == ActionIndexCache.act_none))
			{
				return MBAPI.IMBAnimation.GetActionType(actionIndex.Index);
			}
			return Agent.ActionCodeType.Other;
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x0004F338 File Offset: 0x0004D538
		public static Agent.ActionCodeType GetActionType(ActionIndexValueCache actionIndex)
		{
			if (!(actionIndex == ActionIndexValueCache.act_none))
			{
				return MBAPI.IMBAnimation.GetActionType(actionIndex.Index);
			}
			return Agent.ActionCodeType.Other;
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x0004F35A File Offset: 0x0004D55A
		public static void PrefetchAnimationClip(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			MBAPI.IMBAnimation.PrefetchAnimationClip(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x0004F374 File Offset: 0x0004D574
		public static float GetAnimationDuration(string animationName)
		{
			int indexWithID = MBAPI.IMBAnimation.GetIndexWithID(animationName);
			return MBAPI.IMBAnimation.GetAnimationDuration(indexWithID);
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x0004F398 File Offset: 0x0004D598
		public static float GetAnimationDuration(int animationIndex)
		{
			return MBAPI.IMBAnimation.GetAnimationDuration(animationIndex);
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x0004F3A8 File Offset: 0x0004D5A8
		public static float GetAnimationParameter1(string animationName)
		{
			int indexWithID = MBAPI.IMBAnimation.GetIndexWithID(animationName);
			return MBAPI.IMBAnimation.GetAnimationParameter1(indexWithID);
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x0004F3CC File Offset: 0x0004D5CC
		public static float GetAnimationParameter1(int animationIndex)
		{
			return MBAPI.IMBAnimation.GetAnimationParameter1(animationIndex);
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x0004F3DC File Offset: 0x0004D5DC
		public static float GetAnimationParameter2(string animationName)
		{
			int indexWithID = MBAPI.IMBAnimation.GetIndexWithID(animationName);
			return MBAPI.IMBAnimation.GetAnimationParameter2(indexWithID);
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x0004F400 File Offset: 0x0004D600
		public static float GetAnimationParameter2(int animationIndex)
		{
			return MBAPI.IMBAnimation.GetAnimationParameter2(animationIndex);
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x0004F410 File Offset: 0x0004D610
		public static float GetAnimationParameter3(string animationName)
		{
			int indexWithID = MBAPI.IMBAnimation.GetIndexWithID(animationName);
			return MBAPI.IMBAnimation.GetAnimationParameter3(indexWithID);
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x0004F434 File Offset: 0x0004D634
		public static float GetAnimationBlendInPeriod(string animationName)
		{
			int indexWithID = MBAPI.IMBAnimation.GetIndexWithID(animationName);
			return MBAPI.IMBAnimation.GetAnimationBlendInPeriod(indexWithID);
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x0004F458 File Offset: 0x0004D658
		public static float GetAnimationBlendInPeriod(int animationIndex)
		{
			return MBAPI.IMBAnimation.GetAnimationBlendInPeriod(animationIndex);
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x0004F468 File Offset: 0x0004D668
		public static int GetActionCodeWithName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return ActionIndexValueCache.act_none.Index;
			}
			return MBAPI.IMBAnimation.GetActionCodeWithName(name);
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x0004F496 File Offset: 0x0004D696
		public static string GetActionNameWithCode(int actionIndex)
		{
			if (actionIndex == -1)
			{
				return "act_none";
			}
			return MBAPI.IMBAnimation.GetActionNameWithCode(actionIndex);
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x0004F4AD File Offset: 0x0004D6AD
		public static int GetNumActionCodes()
		{
			return MBAPI.IMBAnimation.GetNumActionCodes();
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0004F4B9 File Offset: 0x0004D6B9
		public static int GetNumAnimations()
		{
			return MBAPI.IMBAnimation.GetNumAnimations();
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x0004F4C5 File Offset: 0x0004D6C5
		public static bool IsAnyAnimationLoadingFromDisk()
		{
			return MBAPI.IMBAnimation.IsAnyAnimationLoadingFromDisk();
		}

		// Token: 0x04000729 RID: 1833
		private readonly int _index;
	}
}
