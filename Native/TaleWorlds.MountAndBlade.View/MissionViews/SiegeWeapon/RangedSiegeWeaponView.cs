using System;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon
{
	// Token: 0x0200007D RID: 125
	public class RangedSiegeWeaponView : UsableMissionObjectComponent
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x00023742 File Offset: 0x00021942
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x0002374A File Offset: 0x0002194A
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00023753 File Offset: 0x00021953
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x0002375B File Offset: 0x0002195B
		public MissionScreen MissionScreen { get; private set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x00023764 File Offset: 0x00021964
		// (set) Token: 0x06000499 RID: 1177 RVA: 0x0002376C File Offset: 0x0002196C
		public Camera Camera { get; private set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x00023775 File Offset: 0x00021975
		public GameEntity CameraHolder
		{
			get
			{
				return this.RangedSiegeWeapon.cameraHolder;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x00023782 File Offset: 0x00021982
		public Agent PilotAgent
		{
			get
			{
				return this.RangedSiegeWeapon.PilotAgent;
			}
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0002378F File Offset: 0x0002198F
		internal void Initialize(RangedSiegeWeapon rangedSiegeWeapon, MissionScreen missionScreen)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.MissionScreen = missionScreen;
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0002379F File Offset: 0x0002199F
		protected override void OnAdded(Scene scene)
		{
			base.OnAdded(scene);
			if (this.CameraHolder != null)
			{
				this.CreateCamera();
			}
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x000237BC File Offset: 0x000219BC
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

		// Token: 0x0600049F RID: 1183 RVA: 0x00023808 File Offset: 0x00021A08
		public override bool IsOnTickRequired()
		{
			return true;
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0002380B File Offset: 0x00021A0B
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!GameNetwork.IsReplay)
			{
				this.HandleUserInput(dt);
			}
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x00023824 File Offset: 0x00021A24
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

		// Token: 0x060004A2 RID: 1186 RVA: 0x000238A4 File Offset: 0x00021AA4
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

		// Token: 0x060004A3 RID: 1187 RVA: 0x00023988 File Offset: 0x00021B88
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

		// Token: 0x060004A4 RID: 1188 RVA: 0x000239EC File Offset: 0x00021BEC
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

		// Token: 0x060004A5 RID: 1189 RVA: 0x00023A40 File Offset: 0x00021C40
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

		// Token: 0x060004A6 RID: 1190 RVA: 0x00023B2C File Offset: 0x00021D2C
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

		// Token: 0x060004A7 RID: 1191 RVA: 0x00023BBC File Offset: 0x00021DBC
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

		// Token: 0x040002C6 RID: 710
		private float _cameraYaw;

		// Token: 0x040002C7 RID: 711
		private float _cameraPitch;

		// Token: 0x040002C8 RID: 712
		private float _cameraRoll;

		// Token: 0x040002C9 RID: 713
		private float _cameraInitialYaw;

		// Token: 0x040002CA RID: 714
		private float _cameraInitialPitch;

		// Token: 0x040002CB RID: 715
		private Vec3 _cameraPositionOffset;

		// Token: 0x040002CC RID: 716
		private bool _isInWeaponCameraMode;

		// Token: 0x040002CD RID: 717
		protected bool UsesMouseForAiming;
	}
}
