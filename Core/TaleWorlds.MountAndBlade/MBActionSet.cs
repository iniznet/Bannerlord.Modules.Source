using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000183 RID: 387
	public struct MBActionSet
	{
		// Token: 0x06001424 RID: 5156 RVA: 0x0004E61A File Offset: 0x0004C81A
		internal MBActionSet(int i)
		{
			this.Index = i;
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x0004E623 File Offset: 0x0004C823
		public bool IsValid
		{
			get
			{
				return this.Index >= 0;
			}
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x0004E631 File Offset: 0x0004C831
		public bool Equals(MBActionSet a)
		{
			return this.Index == a.Index;
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x0004E641 File Offset: 0x0004C841
		public bool Equals(int index)
		{
			return this.Index == index;
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0004E64C File Offset: 0x0004C84C
		public override int GetHashCode()
		{
			return this.Index;
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x0004E654 File Offset: 0x0004C854
		public string GetName()
		{
			if (!this.IsValid)
			{
				return "Invalid";
			}
			return MBAPI.IMBActionSet.GetNameWithIndex(this.Index);
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0004E674 File Offset: 0x0004C874
		public string GetSkeletonName()
		{
			return MBAPI.IMBActionSet.GetSkeletonName(this.Index);
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0004E686 File Offset: 0x0004C886
		public string GetAnimationName(ActionIndexCache actionCode)
		{
			return MBAPI.IMBActionSet.GetAnimationName(this.Index, actionCode.Index);
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x0004E69E File Offset: 0x0004C89E
		public bool AreActionsAlternatives(ActionIndexCache actionCode1, ActionIndexCache actionCode2)
		{
			return MBAPI.IMBActionSet.AreActionsAlternatives(this.Index, actionCode1.Index, actionCode2.Index);
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0004E6BC File Offset: 0x0004C8BC
		public bool AreActionsAlternatives(ActionIndexValueCache actionCode1, ActionIndexCache actionCode2)
		{
			return MBAPI.IMBActionSet.AreActionsAlternatives(this.Index, actionCode1.Index, actionCode2.Index);
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0004E6DB File Offset: 0x0004C8DB
		public static int GetNumberOfActionSets()
		{
			return MBAPI.IMBActionSet.GetNumberOfActionSets();
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0004E6E7 File Offset: 0x0004C8E7
		public static int GetNumberOfMonsterUsageSets()
		{
			return MBAPI.IMBActionSet.GetNumberOfMonsterUsageSets();
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0004E6F3 File Offset: 0x0004C8F3
		public static MBActionSet GetActionSet(string objectID)
		{
			return MBActionSet.GetActionSetWithIndex(MBAPI.IMBActionSet.GetIndexWithID(objectID));
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0004E705 File Offset: 0x0004C905
		public static MBActionSet GetActionSetWithIndex(int index)
		{
			return new MBActionSet(index);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0004E70D File Offset: 0x0004C90D
		public static sbyte GetBoneIndexWithId(string actionSetId, string boneId)
		{
			return MBAPI.IMBActionSet.GetBoneIndexWithId(actionSetId, boneId);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x0004E71B File Offset: 0x0004C91B
		public static bool GetBoneHasParentBone(string actionSetId, sbyte boneIndex)
		{
			return MBAPI.IMBActionSet.GetBoneHasParentBone(actionSetId, boneIndex);
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0004E729 File Offset: 0x0004C929
		public static Vec3 GetActionDisplacementVector(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetDisplacementVector(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0004E741 File Offset: 0x0004C941
		public static AnimFlags GetActionAnimationFlags(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetAnimationFlags(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0004E759 File Offset: 0x0004C959
		public static bool CheckActionAnimationClipExists(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.CheckAnimationClipExists(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0004E771 File Offset: 0x0004C971
		public static int GetAnimationIndexOfAction(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.AnimationIndexOfActionCode(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0004E789 File Offset: 0x0004C989
		public static int GetAnimationIndexOfAction(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.AnimationIndexOfActionCode(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0004E7A2 File Offset: 0x0004C9A2
		public static string GetActionAnimationName(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetAnimationName(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x0004E7BA File Offset: 0x0004C9BA
		public static float GetActionAnimationDuration(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionAnimationDuration(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x0004E7D2 File Offset: 0x0004C9D2
		public static float GetActionAnimationDuration(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionAnimationDuration(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x0004E7EB File Offset: 0x0004C9EB
		public static ActionIndexValueCache GetActionAnimationContinueToAction(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return new ActionIndexValueCache(MBAPI.IMBAnimation.GetAnimationContinueToAction(actionSet.Index, actionIndexCache.Index));
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x0004E80C File Offset: 0x0004CA0C
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

		// Token: 0x0600143E RID: 5182 RVA: 0x0004E842 File Offset: 0x0004CA42
		public static float GetActionBlendOutStartProgress(MBActionSet actionSet, ActionIndexCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionBlendOutStartProgress(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0004E85A File Offset: 0x0004CA5A
		public static float GetActionBlendOutStartProgress(MBActionSet actionSet, ActionIndexValueCache actionIndexCache)
		{
			return MBAPI.IMBAnimation.GetActionBlendOutStartProgress(actionSet.Index, actionIndexCache.Index);
		}

		// Token: 0x040006C5 RID: 1733
		internal readonly int Index;

		// Token: 0x040006C6 RID: 1734
		public static readonly MBActionSet InvalidActionSet = new MBActionSet(-1);
	}
}
