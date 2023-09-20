using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000015 RID: 21
	public class PopupSceneSkeletonAnimationScript : ScriptComponentBehavior
	{
		// Token: 0x0600008D RID: 141 RVA: 0x000061F0 File Offset: 0x000043F0
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006204 File Offset: 0x00004404
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

		// Token: 0x0600008F RID: 143 RVA: 0x00006295 File Offset: 0x00004495
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00006298 File Offset: 0x00004498
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

		// Token: 0x06000091 RID: 145 RVA: 0x000063BC File Offset: 0x000045BC
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

		// Token: 0x0400001E RID: 30
		public string SkeletonName = "";

		// Token: 0x0400001F RID: 31
		public int BoneIndex;

		// Token: 0x04000020 RID: 32
		public Vec3 AttachmentOffset = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x04000021 RID: 33
		public string InitialAnimationClip = "";

		// Token: 0x04000022 RID: 34
		public string PositiveAnimationClip = "";

		// Token: 0x04000023 RID: 35
		public string NegativeAnimationClip = "";

		// Token: 0x04000024 RID: 36
		public string InitialAnimationContinueClip = "";

		// Token: 0x04000025 RID: 37
		public string PositiveAnimationContinueClip = "";

		// Token: 0x04000026 RID: 38
		public string NegativeAnimationContinueClip = "";

		// Token: 0x04000027 RID: 39
		private int _currentState;
	}
}
