using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Cinematics
{
	public class HideoutBossFightBehavior : ScriptComponentBehavior
	{
		public int PerturbSeed
		{
			get
			{
				return this._perturbSeed;
			}
			private set
			{
				this._perturbSeed = value;
				this.ReSeedPerturbRng(0);
			}
		}

		public void GetPlayerFrames(out MatrixFrame initialFrame, out MatrixFrame targetFrame, float perturbAmount = 0f)
		{
			this.ReSeedPerturbRng(0);
			Vec3 vec;
			this.ComputePerturbedSpawnOffset(perturbAmount, out vec);
			float num = 3.1415927f;
			float innerRadius = this.InnerRadius;
			Vec3 vec2 = vec - this.WalkDistance * Vec3.Forward;
			this.ComputeSpawnWorldFrame(num, innerRadius, vec2, out initialFrame);
			this.ComputeSpawnWorldFrame(3.1415927f, this.InnerRadius, vec, out targetFrame);
		}

		public void GetBossFrames(out MatrixFrame initialFrame, out MatrixFrame targetFrame, float perturbAmount = 0f)
		{
			this.ReSeedPerturbRng(1);
			Vec3 vec;
			this.ComputePerturbedSpawnOffset(perturbAmount, out vec);
			float num = 0f;
			float innerRadius = this.InnerRadius;
			Vec3 vec2 = vec + this.WalkDistance * Vec3.Forward;
			this.ComputeSpawnWorldFrame(num, innerRadius, vec2, out initialFrame);
			this.ComputeSpawnWorldFrame(0f, this.InnerRadius, vec, out targetFrame);
		}

		public void GetAllyFrames(out List<MatrixFrame> initialFrames, out List<MatrixFrame> targetFrames, int agentCount = 10, float agentOffsetAngle = 0.15707964f, float perturbAmount = 0f)
		{
			this.ReSeedPerturbRng(2);
			initialFrames = this.ComputeSpawnWorldFrames(agentCount, this.OuterRadius, -this.WalkDistance * Vec3.Forward, 3.1415927f, agentOffsetAngle, perturbAmount).ToList<MatrixFrame>();
			this.ReSeedPerturbRng(2);
			targetFrames = this.ComputeSpawnWorldFrames(agentCount, this.OuterRadius, Vec3.Zero, 3.1415927f, agentOffsetAngle, perturbAmount).ToList<MatrixFrame>();
		}

		public void GetBanditFrames(out List<MatrixFrame> initialFrames, out List<MatrixFrame> targetFrames, int agentCount = 10, float agentOffsetAngle = 0.15707964f, float perturbAmount = 0f)
		{
			this.ReSeedPerturbRng(3);
			initialFrames = this.ComputeSpawnWorldFrames(agentCount, this.OuterRadius, this.WalkDistance * Vec3.Forward, 0f, agentOffsetAngle, perturbAmount).ToList<MatrixFrame>();
			this.ReSeedPerturbRng(3);
			targetFrames = this.ComputeSpawnWorldFrames(agentCount, this.OuterRadius, Vec3.Zero, 0f, agentOffsetAngle, perturbAmount).ToList<MatrixFrame>();
		}

		public void GetAllyInitialFormationFrame(out MatrixFrame frame)
		{
			float num = 3.1415927f;
			float outerRadius = this.OuterRadius;
			Vec3 vec = -this.WalkDistance * Vec3.Forward;
			this.ComputeSpawnWorldFrame(num, outerRadius, vec, out frame);
		}

		public void GetBanditInitialFormationFrame(out MatrixFrame frame)
		{
			float num = 0f;
			float outerRadius = this.OuterRadius;
			Vec3 vec = this.WalkDistance * Vec3.Forward;
			this.ComputeSpawnWorldFrame(num, outerRadius, vec, out frame);
		}

		public bool IsWorldPointInsideCameraVolume(in Vec3 worldPoint)
		{
			Vec3 vec = base.GameEntity.GetGlobalFrame().TransformToLocal(worldPoint);
			return this.IsLocalPointInsideCameraVolume(vec);
		}

		public bool ClampWorldPointToCameraVolume(in Vec3 worldPoint, out Vec3 clampedPoint)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 vec = globalFrame.TransformToLocal(worldPoint);
			bool flag = this.IsLocalPointInsideCameraVolume(vec);
			if (flag)
			{
				clampedPoint = worldPoint;
				return flag;
			}
			float num = 5f;
			float num2 = this.OuterRadius + this.WalkDistance;
			vec.x = MathF.Clamp(vec.x, -num, num);
			vec.y = MathF.Clamp(vec.y, -num2, num2);
			vec.z = MathF.Clamp(vec.z, 0f, 5f);
			clampedPoint = globalFrame.TransformToParent(vec);
			return flag;
		}

		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "ShowPreview")
			{
				this.UpdatePreview();
				this.TogglePreviewVisibility(this.ShowPreview);
				return;
			}
			if (this.ShowPreview && (variableName == "InnerRadius" || variableName == "OuterRadius" || variableName == "WalkDistance"))
			{
				this.UpdatePreview();
			}
		}

		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (this.ShowPreview)
			{
				MatrixFrame frame = base.GameEntity.GetFrame();
				if (!this._previousEntityFrame.origin.NearlyEquals(frame.origin, 1E-05f) || !this._previousEntityFrame.rotation.NearlyEquals(frame.rotation, 1E-05f))
				{
					this._previousEntityFrame = frame;
					this.UpdatePreview();
				}
			}
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this.RemovePreview();
		}

		private void UpdatePreview()
		{
			if (this._previewEntities == null)
			{
				this.GeneratePreview();
			}
			GameEntity previewEntities = this._previewEntities;
			MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
			previewEntities.SetGlobalFrame(ref matrixFrame);
			MatrixFrame identity = MatrixFrame.Identity;
			MatrixFrame identity2 = MatrixFrame.Identity;
			this.GetPlayerFrames(out identity, out identity2, 0.25f);
			this._previewPlayer.InitialEntity.SetGlobalFrame(ref identity);
			this._previewPlayer.TargetEntity.SetGlobalFrame(ref identity2);
			List<MatrixFrame> list;
			List<MatrixFrame> list2;
			this.GetAllyFrames(out list, out list2, 10, 0.15707964f, 0.25f);
			int num = 0;
			foreach (HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo hideoutBossFightPreviewEntityInfo in this._previewAllies)
			{
				GameEntity initialEntity = hideoutBossFightPreviewEntityInfo.InitialEntity;
				matrixFrame = list[num];
				initialEntity.SetGlobalFrame(ref matrixFrame);
				GameEntity targetEntity = hideoutBossFightPreviewEntityInfo.TargetEntity;
				matrixFrame = list2[num];
				targetEntity.SetGlobalFrame(ref matrixFrame);
				num++;
			}
			this.GetBossFrames(out identity, out identity2, 0.25f);
			this._previewBoss.InitialEntity.SetGlobalFrame(ref identity);
			this._previewBoss.TargetEntity.SetGlobalFrame(ref identity2);
			List<MatrixFrame> list3;
			List<MatrixFrame> list4;
			this.GetBanditFrames(out list3, out list4, 10, 0.15707964f, 0.25f);
			int num2 = 0;
			foreach (HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo hideoutBossFightPreviewEntityInfo2 in this._previewBandits)
			{
				GameEntity initialEntity2 = hideoutBossFightPreviewEntityInfo2.InitialEntity;
				matrixFrame = list3[num2];
				initialEntity2.SetGlobalFrame(ref matrixFrame);
				GameEntity targetEntity2 = hideoutBossFightPreviewEntityInfo2.TargetEntity;
				matrixFrame = list4[num2];
				targetEntity2.SetGlobalFrame(ref matrixFrame);
				num2++;
			}
			MatrixFrame frame = this._previewCamera.GetFrame();
			Vec3 scaleVector = frame.rotation.GetScaleVector();
			Vec3 vec = Vec3.Forward * (this.OuterRadius + this.WalkDistance) + Vec3.Side * 5f + Vec3.Up * 5f;
			Vec3 vec2;
			vec2..ctor(vec.x / scaleVector.x, vec.y / scaleVector.y, vec.z / scaleVector.z, -1f);
			frame.rotation.ApplyScaleLocal(vec2);
			this._previewCamera.SetFrame(ref frame);
		}

		private void GeneratePreview()
		{
			Scene scene = base.GameEntity.Scene;
			this._previewEntities = GameEntity.CreateEmpty(scene, false);
			this._previewEntities.EntityFlags |= 131072;
			MatrixFrame identity = MatrixFrame.Identity;
			this._previewEntities.SetFrame(ref identity);
			MatrixFrame globalFrame = this._previewEntities.GetGlobalFrame();
			GameEntity gameEntity = GameEntity.Instantiate(scene, "hideout_boss_fight_preview_boss", globalFrame);
			this._previewEntities.AddChild(gameEntity, false);
			GameEntity gameEntity2;
			GameEntity gameEntity3;
			this.ReadPrefabEntity(gameEntity, out gameEntity2, out gameEntity3);
			this._previewBoss = new HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo(gameEntity, gameEntity2, gameEntity3);
			GameEntity gameEntity4 = GameEntity.Instantiate(scene, "hideout_boss_fight_preview_player", globalFrame);
			this._previewEntities.AddChild(gameEntity4, false);
			GameEntity gameEntity5;
			GameEntity gameEntity6;
			this.ReadPrefabEntity(gameEntity4, out gameEntity5, out gameEntity6);
			this._previewPlayer = new HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo(gameEntity4, gameEntity5, gameEntity6);
			for (int i = 0; i < 10; i++)
			{
				GameEntity gameEntity7 = GameEntity.Instantiate(scene, "hideout_boss_fight_preview_ally", globalFrame);
				this._previewEntities.AddChild(gameEntity7, false);
				GameEntity gameEntity8;
				GameEntity gameEntity9;
				this.ReadPrefabEntity(gameEntity7, out gameEntity8, out gameEntity9);
				this._previewAllies.Add(new HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo(gameEntity7, gameEntity8, gameEntity9));
			}
			for (int j = 0; j < 10; j++)
			{
				GameEntity gameEntity10 = GameEntity.Instantiate(scene, "hideout_boss_fight_preview_bandit", globalFrame);
				this._previewEntities.AddChild(gameEntity10, false);
				GameEntity gameEntity11;
				GameEntity gameEntity12;
				this.ReadPrefabEntity(gameEntity10, out gameEntity11, out gameEntity12);
				this._previewBandits.Add(new HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo(gameEntity10, gameEntity11, gameEntity12));
			}
			this._previewCamera = GameEntity.Instantiate(scene, "hideout_boss_fight_camera_preview", globalFrame);
			this._previewEntities.AddChild(this._previewCamera, false);
		}

		private void RemovePreview()
		{
			if (this._previewEntities != null)
			{
				this._previewEntities.Remove(90);
			}
		}

		private void TogglePreviewVisibility(bool value)
		{
			if (this._previewEntities != null)
			{
				this._previewEntities.SetVisibilityExcludeParents(value);
			}
		}

		private void ReadPrefabEntity(GameEntity entity, out GameEntity initialEntity, out GameEntity targetEntity)
		{
			GameEntity firstChildEntityWithTag = MBExtensions.GetFirstChildEntityWithTag(entity, "initial_frame");
			if (firstChildEntityWithTag == null)
			{
				Debug.FailedAssert("Prefab entity " + entity.Name + " is not a spawn prefab with an initial frame entity", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Objects\\Cinematics\\HideoutBossFightBehavior.cs", "ReadPrefabEntity", 389);
			}
			GameEntity firstChildEntityWithTag2 = MBExtensions.GetFirstChildEntityWithTag(entity, "target_frame");
			if (firstChildEntityWithTag2 == null)
			{
				Debug.FailedAssert("Prefab entity " + entity.Name + " is not a spawn prefab with an target frame entity", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Objects\\Cinematics\\HideoutBossFightBehavior.cs", "ReadPrefabEntity", 395);
			}
			initialEntity = firstChildEntityWithTag;
			targetEntity = firstChildEntityWithTag2;
		}

		private void FindRadialPlacementFrame(float angle, float radius, out MatrixFrame frame)
		{
			float num;
			float num2;
			MathF.SinCos(angle, ref num, ref num2);
			Vec3 vec = num2 * Vec3.Forward + num * Vec3.Side;
			Vec3 vec2 = radius * vec;
			Vec3 vec3 = ((num2 > 0f) ? (-1f) : 1f) * Vec3.Forward;
			Mat3 mat = Mat3.CreateMat3WithForward(ref vec3);
			frame = new MatrixFrame(mat, vec2);
		}

		private void SnapOnClosestCollider(ref MatrixFrame frameWs)
		{
			Scene scene = base.GameEntity.Scene;
			Vec3 origin = frameWs.origin;
			origin.z += 5f;
			Vec3 vec = origin;
			float num = 500f;
			vec.z -= num;
			float num2;
			if (scene.RayCastForClosestEntityOrTerrain(origin, vec, ref num2, 0.01f, 79617))
			{
				frameWs.origin.z = origin.z - num2;
			}
		}

		private void ReSeedPerturbRng(int seedOffset = 0)
		{
			this._perturbRng = new Random(this._perturbSeed + seedOffset);
		}

		private void ComputeSpawnWorldFrame(float localAngle, float localRadius, in Vec3 localOffset, out MatrixFrame worldFrame)
		{
			MatrixFrame matrixFrame;
			this.FindRadialPlacementFrame(localAngle, localRadius, out matrixFrame);
			matrixFrame.origin += localOffset;
			worldFrame = base.GameEntity.GetGlobalFrame().TransformToParent(matrixFrame);
			this.SnapOnClosestCollider(ref worldFrame);
		}

		private IEnumerable<MatrixFrame> ComputeSpawnWorldFrames(int spawnCount, float localRadius, Vec3 localOffset, float localBaseAngle, float localOffsetAngle, float localPerturbAmount = 0f)
		{
			float[] localPlacementAngles = new float[]
			{
				localBaseAngle + localOffsetAngle / 2f,
				localBaseAngle - localOffsetAngle / 2f
			};
			int angleIndex = 0;
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 zero = Vec3.Zero;
			int num2;
			for (int i = 0; i < spawnCount; i = num2 + 1)
			{
				this.ComputePerturbedSpawnOffset(localPerturbAmount, out zero);
				float num = localPlacementAngles[angleIndex];
				Vec3 vec = zero + localOffset;
				this.ComputeSpawnWorldFrame(num, localRadius, vec, out identity);
				yield return identity;
				localPlacementAngles[angleIndex] += (float)((angleIndex == 0) ? 1 : (-1)) * localOffsetAngle;
				angleIndex = (angleIndex + 1) % 2;
				num2 = i;
			}
			yield break;
		}

		private void ComputePerturbedSpawnOffset(float perturbAmount, out Vec3 perturbVector)
		{
			perturbVector = Vec3.Zero;
			perturbAmount = MathF.Abs(perturbAmount);
			if (perturbAmount > 1E-05f)
			{
				float num;
				float num2;
				MathF.SinCos(6.2831855f * Extensions.NextFloat(this._perturbRng), ref num, ref num2);
				perturbVector.x = perturbAmount * num2;
				perturbVector.y = perturbAmount * num;
			}
		}

		private bool IsLocalPointInsideCameraVolume(in Vec3 localPoint)
		{
			float num = 5f;
			float num2 = this.OuterRadius + this.WalkDistance;
			return localPoint.x >= -num && localPoint.x <= num && localPoint.y >= -num2 && localPoint.y <= num2 && localPoint.z >= 0f && localPoint.z <= 5f;
		}

		private const int PreviewPerturbSeed = 0;

		private const float PreviewPerturbAmount = 0.25f;

		private const int PreviewTroopCount = 10;

		private const float PreviewPlacementAngle = 0.15707964f;

		private const string InitialFrameTag = "initial_frame";

		private const string TargetFrameTag = "target_frame";

		private const string BossPreviewPrefab = "hideout_boss_fight_preview_boss";

		private const string PlayerPreviewPrefab = "hideout_boss_fight_preview_player";

		private const string AllyPreviewPrefab = "hideout_boss_fight_preview_ally";

		private const string BanditPreviewPrefab = "hideout_boss_fight_preview_bandit";

		private const string PreviewCameraPrefab = "hideout_boss_fight_camera_preview";

		public const float MaxCameraHeight = 5f;

		public const float MaxCameraWidth = 10f;

		public float InnerRadius = 2.5f;

		public float OuterRadius = 6f;

		public float WalkDistance = 3f;

		public bool ShowPreview;

		private int _perturbSeed;

		private Random _perturbRng = new Random(0);

		private MatrixFrame _previousEntityFrame = MatrixFrame.Identity;

		private GameEntity _previewEntities;

		private List<HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo> _previewAllies = new List<HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo>();

		private List<HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo> _previewBandits = new List<HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo>();

		private HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo _previewBoss = HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo.Invalid;

		private HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo _previewPlayer = HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo.Invalid;

		private GameEntity _previewCamera;

		private readonly struct HideoutBossFightPreviewEntityInfo
		{
			public static HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo Invalid
			{
				get
				{
					return new HideoutBossFightBehavior.HideoutBossFightPreviewEntityInfo(null, null, null);
				}
			}

			public bool IsValid
			{
				get
				{
					return this.BaseEntity == null;
				}
			}

			public HideoutBossFightPreviewEntityInfo(GameEntity baseEntity, GameEntity initialEntity, GameEntity targetEntity)
			{
				this.BaseEntity = baseEntity;
				this.InitialEntity = initialEntity;
				this.TargetEntity = targetEntity;
			}

			public readonly GameEntity BaseEntity;

			public readonly GameEntity InitialEntity;

			public readonly GameEntity TargetEntity;
		}

		private enum HideoutSeedPerturbOffset
		{
			Player,
			Boss,
			Ally,
			Bandit
		}
	}
}
