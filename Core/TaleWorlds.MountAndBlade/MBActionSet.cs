using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("int", false)]
	public struct MBActionSet
	{
		internal MBActionSet(int i)
		{
			this.Index = i;
		}

		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		public bool Equals(MBActionSet a)
		{
			return this.Index == a.Index;
		}

		public bool Equals(int index)
		{
			return this.Index == index;
		}

		public override int GetHashCode()
		{
			return this.Index;
		}

		public string GetName()
		{
			if (!this.IsValid)
			{
				return "Invalid";
			}
			return MBAPI.IMBActionSet.GetNameWithIndex(this.Index);
		}

		public string GetSkeletonName()
		{
			return MBAPI.IMBActionSet.GetSkeletonName(this.Index);
		}

		public string GetAnimationName(ActionIndexCache actionCode)
		{
			return MBAPI.IMBActionSet.GetAnimationName(this.Index, actionCode.Index);
		}

		public bool AreActionsAlternatives(ActionIndexCache actionCode1, ActionIndexCache actionCode2)
		{
			return MBAPI.IMBActionSet.AreActionsAlternatives(this.Index, actionCode1.Index, actionCode2.Index);
		}

		public bool AreActionsAlternatives(ActionIndexValueCache actionCode1, ActionIndexCache actionCode2)
		{
			return MBAPI.IMBActionSet.AreActionsAlternatives(this.Index, actionCode1.Index, actionCode2.Index);
		}

		public static int GetNumberOfActionSets()
		{
			return MBAPI.IMBActionSet.GetNumberOfActionSets();
		}

		public static int GetNumberOfMonsterUsageSets()
		{
			return MBAPI.IMBActionSet.GetNumberOfMonsterUsageSets();
		}

		public static MBActionSet GetActionSet(string objectID)
		{
			return MBActionSet.GetActionSetWithIndex(MBAPI.IMBActionSet.GetIndexWithID(objectID));
		}

		public static MBActionSet GetActionSetWithIndex(int index)
		{
			return new MBActionSet(index);
		}

		public static sbyte GetBoneIndexWithId(string actionSetId, string boneId)
		{
			return MBAPI.IMBActionSet.GetBoneIndexWithId(actionSetId, boneId);
		}

		public static bool GetBoneHasParentBone(string actionSetId, sbyte boneIndex)
		{
			return MBAPI.IMBActionSet.GetBoneHasParentBone(actionSetId, boneIndex);
		}

		public static Vec3 GetActionDisplacementVector(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetDisplacementVector(actionSet.Index, actionIndexCache.Index);
		}

		public static AnimFlags GetActionAnimationFlags(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetAnimationFlags(actionSet.Index, actionIndexCache.Index);
		}

		public static bool CheckActionAnimationClipExists(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.CheckAnimationClipExists(actionSet.Index, actionIndexCache.Index);
		}

		public static int GetAnimationIndexOfAction(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.AnimationIndexOfActionCode(actionSet.Index, actionIndexCache.Index);
		}

		public static int GetAnimationIndexOfAction(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.AnimationIndexOfActionCode(actionSet.Index, actionIndexCache.Index);
		}

		public static string GetActionAnimationName(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetAnimationName(actionSet.Index, actionIndexCache.Index);
		}

		public static float GetActionAnimationDuration(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionAnimationDuration(actionSet.Index, actionIndexCache.Index);
		}

		public static float GetActionAnimationDuration(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionAnimationDuration(actionSet.Index, actionIndexCache.Index);
		}

		public static ActionIndexValueCache GetActionAnimationContinueToAction(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return new ActionIndexValueCache(MBAPI.IMBAnimation.GetAnimationContinueToAction(actionSet.Index, actionIndexCache.Index));
		}

		public static float GetTotalAnimationDurationWithContinueToAction(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			float num = 0f;
			while (actionIndexCache != ActionIndexValueCache.act_none)
			{
				num += MBActionSet.GetActionAnimationDuration(actionSet, actionIndexCache);
				actionIndexCache = MBActionSet.GetActionAnimationContinueToAction(actionSet, actionIndexCache);
			}
			return num;
		}

		public static float GetActionBlendOutStartProgress(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionBlendOutStartProgress(actionSet.Index, actionIndexCache.Index);
		}

		public static float GetActionBlendOutStartProgress(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionBlendOutStartProgress(actionSet.Index, actionIndexCache.Index);
		}

		[CustomEngineStructMemberData("ignoredMember", true)]
		internal readonly int Index;

		public static readonly MBActionSet InvalidActionSet = new MBActionSet(-1);
	}
}
