using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	public class PopupSceneSkeletonAnimationScript : ScriptComponentBehavior
	{
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void Initialize()
		{
			if (this.SkeletonName != "" && (base.GameEntity.Skeleton == null || base.GameEntity.Skeleton.GetName() != this.SkeletonName))
			{
				GameEntityExtensions.CreateSimpleSkeleton(base.GameEntity, this.SkeletonName);
				return;
			}
			if (this.SkeletonName == "" && base.GameEntity.Skeleton != null)
			{
				base.GameEntity.RemoveSkeleton();
			}
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		protected override void OnTick(float dt)
		{
			if (base.GameEntity.Skeleton.GetAnimationAtChannel(0) == this.InitialAnimationClip && base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) >= 1f)
			{
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, this.InitialAnimationContinueClip, 0, 1f, -1f, 0f);
			}
			if (base.GameEntity.Skeleton.GetAnimationAtChannel(0) == this.PositiveAnimationClip && base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) >= 1f)
			{
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, this.PositiveAnimationContinueClip, 0, 1f, -1f, 0f);
			}
			if (base.GameEntity.Skeleton.GetAnimationAtChannel(0) == this.NegativeAnimationClip && base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) >= 1f)
			{
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, this.NegativeAnimationContinueClip, 0, 1f, -1f, 0f);
			}
		}

		public void SetState(int state)
		{
			string text = ((state == 1) ? this.PositiveAnimationClip : ((state == 0) ? this.InitialAnimationClip : this.NegativeAnimationClip));
			GameEntity gameEntity = base.GameEntity;
			if (((gameEntity != null) ? gameEntity.Skeleton : null) != null && !string.IsNullOrEmpty(text))
			{
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, text, 0, 1f, -1f, 0f);
			}
			this._currentState = state;
		}

		public string SkeletonName = "";

		public int BoneIndex;

		public Vec3 AttachmentOffset = new Vec3(0f, 0f, 0f, -1f);

		public string InitialAnimationClip = "";

		public string PositiveAnimationClip = "";

		public string NegativeAnimationClip = "";

		public string InitialAnimationContinueClip = "";

		public string PositiveAnimationContinueClip = "";

		public string NegativeAnimationContinueClip = "";

		private int _currentState;
	}
}
