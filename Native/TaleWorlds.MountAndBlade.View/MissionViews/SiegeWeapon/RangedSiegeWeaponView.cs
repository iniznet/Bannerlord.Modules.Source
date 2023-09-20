using System;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon
{
	public class RangedSiegeWeaponView : UsableMissionObjectComponent
	{
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		public MissionScreen MissionScreen { get; private set; }

		public Camera Camera { get; private set; }

		public GameEntity CameraHolder
		{
			get
			{
				return this.RangedSiegeWeapon.cameraHolder;
			}
		}

		public Agent PilotAgent
		{
			get
			{
				return this.RangedSiegeWeapon.PilotAgent;
			}
		}

		internal void Initialize(RangedSiegeWeapon rangedSiegeWeapon, MissionScreen missionScreen)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.MissionScreen = missionScreen;
		}

		protected override void OnAdded(Scene scene)
		{
			base.OnAdded(scene);
			if (this.CameraHolder != null)
			{
				this.CreateCamera();
			}
		}

		protected override void OnMissionReset()
		{
			base.OnMissionReset();
			if (this.CameraHolder != null)
			{
				this._cameraYaw = this._cameraInitialYaw;
				this._cameraPitch = this._cameraInitialPitch;
				this.ApplyCameraRotation();
				this._isInWeaponCameraMode = false;
				this.ResetCamera();
			}
		}

		public override bool IsOnTickRequired()
		{
			return true;
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!GameNetwork.IsReplay)
			{
				this.HandleUserInput(dt);
			}
		}

		protected virtual void HandleUserInput(float dt)
		{
			if (this.PilotAgent != null && this.PilotAgent.IsMainAgent && this.CameraHolder != null)
			{
				if (!this._isInWeaponCameraMode)
				{
					this._isInWeaponCameraMode = true;
					this.StartUsingWeaponCamera();
				}
				this.HandleUserCameraRotation(dt);
			}
			if (this._isInWeaponCameraMode && (this.PilotAgent == null || !this.PilotAgent.IsMainAgent))
			{
				this._isInWeaponCameraMode = false;
				this.ResetCamera();
			}
			this.HandleUserAiming(dt);
		}

		private void CreateCamera()
		{
			this.Camera = Camera.CreateCamera();
			float aspectRatio = Screen.AspectRatio;
			this.Camera.SetFovVertical(1.0471976f, aspectRatio, 0.1f, 1000f);
			this.Camera.Entity = this.CameraHolder;
			MatrixFrame frame = this.CameraHolder.GetFrame();
			Vec3 eulerAngles = frame.rotation.GetEulerAngles();
			this._cameraYaw = eulerAngles.z;
			this._cameraPitch = eulerAngles.x;
			this._cameraRoll = eulerAngles.y;
			this._cameraPositionOffset = frame.origin;
			this._cameraPositionOffset.RotateAboutZ(-this._cameraYaw);
			this._cameraPositionOffset.RotateAboutX(-this._cameraPitch);
			this._cameraPositionOffset.RotateAboutY(-this._cameraRoll);
			this._cameraInitialYaw = this._cameraYaw;
			this._cameraInitialPitch = this._cameraPitch;
		}

		protected virtual void StartUsingWeaponCamera()
		{
			if (this.CameraHolder != null && this.Camera.Entity != null)
			{
				this.MissionScreen.CustomCamera = this.Camera;
				Agent.Main.IsLookDirectionLocked = true;
				return;
			}
			Debug.FailedAssert("Camera entities are null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\MissionViews\\SiegeWeapon\\RangedSiegeWeaponView.cs", "StartUsingWeaponCamera", 140);
		}

		private void ResetCamera()
		{
			if (this.MissionScreen.CustomCamera == this.Camera)
			{
				this.MissionScreen.CustomCamera = null;
				if (Agent.Main != null)
				{
					Agent.Main.IsLookDirectionLocked = false;
					this.MissionScreen.SetExtraCameraParameters(false, 0f);
				}
			}
		}

		protected virtual void HandleUserCameraRotation(float dt)
		{
			float cameraYaw = this._cameraYaw;
			float cameraPitch = this._cameraPitch;
			if (this.MissionScreen.SceneLayer.Input.IsGameKeyDown(10))
			{
				this._cameraYaw = this._cameraInitialYaw;
				this._cameraPitch = this._cameraInitialPitch;
			}
			this._cameraYaw += this.MissionScreen.SceneLayer.Input.GetMouseMoveX() * dt * 0.2f;
			this._cameraPitch += this.MissionScreen.SceneLayer.Input.GetMouseMoveY() * dt * 0.2f;
			this._cameraYaw = MBMath.ClampFloat(this._cameraYaw, 1.5707964f, 4.712389f);
			this._cameraPitch = MBMath.ClampFloat(this._cameraPitch, 1.0471976f, 1.7453294f);
			if (cameraPitch != this._cameraPitch || cameraYaw != this._cameraYaw)
			{
				this.ApplyCameraRotation();
			}
		}

		private void ApplyCameraRotation()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.RotateAboutUp(this._cameraYaw);
			identity.rotation.RotateAboutSide(this._cameraPitch);
			identity.rotation.RotateAboutForward(this._cameraRoll);
			identity.Strafe(this._cameraPositionOffset.x);
			identity.Advance(this._cameraPositionOffset.y);
			identity.Elevate(this._cameraPositionOffset.z);
			this.CameraHolder.SetFrame(ref identity);
		}

		private void HandleUserAiming(float dt)
		{
			bool flag = false;
			float num = 0f;
			float num2 = 0f;
			if (this.PilotAgent != null && this.PilotAgent.IsMainAgent)
			{
				if (this.UsesMouseForAiming)
				{
					InputContext input = this.MissionScreen.SceneLayer.Input;
					float num3 = dt / 0.0006f;
					float num4 = input.GetMouseMoveX() + num3 * input.GetGameKeyAxis("CameraAxisX");
					float num5 = input.GetMouseMoveY() + -num3 * input.GetGameKeyAxis("CameraAxisY");
					if (NativeConfig.InvertMouse)
					{
						num5 *= -1f;
					}
					Vec2 vec;
					vec..ctor(-num4, -num5);
					if (vec.IsNonZero())
					{
						float num6 = vec.Normalize();
						num6 = MathF.Min(5f, MathF.Pow(num6, 1.5f) * 0.025f);
						vec *= num6;
						num = vec.x;
						num2 = vec.y;
					}
				}
				else
				{
					if (this.MissionScreen.SceneLayer.Input.IsGameKeyDown(2))
					{
						num = 1f;
					}
					else if (this.MissionScreen.SceneLayer.Input.IsGameKeyDown(3))
					{
						num = -1f;
					}
					if (this.MissionScreen.SceneLayer.Input.IsGameKeyDown(0))
					{
						num2 = 1f;
					}
					else if (this.MissionScreen.SceneLayer.Input.IsGameKeyDown(1))
					{
						num2 = -1f;
					}
				}
				if (num != 0f)
				{
					flag = true;
				}
				if (num2 != 0f)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.RangedSiegeWeapon.GiveInput(num, num2);
			}
		}

		private float _cameraYaw;

		private float _cameraPitch;

		private float _cameraRoll;

		private float _cameraInitialYaw;

		private float _cameraInitialPitch;

		private Vec3 _cameraPositionOffset;

		private bool _isInWeaponCameraMode;

		protected bool UsesMouseForAiming;
	}
}
