using System;
using System.Collections.Generic;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Cinematics
{
	public class SkeletonAnimatedCamera : ScriptComponentBehavior
	{
		private void CreateVisualizer()
		{
			if (this.SkeletonName != "" && this.AnimationName != "")
			{
				GameEntityExtensions.CreateSimpleSkeleton(base.GameEntity, this.SkeletonName);
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, this.AnimationName, 0, 1f, -1f, 0f);
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.CreateVisualizer();
		}

		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.OnInit();
		}

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

		protected override void OnEditorTick(float dt)
		{
			this.OnTick(dt);
		}

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

		public string SkeletonName = "human_skeleton";

		public int BoneIndex;

		public Vec3 AttachmentOffset = new Vec3(0f, 0f, 0f, -1f);

		public string AnimationName = "";

		public SimpleButton Restart;
	}
}
