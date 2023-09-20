using System;
using System.Collections.Generic;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Cinematics
{
	// Token: 0x0200002D RID: 45
	public class SkeletonAnimatedCamera : ScriptComponentBehavior
	{
		// Token: 0x06000208 RID: 520 RVA: 0x0000DE70 File Offset: 0x0000C070
		private void CreateVisualizer()
		{
			if (this.SkeletonName != "" && this.AnimationName != "")
			{
				GameEntityExtensions.CreateSimpleSkeleton(base.GameEntity, this.SkeletonName);
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, this.AnimationName, 0, 1f, -1f, 0f);
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000DED8 File Offset: 0x0000C0D8
		protected override void OnInit()
		{
			base.OnInit();
			this.CreateVisualizer();
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000DEE6 File Offset: 0x0000C0E6
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.OnInit();
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000DEF4 File Offset: 0x0000C0F4
		protected override void OnTick(float dt)
		{
			GameEntity gameEntity = base.GameEntity.Scene.FindEntityWithTag("camera_instance");
			if (gameEntity != null && base.GameEntity.Skeleton != null)
			{
				MatrixFrame matrixFrame = base.GameEntity.Skeleton.GetBoneEntitialFrame((sbyte)this.BoneIndex);
				matrixFrame = base.GameEntity.GetGlobalFrame().TransformToParent(matrixFrame);
				MatrixFrame matrixFrame2 = default(MatrixFrame);
				matrixFrame2.rotation = matrixFrame.rotation;
				matrixFrame2.rotation.u = -matrixFrame.rotation.s;
				matrixFrame2.rotation.f = -matrixFrame.rotation.u;
				matrixFrame2.rotation.s = matrixFrame.rotation.f;
				matrixFrame2.origin = matrixFrame.origin + this.AttachmentOffset;
				gameEntity.SetGlobalFrame(ref matrixFrame2);
				SoundManager.SetListenerFrame(matrixFrame2);
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000DFF0 File Offset: 0x0000C1F0
		protected override void OnEditorTick(float dt)
		{
			this.OnTick(dt);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000DFFC File Offset: 0x0000C1FC
		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "SkeletonName" || variableName == "AnimationName")
			{
				this.CreateVisualizer();
			}
			if (variableName == "Restart")
			{
				List<GameEntity> list = new List<GameEntity>();
				base.GameEntity.Scene.GetAllEntitiesWithScriptComponent<AnimationPoint>(ref list);
				foreach (GameEntity gameEntity in list)
				{
					gameEntity.GetFirstScriptOfType<AnimationPoint>().RequestResync();
				}
				this.CreateVisualizer();
			}
		}

		// Token: 0x040000EB RID: 235
		public string SkeletonName = "human_skeleton";

		// Token: 0x040000EC RID: 236
		public int BoneIndex;

		// Token: 0x040000ED RID: 237
		public Vec3 AttachmentOffset = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x040000EE RID: 238
		public string AnimationName = "";

		// Token: 0x040000EF RID: 239
		public SimpleButton Restart;
	}
}
