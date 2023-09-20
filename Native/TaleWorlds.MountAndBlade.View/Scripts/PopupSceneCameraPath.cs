using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	public class PopupSceneCameraPath : ScriptComponentBehavior
	{
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		protected override void OnEditorInit()
		{
			base.OnEditorInit();
		}

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

		public void SetInitialState()
		{
			this.SetState(0);
		}

		public void SetPositiveState()
		{
			this.SetState(1);
		}

		public void SetNegativeState()
		{
			this.SetState(2);
		}

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

		public float GetCameraFade()
		{
			return this._cameraFadeValue;
		}

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

		private float InQuadBlend(float t)
		{
			return t * t;
		}

		private float OutQuadBlend(float t)
		{
			return t * (2f - t);
		}

		private float InOutQuadBlend(float t)
		{
			if (t >= 0.5f)
			{
				return -1f + (4f - 2f * t) * t;
			}
			return 2f * t * t;
		}

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

		private float SmoothStep(float edge0, float edge1, float x)
		{
			x = this.Clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
			return x * x * (3f - 2f * x);
		}

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

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			this.UpdateCamera(dt, ref this._transitionState[this._currentState]);
		}

		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.OnTick(dt);
		}

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

		public string LookAtEntity = "";

		public string SkeletonName = "";

		public int BoneIndex;

		public Vec3 AttachmentOffset = new Vec3(0f, 0f, 0f, -1f);

		public string InitialPath = "";

		public string InitialAnimationClip = "";

		public string InitialSound = "event:/mission/siege/siegetower/doorland";

		public float InitialPathStartTime;

		public float InitialPathDuration = 1f;

		public PopupSceneCameraPath.InterpolationType InitialInterpolation;

		public bool InitialFadeOut;

		public string PositivePath = "";

		public string PositiveAnimationClip = "";

		public string PositiveSound = "";

		public float PositivePathStartTime;

		public float PositivePathDuration = 1f;

		public PopupSceneCameraPath.InterpolationType PositiveInterpolation;

		public bool PositiveFadeOut;

		public string NegativePath = "";

		public string NegativeAnimationClip = "";

		public string NegativeSound = "";

		public float NegativePathStartTime;

		public float NegativePathDuration = 1f;

		public PopupSceneCameraPath.InterpolationType NegativeInterpolation;

		public bool NegativeFadeOut;

		private bool _isReady;

		public SimpleButton TestInitial;

		public SimpleButton TestPositive;

		public SimpleButton TestNegative;

		private MatrixFrame _localFrameIdentity = MatrixFrame.Identity;

		private GameEntity _lookAtEntity;

		private int _currentState;

		private float _cameraFadeValue;

		private List<PopupSceneSkeletonAnimationScript> _skeletonAnims = new List<PopupSceneSkeletonAnimationScript>();

		private SoundEvent _activeSoundEvent;

		private readonly PopupSceneCameraPath.PathAnimationState[] _transitionState = new PopupSceneCameraPath.PathAnimationState[3];

		public enum InterpolationType
		{
			Linear,
			EaseIn,
			EaseOut,
			EaseInOut
		}

		public struct PathAnimationState
		{
			public Path path;

			public string animationName;

			public float totalDistance;

			public float startTime;

			public float duration;

			public float alpha;

			public float easedAlpha;

			public bool fadeCamera;

			public PopupSceneCameraPath.InterpolationType interpolation;

			public string soundEvent;
		}
	}
}
