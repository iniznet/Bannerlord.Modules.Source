using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x0200003E RID: 62
	public class HandPose : ScriptComponentBehavior
	{
		// Token: 0x060002DC RID: 732 RVA: 0x00019AC4 File Offset: 0x00017CC4
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			if (Game.Current == null)
			{
				this._editorGameManager = new EditorGameManager();
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00019AE0 File Offset: 0x00017CE0
		protected override void OnEditorTick(float dt)
		{
			if (!this._isFinished && this._editorGameManager != null)
			{
				this._isFinished = !this._editorGameManager.DoLoadingForGameManager();
			}
			if (Game.Current != null && !this._initiliazed)
			{
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(Game.Current.DefaultMonster, MBActionSet.GetActionSet(Game.Current.DefaultMonster.ActionSetCode), 1f, false);
				GameEntityExtensions.CreateSkeletonWithActionSet(base.GameEntity, ref animationSystemData);
				base.GameEntity.CopyComponentsToSkeleton();
				MBSkeletonExtensions.SetAgentActionChannel(base.GameEntity.Skeleton, 0, ActionIndexCache.Create("act_tableau_hand_armor_pose"), 0f, -0.2f, true);
				base.GameEntity.Skeleton.TickAnimationsAndForceUpdate(0.01f, base.GameEntity.GetGlobalFrame(), true);
				base.GameEntity.Skeleton.Freeze(false);
				base.GameEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, base.GameEntity.GetGlobalFrame(), false);
				base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(0f, 0f, 1f));
				base.GameEntity.Skeleton.SetUptoDate(false);
				base.GameEntity.Skeleton.Freeze(true);
				this._initiliazed = true;
			}
			if (this._initiliazed)
			{
				base.GameEntity.Skeleton.Freeze(false);
				base.GameEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, base.GameEntity.GetGlobalFrame(), false);
				base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(0f, 0f, 1f));
				base.GameEntity.Skeleton.SetUptoDate(false);
				base.GameEntity.Skeleton.Freeze(true);
			}
		}

		// Token: 0x040001FC RID: 508
		private MBGameManager _editorGameManager;

		// Token: 0x040001FD RID: 509
		private bool _initiliazed;

		// Token: 0x040001FE RID: 510
		private bool _isFinished;
	}
}
