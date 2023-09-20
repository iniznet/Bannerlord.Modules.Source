using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x02000037 RID: 55
	public class PopupSceneCameraPath : ScriptComponentBehavior
	{
		// Token: 0x06000280 RID: 640 RVA: 0x0001709D File Offset: 0x0001529D
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000170B1 File Offset: 0x000152B1
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000170BC File Offset: 0x000152BC
		public void Initialize()
		{
			if (this.SkeletonName != "" && (base.GameEntity.Skeleton == null || base.GameEntity.Skeleton.GetName() != this.SkeletonName))
			{
				GameEntityExtensions.CreateSimpleSkeleton(base.GameEntity, this.SkeletonName);
			}
			else if (this.SkeletonName == "" && base.GameEntity.Skeleton != null)
			{
				base.GameEntity.RemoveSkeleton();
			}
			if (this.LookAtEntity != "")
			{
				this._lookAtEntity = base.GameEntity.Scene.GetFirstEntityWithName(this.LookAtEntity);
			}
			this._transitionState[0].path = ((this.InitialPath == "") ? null : base.GameEntity.Scene.GetPathWithName(this.InitialPath));
			this._transitionState[0].animationName = this.InitialAnimationClip;
			this._transitionState[0].startTime = this.InitialPathStartTime;
			this._transitionState[0].duration = this.InitialPathDuration;
			this._transitionState[0].interpolation = this.InitialInterpolation;
			this._transitionState[0].fadeCamera = this.InitialFadeOut;
			this._transitionState[0].soundEvent = this.InitialSound;
			this._transitionState[1].path = ((this.PositivePath == "") ? null : base.GameEntity.Scene.GetPathWithName(this.PositivePath));
			this._transitionState[1].animationName = this.PositiveAnimationClip;
			this._transitionState[1].startTime = this.PositivePathStartTime;
			this._transitionState[1].duration = this.PositivePathDuration;
			this._transitionState[1].interpolation = this.PositiveInterpolation;
			this._transitionState[1].fadeCamera = this.PositiveFadeOut;
			this._transitionState[1].soundEvent = this.PositiveSound;
			this._transitionState[2].path = ((this.NegativePath == "") ? null : base.GameEntity.Scene.GetPathWithName(this.NegativePath));
			this._transitionState[2].animationName = this.NegativeAnimationClip;
			this._transitionState[2].startTime = this.NegativePathStartTime;
			this._transitionState[2].duration = this.NegativePathDuration;
			this._transitionState[2].interpolation = this.NegativeInterpolation;
			this._transitionState[2].fadeCamera = this.NegativeFadeOut;
			this._transitionState[2].soundEvent = this.NegativeSound;
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = base.GameEntity.GlobalPosition;
			SoundManager.SetListenerFrame(identity);
			List<GameEntity> list = new List<GameEntity>();
			base.Scene.GetAllEntitiesWithScriptComponent<PopupSceneSkeletonAnimationScript>(ref list);
			list.ForEach(delegate(GameEntity e)
			{
				this._skeletonAnims.Add(e.GetFirstScriptOfType<PopupSceneSkeletonAnimationScript>());
			});
			this._skeletonAnims.ForEach(delegate(PopupSceneSkeletonAnimationScript s)
			{
				s.Initialize();
			});
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0001743C File Offset: 0x0001563C
		private void SetState(int state)
		{
			if (base.GameEntity.Skeleton != null && !string.IsNullOrEmpty(this._transitionState[state].animationName))
			{
				MBSkeletonExtensions.SetAnimationAtChannel(base.GameEntity.Skeleton, this._transitionState[state].animationName, 0, 1f, -1f, 0f);
			}
			this._currentState = state;
			this._transitionState[state].alpha = 0f;
			if (this._transitionState[state].path != null)
			{
				this._transitionState[state].totalDistance = this._transitionState[state].path.GetTotalLength();
			}
			if (this._transitionState[state].soundEvent != "")
			{
				SoundEvent activeSoundEvent = this._activeSoundEvent;
				if (activeSoundEvent != null)
				{
					activeSoundEvent.Stop();
				}
				this._activeSoundEvent = SoundEvent.CreateEventFromString(this._transitionState[state].soundEvent, null);
				if (this._isReady)
				{
					SoundEvent activeSoundEvent2 = this._activeSoundEvent;
					if (activeSoundEvent2 != null)
					{
						activeSoundEvent2.Play();
					}
				}
			}
			this.UpdateCamera(0f, ref this._transitionState[state]);
			this._skeletonAnims.ForEach(delegate(PopupSceneSkeletonAnimationScript s)
			{
				s.SetState(state);
			});
		}

		// Token: 0x06000284 RID: 644 RVA: 0x000175D4 File Offset: 0x000157D4
		public void SetInitialState()
		{
			this.SetState(0);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000175DD File Offset: 0x000157DD
		public void SetPositiveState()
		{
			this.SetState(1);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000175E6 File Offset: 0x000157E6
		public void SetNegativeState()
		{
			this.SetState(2);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000175EF File Offset: 0x000157EF
		public void SetIsReady(bool isReady)
		{
			if (this._isReady != isReady)
			{
				if (isReady)
				{
					SoundEvent activeSoundEvent = this._activeSoundEvent;
					if (activeSoundEvent != null && !activeSoundEvent.IsPlaying())
					{
						this._activeSoundEvent.Play();
					}
				}
				this._isReady = isReady;
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00017627 File Offset: 0x00015827
		public float GetCameraFade()
		{
			return this._cameraFadeValue;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00017630 File Offset: 0x00015830
		public void Destroy()
		{
			SoundEvent activeSoundEvent = this._activeSoundEvent;
			if (activeSoundEvent != null)
			{
				activeSoundEvent.Stop();
			}
			for (int i = 0; i < 3; i++)
			{
				this._transitionState[i].path = null;
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0001766C File Offset: 0x0001586C
		private float InQuadBlend(float t)
		{
			return t * t;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00017671 File Offset: 0x00015871
		private float OutQuadBlend(float t)
		{
			return t * (2f - t);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0001767C File Offset: 0x0001587C
		private float InOutQuadBlend(float t)
		{
			if (t >= 0.5f)
			{
				return -1f + (4f - 2f * t) * t;
			}
			return 2f * t * t;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000176A8 File Offset: 0x000158A8
		private MatrixFrame CreateLookAt(Vec3 position, Vec3 target, Vec3 upVector)
		{
			Vec3 vec = target - position;
			vec.Normalize();
			Vec3 vec2 = Vec3.CrossProduct(vec, upVector);
			vec2.Normalize();
			Vec3 vec3 = Vec3.CrossProduct(vec2, vec);
			float x = vec2.x;
			float y = vec2.y;
			float z = vec2.z;
			float num = 0f;
			float x2 = vec3.x;
			float y2 = vec3.y;
			float z2 = vec3.z;
			float num2 = 0f;
			float num3 = -vec.x;
			float num4 = -vec.y;
			float num5 = -vec.z;
			float num6 = 0f;
			float x3 = position.x;
			float y3 = position.y;
			float z3 = position.z;
			float num7 = 1f;
			return new MatrixFrame(x, y, z, num, x2, y2, z2, num2, num3, num4, num5, num6, x3, y3, z3, num7);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0001777B File Offset: 0x0001597B
		private float Clamp(float x, float a, float b)
		{
			if (x < a)
			{
				return a;
			}
			if (x <= b)
			{
				return x;
			}
			return b;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0001778A File Offset: 0x0001598A
		private float SmoothStep(float edge0, float edge1, float x)
		{
			x = this.Clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
			return x * x * (3f - 2f * x);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x000177B8 File Offset: 0x000159B8
		private void UpdateCamera(float dt, ref PopupSceneCameraPath.PathAnimationState state)
		{
			GameEntity gameEntity = base.GameEntity.Scene.FindEntityWithTag("camera_instance");
			if (gameEntity == null)
			{
				return;
			}
			state.alpha += dt;
			if (state.alpha > state.startTime + state.duration)
			{
				state.alpha = state.startTime + state.duration;
			}
			float num = this.SmoothStep(state.startTime, state.startTime + state.duration, state.alpha);
			switch (state.interpolation)
			{
			case PopupSceneCameraPath.InterpolationType.EaseIn:
				num = this.InQuadBlend(num);
				break;
			case PopupSceneCameraPath.InterpolationType.EaseOut:
				num = this.OutQuadBlend(num);
				break;
			case PopupSceneCameraPath.InterpolationType.EaseInOut:
				num = this.InOutQuadBlend(num);
				break;
			}
			state.easedAlpha = num;
			if (state.fadeCamera)
			{
				this._cameraFadeValue = num;
			}
			if (base.GameEntity.Skeleton != null && !string.IsNullOrEmpty(state.animationName))
			{
				MatrixFrame matrixFrame = base.GameEntity.Skeleton.GetBoneEntitialFrame((sbyte)this.BoneIndex);
				matrixFrame = this._localFrameIdentity.TransformToParent(matrixFrame);
				MatrixFrame matrixFrame2 = default(MatrixFrame);
				matrixFrame2.rotation = matrixFrame.rotation;
				matrixFrame2.rotation.u = -matrixFrame.rotation.s;
				matrixFrame2.rotation.f = -matrixFrame.rotation.u;
				matrixFrame2.rotation.s = matrixFrame.rotation.f;
				matrixFrame2.origin = matrixFrame.origin + this.AttachmentOffset;
				gameEntity.SetFrame(ref matrixFrame2);
				SoundManager.SetListenerFrame(matrixFrame2);
				return;
			}
			if (state.path != null)
			{
				float num2 = num * state.totalDistance;
				Vec3 origin = state.path.GetFrameForDistance(num2).origin;
				MatrixFrame matrixFrame3 = MatrixFrame.Identity;
				if (this._lookAtEntity != null)
				{
					matrixFrame3 = this.CreateLookAt(origin, this._lookAtEntity.GetGlobalFrame().origin, Vec3.Up);
				}
				else
				{
					matrixFrame3.origin = origin;
				}
				gameEntity.SetGlobalFrame(ref matrixFrame3);
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x000179D3 File Offset: 0x00015BD3
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x000179DD File Offset: 0x00015BDD
		protected override void OnTick(float dt)
		{
			this.UpdateCamera(dt, ref this._transitionState[this._currentState]);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000179F7 File Offset: 0x00015BF7
		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.OnTick(dt);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00017A08 File Offset: 0x00015C08
		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			this.Initialize();
			if (variableName == "TestInitial")
			{
				this.SetState(0);
			}
			if (variableName == "TestPositive")
			{
				this.SetState(1);
			}
			if (variableName == "TestNegative")
			{
				this.SetState(2);
			}
		}

		// Token: 0x04000197 RID: 407
		public string LookAtEntity = "";

		// Token: 0x04000198 RID: 408
		public string SkeletonName = "";

		// Token: 0x04000199 RID: 409
		public int BoneIndex;

		// Token: 0x0400019A RID: 410
		public Vec3 AttachmentOffset = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x0400019B RID: 411
		public string InitialPath = "";

		// Token: 0x0400019C RID: 412
		public string InitialAnimationClip = "";

		// Token: 0x0400019D RID: 413
		public string InitialSound = "event:/mission/siege/siegetower/doorland";

		// Token: 0x0400019E RID: 414
		public float InitialPathStartTime;

		// Token: 0x0400019F RID: 415
		public float InitialPathDuration = 1f;

		// Token: 0x040001A0 RID: 416
		public PopupSceneCameraPath.InterpolationType InitialInterpolation;

		// Token: 0x040001A1 RID: 417
		public bool InitialFadeOut;

		// Token: 0x040001A2 RID: 418
		public string PositivePath = "";

		// Token: 0x040001A3 RID: 419
		public string PositiveAnimationClip = "";

		// Token: 0x040001A4 RID: 420
		public string PositiveSound = "";

		// Token: 0x040001A5 RID: 421
		public float PositivePathStartTime;

		// Token: 0x040001A6 RID: 422
		public float PositivePathDuration = 1f;

		// Token: 0x040001A7 RID: 423
		public PopupSceneCameraPath.InterpolationType PositiveInterpolation;

		// Token: 0x040001A8 RID: 424
		public bool PositiveFadeOut;

		// Token: 0x040001A9 RID: 425
		public string NegativePath = "";

		// Token: 0x040001AA RID: 426
		public string NegativeAnimationClip = "";

		// Token: 0x040001AB RID: 427
		public string NegativeSound = "";

		// Token: 0x040001AC RID: 428
		public float NegativePathStartTime;

		// Token: 0x040001AD RID: 429
		public float NegativePathDuration = 1f;

		// Token: 0x040001AE RID: 430
		public PopupSceneCameraPath.InterpolationType NegativeInterpolation;

		// Token: 0x040001AF RID: 431
		public bool NegativeFadeOut;

		// Token: 0x040001B0 RID: 432
		private bool _isReady;

		// Token: 0x040001B1 RID: 433
		public SimpleButton TestInitial;

		// Token: 0x040001B2 RID: 434
		public SimpleButton TestPositive;

		// Token: 0x040001B3 RID: 435
		public SimpleButton TestNegative;

		// Token: 0x040001B4 RID: 436
		private MatrixFrame _localFrameIdentity = MatrixFrame.Identity;

		// Token: 0x040001B5 RID: 437
		private GameEntity _lookAtEntity;

		// Token: 0x040001B6 RID: 438
		private int _currentState;

		// Token: 0x040001B7 RID: 439
		private float _cameraFadeValue;

		// Token: 0x040001B8 RID: 440
		private List<PopupSceneSkeletonAnimationScript> _skeletonAnims = new List<PopupSceneSkeletonAnimationScript>();

		// Token: 0x040001B9 RID: 441
		private SoundEvent _activeSoundEvent;

		// Token: 0x040001BA RID: 442
		private readonly PopupSceneCameraPath.PathAnimationState[] _transitionState = new PopupSceneCameraPath.PathAnimationState[3];

		// Token: 0x020000AF RID: 175
		public enum InterpolationType
		{
			// Token: 0x04000341 RID: 833
			Linear,
			// Token: 0x04000342 RID: 834
			EaseIn,
			// Token: 0x04000343 RID: 835
			EaseOut,
			// Token: 0x04000344 RID: 836
			EaseInOut
		}

		// Token: 0x020000B0 RID: 176
		public struct PathAnimationState
		{
			// Token: 0x04000345 RID: 837
			public Path path;

			// Token: 0x04000346 RID: 838
			public string animationName;

			// Token: 0x04000347 RID: 839
			public float totalDistance;

			// Token: 0x04000348 RID: 840
			public float startTime;

			// Token: 0x04000349 RID: 841
			public float duration;

			// Token: 0x0400034A RID: 842
			public float alpha;

			// Token: 0x0400034B RID: 843
			public float easedAlpha;

			// Token: 0x0400034C RID: 844
			public bool fadeCamera;

			// Token: 0x0400034D RID: 845
			public PopupSceneCameraPath.InterpolationType interpolation;

			// Token: 0x0400034E RID: 846
			public string soundEvent;
		}
	}
}
