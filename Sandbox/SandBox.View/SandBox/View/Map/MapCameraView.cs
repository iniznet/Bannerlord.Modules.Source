using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View.Map
{
	public class MapCameraView : MapView
	{
		protected virtual MapCameraView.CameraFollowMode CurrentCameraFollowMode { get; set; }

		public virtual float CameraFastMoveMultiplier { get; protected set; }

		protected virtual float CameraBearing { get; set; }

		protected virtual float MaximumCameraHeight
		{
			get
			{
				return Math.Max(this._customMaximumCameraHeight, Campaign.MapMaximumHeight);
			}
		}

		protected virtual float CameraBearingVelocity { get; set; }

		public virtual float CameraDistance { get; protected set; }

		protected virtual float TargetCameraDistance { get; set; }

		protected virtual float AdditionalElevation { get; set; }

		public virtual bool CameraAnimationInProgress { get; protected set; }

		public virtual bool ProcessCameraInput { get; protected set; }

		public virtual Camera Camera { get; protected set; }

		public virtual MatrixFrame CameraFrame
		{
			get
			{
				return this._cameraFrame;
			}
			protected set
			{
				this._cameraFrame = value;
			}
		}

		protected virtual Vec3 IdealCameraTarget { get; set; }

		private static MapCameraView Instance { get; set; }

		public MapCameraView()
		{
			this.Camera = Camera.CreateCamera();
			this.Camera.SetViewVolume(true, -0.1f, 0.1f, -0.07f, 0.07f, 0.2f, 300f);
			this.Camera.Position = new Vec3(0f, 0f, 10f, -1f);
			this.CameraBearing = 0f;
			this._cameraElevation = 1f;
			this.CameraDistance = 2.5f;
			this.ProcessCameraInput = true;
			this.CameraFastMoveMultiplier = 4f;
			this._cameraFrame = MatrixFrame.Identity;
			this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.FollowParty;
			this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			MapCameraView.Instance = this;
		}

		public virtual void OnActivate(bool leftButtonDraggingMode, Vec3 clickedPosition)
		{
			this.SetCameraMode(MapCameraView.CameraFollowMode.FollowParty);
			this.CameraBearingVelocity = 0f;
			this.UpdateMapCamera(leftButtonDraggingMode, clickedPosition);
		}

		public virtual void Initialize()
		{
			if (MobileParty.MainParty != null && PartyBase.MainParty.IsValid)
			{
				float num = 0f;
				this._mapScene.GetHeightAtPoint(MobileParty.MainParty.Position2D, 2208137, ref num);
				this.IdealCameraTarget = new Vec3(MobileParty.MainParty.Position2D, num + 1f, -1f);
			}
			this._cameraTarget = this.IdealCameraTarget;
		}

		protected internal override void OnFinalize()
		{
			base.OnFinalize();
			MapCameraView.Instance = null;
		}

		public virtual void SetCameraMode(MapCameraView.CameraFollowMode cameraMode)
		{
			this.CurrentCameraFollowMode = cameraMode;
		}

		public virtual void ResetCamera(bool resetDistance, bool teleportToMainParty)
		{
			if (teleportToMainParty)
			{
				this.TeleportCameraToMainParty();
			}
			if (resetDistance)
			{
				this.TargetCameraDistance = 15f;
				this.CameraDistance = 15f;
			}
			this.CameraBearing = 0f;
			this._cameraElevation = 1f;
		}

		public virtual void TeleportCameraToMainParty()
		{
			this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.FollowParty;
			Campaign.Current.CameraFollowParty = MobileParty.MainParty.Party;
			this.IdealCameraTarget = this.GetCameraTargetForParty(Campaign.Current.CameraFollowParty);
			this._lastUsedIdealCameraTarget = this.IdealCameraTarget.AsVec2;
			this._cameraTarget = this.IdealCameraTarget;
		}

		public virtual void FastMoveCameraToMainParty()
		{
			this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.FollowParty;
			Campaign.Current.CameraFollowParty = MobileParty.MainParty.Party;
			this.IdealCameraTarget = this.GetCameraTargetForParty(Campaign.Current.CameraFollowParty);
			this._doFastCameraMovementToTarget = true;
			this.TargetCameraDistance = 15f;
		}

		public virtual void FastMoveCameraToPosition(Vec2 target, bool isInMenu)
		{
			if (!isInMenu)
			{
				this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.MoveToPosition;
				this.IdealCameraTarget = this.GetCameraTargetForPosition(target);
				this._doFastCameraMovementToTarget = true;
				this.TargetCameraDistance = 15f;
			}
		}

		public virtual bool IsCameraLockedToPlayerParty()
		{
			return this.CurrentCameraFollowMode == MapCameraView.CameraFollowMode.FollowParty && Campaign.Current.CameraFollowParty == MobileParty.MainParty.Party;
		}

		public virtual void StartCameraAnimation(Vec2 targetPosition, float animationStopDuration)
		{
			this.CameraAnimationInProgress = true;
			this._cameraAnimationTarget = targetPosition;
			this._cameraAnimationStopDuration = animationStopDuration;
			Campaign.Current.SetTimeSpeed(0);
			Campaign.Current.SetTimeControlModeLock(true);
		}

		public virtual void SiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			if (this.TargetCameraDistance > 18f)
			{
				this.TargetCameraDistance = 18f;
			}
		}

		public virtual void OnExit()
		{
			this.ProcessCameraInput = true;
		}

		public virtual void OnEscapeMenuToggled(bool isOpened)
		{
			this.ProcessCameraInput = !isOpened;
		}

		public virtual void HandleMouse(bool rightMouseButtonPressed, float verticalCameraInput, float mouseMoveY, float dt)
		{
			float num = 0.3f / 700f;
			float num2 = -(700f - MathF.Min(700f, MathF.Max(50f, this.CameraDistance))) * num;
			float num3 = MathF.Max(num2 + 1E-05f, 1.5550884f - this.CalculateCameraElevation(this.CameraDistance));
			if (rightMouseButtonPressed)
			{
				this.AdditionalElevation = MBMath.ClampFloat(this.AdditionalElevation + mouseMoveY * 0.0015f, num2, num3);
			}
			if (verticalCameraInput != 0f)
			{
				this.AdditionalElevation = MBMath.ClampFloat(this.AdditionalElevation - verticalCameraInput * dt, num2, num3);
			}
		}

		public virtual void HandleLeftMouseButtonClick(bool isMouseActive)
		{
			if (isMouseActive)
			{
				this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.FollowParty;
				Campaign.Current.CameraFollowParty = PartyBase.MainParty;
			}
		}

		public virtual void OnSetMapSiegeOverlayState(bool isActive, bool isMapSiegeOverlayViewNull)
		{
			if (isActive && isMapSiegeOverlayViewNull && PlayerSiege.PlayerSiegeEvent != null)
			{
				this.TargetCameraDistance = 13f;
			}
		}

		public virtual void OnRefreshMapSiegeOverlayRequired(bool isMapSiegeOverlayViewNull)
		{
			if (PlayerSiege.PlayerSiegeEvent != null && isMapSiegeOverlayViewNull)
			{
				this.TargetCameraDistance = 13f;
			}
		}

		public virtual void OnBeforeTick(in MapCameraView.InputInformation inputInformation)
		{
			float num = MathF.Min(1f, MathF.Max(0f, 1f - this.CameraFrame.rotation.f.z)) + 0.15f;
			this._mapScene.SetDepthOfFieldParameters(0.05f, num * 1000f, true);
			this._mapScene.SetDepthOfFieldFocus(0.05f);
			MobileParty mainParty = MobileParty.MainParty;
			if (inputInformation.IsMainPartyValid && this.CameraAnimationInProgress)
			{
				Campaign.Current.TimeControlMode = 0;
				if (this._cameraAnimationStopDuration > 0f)
				{
					if (this._cameraAnimationTarget.DistanceSquared(this._cameraTarget.AsVec2) < 0.0001f)
					{
						this._cameraAnimationStopDuration = MathF.Max(this._cameraAnimationStopDuration - inputInformation.Dt, 0f);
					}
					else
					{
						float terrainHeight = this._mapScene.GetTerrainHeight(this._cameraAnimationTarget, true);
						this.IdealCameraTarget = this._cameraAnimationTarget.ToVec3(terrainHeight + 1f);
					}
				}
				else if (MobileParty.MainParty.Position2D.DistanceSquared(this._cameraTarget.AsVec2) < 0.0001f)
				{
					this.CameraAnimationInProgress = false;
					Campaign.Current.SetTimeControlModeLock(false);
				}
				else
				{
					this.IdealCameraTarget = MobileParty.MainParty.GetPosition() + Vec3.Up;
				}
			}
			bool flag = this.CameraAnimationInProgress;
			if (this.ProcessCameraInput && !this.CameraAnimationInProgress && inputInformation.IsMapReady)
			{
				flag = this.GetMapCameraInput(inputInformation);
			}
			if (flag)
			{
				Vec3 vec = this.IdealCameraTarget - this._cameraTarget;
				Vec3 vec2 = 10f * vec * inputInformation.Dt;
				float num2 = MathF.Sqrt(MathF.Max(this.CameraDistance, 20f)) * 0.15f;
				float num3 = (this._doFastCameraMovementToTarget ? (num2 * 5f) : num2);
				if (vec2.LengthSquared > num3 * num3)
				{
					vec2 = vec2.NormalizedCopy() * num3;
				}
				if (vec2.LengthSquared < num2 * num2)
				{
					this._doFastCameraMovementToTarget = false;
				}
				this._cameraTarget += vec2;
			}
			else
			{
				this._cameraTarget = this.IdealCameraTarget;
				this._doFastCameraMovementToTarget = false;
			}
			if (inputInformation.IsMainPartyValid)
			{
				if (inputInformation.CameraFollowModeKeyPressed)
				{
					this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.FollowParty;
				}
				if ((!inputInformation.IsInMenu && !inputInformation.MiddleMouseButtonDown && (MobileParty.MainParty == null || MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty) && inputInformation.PartyMoveRightKey) || inputInformation.PartyMoveLeftKey || inputInformation.PartyMoveUpKey || inputInformation.PartyMoveDownKey)
				{
					float num4 = 0f;
					float num5 = 0f;
					float num6;
					float num7;
					MathF.SinCos(this.CameraBearing, ref num6, ref num7);
					float num8;
					float num9;
					MathF.SinCos(this.CameraBearing + 1.5707964f, ref num8, ref num9);
					float num10 = 0.5f;
					if (inputInformation.PartyMoveUpKey)
					{
						num5 += num7 * num10;
						num4 += num6 * num10;
						mainParty.Ai.ForceAiNoPathMode = true;
					}
					if (inputInformation.PartyMoveDownKey)
					{
						num5 -= num7 * num10;
						num4 -= num6 * num10;
						mainParty.Ai.ForceAiNoPathMode = true;
					}
					if (inputInformation.PartyMoveLeftKey)
					{
						num5 -= num9 * num10;
						num4 -= num8 * num10;
						mainParty.Ai.ForceAiNoPathMode = true;
					}
					if (inputInformation.PartyMoveRightKey)
					{
						num5 += num9 * num10;
						num4 += num8 * num10;
						mainParty.Ai.ForceAiNoPathMode = true;
					}
					this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.FollowParty;
					mainParty.Ai.SetMoveGoToPoint(mainParty.Position2D + new Vec2(num4, num5));
					Campaign.Current.TimeControlMode = 3;
				}
				else if (mainParty.Ai.ForceAiNoPathMode)
				{
					mainParty.Ai.SetMoveGoToPoint(mainParty.Position2D);
				}
			}
			this.UpdateMapCamera(inputInformation.LeftButtonDraggingMode, inputInformation.ClickedPosition);
		}

		protected virtual void UpdateMapCamera(bool _leftButtonDraggingMode, Vec3 _clickedPosition)
		{
			this._lastUsedIdealCameraTarget = this.IdealCameraTarget.AsVec2;
			MatrixFrame matrixFrame = this.ComputeMapCamera(ref this._cameraTarget, this.CameraBearing, this._cameraElevation, this.CameraDistance, ref this._lastUsedIdealCameraTarget);
			bool flag = !matrixFrame.origin.NearlyEquals(this._cameraFrame.origin, 1E-05f);
			bool flag2 = !matrixFrame.rotation.NearlyEquals(this._cameraFrame.rotation, 1E-05f);
			if (flag2 || flag)
			{
				Game.Current.EventManager.TriggerEvent<MainMapCameraMoveEvent>(new MainMapCameraMoveEvent(flag2, flag));
			}
			this._cameraFrame = matrixFrame;
			float num = 0f;
			this._mapScene.GetHeightAtPoint(this._cameraFrame.origin.AsVec2, 2208137, ref num);
			num += 0.5f;
			if (this._cameraFrame.origin.z < num)
			{
				if (_leftButtonDraggingMode)
				{
					Vec3 vec = _clickedPosition - Vec3.DotProduct(_clickedPosition - this._cameraFrame.origin, this._cameraFrame.rotation.s) * this._cameraFrame.rotation.s;
					Vec3 vec2 = Vec3.CrossProduct((vec - this._cameraFrame.origin).NormalizedCopy(), (vec - (this._cameraFrame.origin + new Vec3(0f, 0f, num - this._cameraFrame.origin.z, -1f))).NormalizedCopy());
					float num2 = vec2.Normalize();
					this._cameraFrame.origin.z = num;
					this._cameraFrame.rotation.u = this._cameraFrame.rotation.u.RotateAboutAnArbitraryVector(vec2, num2);
					this._cameraFrame.rotation.f = Vec3.CrossProduct(this._cameraFrame.rotation.u, this._cameraFrame.rotation.s).NormalizedCopy();
					this._cameraFrame.rotation.s = Vec3.CrossProduct(this._cameraFrame.rotation.f, this._cameraFrame.rotation.u);
					Vec3 vec3 = -Vec3.Up;
					Vec3 vec4 = -this._cameraFrame.rotation.u;
					Vec3 idealCameraTarget = this.IdealCameraTarget;
					float num3;
					if (MBMath.GetRayPlaneIntersectionPoint(ref vec3, ref idealCameraTarget, ref this._cameraFrame.origin, ref vec4, ref num3))
					{
						this.IdealCameraTarget = this._cameraFrame.origin + vec4 * num3;
						this._cameraTarget = this.IdealCameraTarget;
					}
					this._cameraElevation = -new Vec2(this._cameraFrame.rotation.f.AsVec2.Length, this._cameraFrame.rotation.f.z).RotationInRadians;
					this.CameraDistance = (this._cameraFrame.origin - this.IdealCameraTarget).Length - 2f;
					this.TargetCameraDistance = this.CameraDistance;
					this.AdditionalElevation = this._cameraElevation - this.CalculateCameraElevation(this.CameraDistance);
					this._lastUsedIdealCameraTarget = this.IdealCameraTarget.AsVec2;
					this.ComputeMapCamera(ref this._cameraTarget, this.CameraBearing, this._cameraElevation, this.CameraDistance, ref this._lastUsedIdealCameraTarget);
				}
				else
				{
					float num4 = 0.47123894f;
					int num5 = 0;
					do
					{
						this._cameraElevation += ((this._cameraFrame.origin.z < num) ? num4 : (-num4));
						this.AdditionalElevation = this._cameraElevation - this.CalculateCameraElevation(this.CameraDistance);
						float num6 = 700f;
						float num7 = 0.3f / num6;
						float num8 = 50f;
						float num9 = -(num6 - MathF.Min(num6, MathF.Max(num8, this.CameraDistance))) * num7;
						float num10 = MathF.Max(num9 + 1E-05f, 1.5550884f - this.CalculateCameraElevation(this.CameraDistance));
						this.AdditionalElevation = MBMath.ClampFloat(this.AdditionalElevation, num9, num10);
						this._cameraElevation = this.AdditionalElevation + this.CalculateCameraElevation(this.CameraDistance);
						Vec2 zero = Vec2.Zero;
						this._cameraFrame = this.ComputeMapCamera(ref this._cameraTarget, this.CameraBearing, this._cameraElevation, this.CameraDistance, ref zero);
						this._mapScene.GetHeightAtPoint(this._cameraFrame.origin.AsVec2, 2208137, ref num);
						num += 0.5f;
						if (num4 > 0.0001f)
						{
							num4 *= 0.5f;
						}
						else
						{
							num5++;
						}
					}
					while (num4 > 0.0001f || (this._cameraFrame.origin.z < num && num5 < 5));
					if (this._cameraFrame.origin.z < num)
					{
						this._cameraFrame.origin.z = num;
						Vec3 vec5 = -Vec3.Up;
						Vec3 vec6 = -this._cameraFrame.rotation.u;
						Vec3 idealCameraTarget2 = this.IdealCameraTarget;
						float num11;
						if (MBMath.GetRayPlaneIntersectionPoint(ref vec5, ref idealCameraTarget2, ref this._cameraFrame.origin, ref vec6, ref num11))
						{
							this.IdealCameraTarget = this._cameraFrame.origin + vec6 * num11;
							this._cameraTarget = this.IdealCameraTarget;
						}
						this.CameraDistance = (this._cameraFrame.origin - this.IdealCameraTarget).Length - 2f;
						this._lastUsedIdealCameraTarget = this.IdealCameraTarget.AsVec2;
						this.ComputeMapCamera(ref this._cameraTarget, this.CameraBearing, this._cameraElevation, this.CameraDistance, ref this._lastUsedIdealCameraTarget);
						this.TargetCameraDistance = MathF.Max(this.TargetCameraDistance, this.CameraDistance);
					}
				}
			}
			this.Camera.Frame = this._cameraFrame;
			this.Camera.SetFovVertical(0.6981317f, Screen.AspectRatio, 0.01f, this.MaximumCameraHeight * 4f);
			this._mapScene.SetDepthOfFieldFocus(0f);
			this._mapScene.SetDepthOfFieldParameters(0f, 0f, false);
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation = this._cameraFrame.rotation;
			identity.origin = this._cameraTarget;
			this._mapScene.GetHeightAtPoint(identity.origin.AsVec2, 2208137, ref identity.origin.z);
			identity.origin = MBMath.Lerp(identity.origin, this._cameraFrame.origin, 0.075f, 1E-05f);
			PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(identity.origin.AsVec2);
			if (faceIndex.IsValid())
			{
				TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(faceIndex);
				MBMapScene.TickAmbientSounds(this._mapScene, faceTerrainType);
			}
			SoundManager.SetListenerFrame(identity);
		}

		protected virtual Vec3 GetCameraTargetForPosition(Vec2 targetPosition)
		{
			float terrainHeight = this._mapScene.GetTerrainHeight(targetPosition, true);
			return new Vec3(targetPosition, terrainHeight + 1f, -1f);
		}

		protected virtual Vec3 GetCameraTargetForParty(PartyBase party)
		{
			Vec2 vec;
			if (party.IsMobile && party.MobileParty.CurrentSettlement != null)
			{
				vec = party.MobileParty.CurrentSettlement.Position2D;
			}
			else if (party.IsMobile && party.MobileParty.BesiegedSettlement != null)
			{
				if (PlayerSiege.PlayerSiegeEvent != null)
				{
					Vec2 asVec = party.MobileParty.BesiegedSettlement.Party.Visuals.GetSiegeCamp1GlobalFrames().First<MatrixFrame>().origin.AsVec2;
					vec = Vec2.Lerp(party.MobileParty.BesiegedSettlement.GatePosition, asVec, 0.75f);
				}
				else
				{
					vec = party.MobileParty.BesiegedSettlement.GatePosition;
				}
			}
			else
			{
				vec = party.Position2D;
			}
			return this.GetCameraTargetForPosition(vec);
		}

		protected virtual bool GetMapCameraInput(MapCameraView.InputInformation inputInformation)
		{
			bool flag = false;
			bool flag2 = !inputInformation.LeftButtonDraggingMode;
			if (inputInformation.IsControlDown && inputInformation.CheatModeEnabled)
			{
				flag = true;
				if (inputInformation.DeltaMouseScroll > 0.01f)
				{
					this.CameraFastMoveMultiplier *= 1.25f;
				}
				else if (inputInformation.DeltaMouseScroll < -0.01f)
				{
					this.CameraFastMoveMultiplier *= 0.8f;
				}
				this.CameraFastMoveMultiplier = MBMath.ClampFloat(this.CameraFastMoveMultiplier, 1f, 37.252903f);
			}
			Vec2 vec = Vec2.Zero;
			if (!inputInformation.LeftMouseButtonPressed && inputInformation.LeftMouseButtonDown && !inputInformation.LeftMouseButtonReleased && inputInformation.MousePositionPixel.DistanceSquared(inputInformation.ClickedPositionPixel) > 300f && !inputInformation.IsInMenu)
			{
				if (!inputInformation.LeftButtonDraggingMode)
				{
					this.IdealCameraTarget = this._cameraTarget;
					this._lastUsedIdealCameraTarget = this.IdealCameraTarget.AsVec2;
				}
				Vec3 vec2 = (inputInformation.WorldMouseFar - inputInformation.WorldMouseNear).NormalizedCopy();
				Vec3 vec3 = -Vec3.Up;
				float num;
				if (MBMath.GetRayPlaneIntersectionPoint(ref vec3, ref inputInformation.ClickedPosition, ref inputInformation.WorldMouseNear, ref vec2, ref num))
				{
					this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.Free;
					Vec3 vec4 = inputInformation.WorldMouseNear + vec2 * num;
					vec = inputInformation.ClickedPosition.AsVec2 - vec4.AsVec2;
				}
			}
			if (inputInformation.MiddleMouseButtonDown)
			{
				this.TargetCameraDistance += 0.01f * (this.CameraDistance + 20f) * inputInformation.MouseSensitivity * inputInformation.MouseMoveY;
			}
			if (inputInformation.RotateLeftKeyDown)
			{
				this.CameraBearingVelocity = inputInformation.Dt * 2f;
			}
			else if (inputInformation.RotateRightKeyDown)
			{
				this.CameraBearingVelocity = inputInformation.Dt * -2f;
			}
			this.CameraBearingVelocity += inputInformation.HorizontalCameraInput * 1.75f * inputInformation.Dt;
			if (inputInformation.RightMouseButtonDown)
			{
				this.CameraBearingVelocity += 0.01f * inputInformation.MouseSensitivity * inputInformation.MouseMoveX;
			}
			float num2 = 0.1f;
			if (!inputInformation.IsMouseActive)
			{
				num2 *= inputInformation.Dt * 10f;
			}
			if (!flag)
			{
				this.TargetCameraDistance -= inputInformation.MapZoomIn * num2 * (this.CameraDistance + 20f);
				this.TargetCameraDistance += inputInformation.MapZoomOut * num2 * (this.CameraDistance + 20f);
			}
			PartyBase cameraFollowParty = Campaign.Current.CameraFollowParty;
			this.TargetCameraDistance = MBMath.ClampFloat(this.TargetCameraDistance, 2.5f, (cameraFollowParty != null && cameraFollowParty.IsMobile && (cameraFollowParty.MobileParty.BesiegedSettlement != null || (cameraFollowParty.MobileParty.CurrentSettlement != null && cameraFollowParty.MobileParty.CurrentSettlement.IsUnderSiege))) ? 30f : this.MaximumCameraHeight);
			float num3 = this.TargetCameraDistance - this.CameraDistance;
			float num4 = MathF.Abs(num3);
			float num5 = ((num4 > 0.001f) ? (this.CameraDistance + num3 * inputInformation.Dt * 8f) : this.TargetCameraDistance);
			if (this.CurrentCameraFollowMode == MapCameraView.CameraFollowMode.Free && !inputInformation.RightMouseButtonDown && !inputInformation.LeftMouseButtonDown && num4 >= 0.001f && (inputInformation.WorldMouseFar - this.CameraFrame.origin).NormalizedCopy().z < -0.2f && inputInformation.RayCastForClosestEntityOrTerrainCondition)
			{
				MatrixFrame matrixFrame = this.ComputeMapCamera(ref this._cameraTarget, this.CameraBearing + this.CameraBearingVelocity, MathF.Min(this.CalculateCameraElevation(num5) + this.AdditionalElevation, 1.5550884f), num5, ref this._lastUsedIdealCameraTarget);
				Vec3 vec5 = -Vec3.Up;
				Vec3 vec6 = (inputInformation.WorldMouseFar - this.CameraFrame.origin).NormalizedCopy();
				MatrixFrame matrixFrame2 = this.CameraFrame;
				Vec3 vec7 = matrixFrame.rotation.TransformToParent(matrixFrame2.rotation.TransformToLocal(vec6));
				float num6;
				if (MBMath.GetRayPlaneIntersectionPoint(ref vec5, ref inputInformation.ProjectedPosition, ref matrixFrame.origin, ref vec7, ref num6))
				{
					vec = inputInformation.ProjectedPosition.AsVec2 - (matrixFrame.origin + vec7 * num6).AsVec2;
					flag2 = false;
				}
			}
			if (inputInformation.RX != 0f || inputInformation.RY != 0f || vec.IsNonZero())
			{
				float num7 = 0.001f * (this.CameraDistance * 0.55f + 15f);
				Vec2 vec8 = Vec2.FromRotation(-this.CameraBearing);
				if ((this.IdealCameraTarget.AsVec2 - this._lastUsedIdealCameraTarget).LengthSquared > 0.010000001f)
				{
					this.IdealCameraTarget = new Vec3(this._lastUsedIdealCameraTarget.x, this._lastUsedIdealCameraTarget.y, this.IdealCameraTarget.z, -1f);
					this._cameraTarget = this.IdealCameraTarget;
				}
				if (!vec.IsNonZero())
				{
					this.IdealCameraTarget = this._cameraTarget;
				}
				Vec2 vec9 = inputInformation.Dt * 500f * inputInformation.RX * vec8.RightVec() * num7 + inputInformation.Dt * 500f * inputInformation.RY * vec8 * num7;
				this.IdealCameraTarget = new Vec3(this.IdealCameraTarget.x + vec.x + vec9.x, this.IdealCameraTarget.y + vec.y + vec9.y, this.IdealCameraTarget.z, -1f);
				if (vec.IsNonZero())
				{
					this._cameraTarget = this.IdealCameraTarget;
				}
				this._cameraTarget.AsVec2 = this._cameraTarget.AsVec2 + vec9;
				if (inputInformation.RX != 0f || inputInformation.RY != 0f)
				{
					this.CurrentCameraFollowMode = MapCameraView.CameraFollowMode.Free;
				}
			}
			this.CameraBearing += this.CameraBearingVelocity;
			this.CameraBearingVelocity = 0f;
			this.CameraDistance = num5;
			this._cameraElevation = MathF.Min(this.CalculateCameraElevation(num5) + this.AdditionalElevation, 1.5550884f);
			if (this.CurrentCameraFollowMode == MapCameraView.CameraFollowMode.FollowParty && cameraFollowParty != null && cameraFollowParty.IsValid)
			{
				Vec2 vec10;
				if (cameraFollowParty.IsMobile && cameraFollowParty.MobileParty.CurrentSettlement != null)
				{
					vec10 = cameraFollowParty.MobileParty.CurrentSettlement.Position2D;
				}
				else if (cameraFollowParty.IsMobile && cameraFollowParty.MobileParty.BesiegedSettlement != null)
				{
					if (PlayerSiege.PlayerSiegeEvent != null)
					{
						MatrixFrame matrixFrame2 = cameraFollowParty.MobileParty.BesiegedSettlement.Party.Visuals.GetSiegeCamp1GlobalFrames().First<MatrixFrame>();
						Vec2 asVec = matrixFrame2.origin.AsVec2;
						vec10 = Vec2.Lerp(cameraFollowParty.MobileParty.BesiegedSettlement.GatePosition, asVec, 0.75f);
					}
					else
					{
						vec10 = cameraFollowParty.MobileParty.BesiegedSettlement.GatePosition;
					}
				}
				else
				{
					vec10 = cameraFollowParty.Position2D;
				}
				float terrainHeight = this._mapScene.GetTerrainHeight(vec10, true);
				this.IdealCameraTarget = new Vec3(vec10.x, vec10.y, terrainHeight + 1f, -1f);
			}
			return flag2;
		}

		protected virtual MatrixFrame ComputeMapCamera(ref Vec3 cameraTarget, float cameraBearing, float cameraElevation, float cameraDistance, ref Vec2 lastUsedIdealCameraTarget)
		{
			Vec2 asVec = cameraTarget.AsVec2;
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = cameraTarget;
			identity.rotation.RotateAboutSide(1.5707964f);
			identity.rotation.RotateAboutForward(-cameraBearing);
			identity.rotation.RotateAboutSide(-cameraElevation);
			identity.origin += identity.rotation.u * (cameraDistance + 2f);
			Vec2 vec = (Campaign.MapMinimumPosition + Campaign.MapMaximumPosition) * 0.5f;
			float num = Campaign.MapMaximumPosition.y - vec.y;
			float num2 = Campaign.MapMaximumPosition.x - vec.x;
			asVec.x = MBMath.ClampFloat(asVec.x, vec.x - num2, vec.x + num2);
			asVec.y = MBMath.ClampFloat(asVec.y, vec.y - num, vec.y + num);
			lastUsedIdealCameraTarget.x = MBMath.ClampFloat(lastUsedIdealCameraTarget.x, vec.x - num2, vec.x + num2);
			lastUsedIdealCameraTarget.y = MBMath.ClampFloat(lastUsedIdealCameraTarget.y, vec.y - num, vec.y + num);
			identity.origin.x = identity.origin.x + (asVec.x - cameraTarget.x);
			identity.origin.y = identity.origin.y + (asVec.y - cameraTarget.y);
			return identity;
		}

		protected virtual float CalculateCameraElevation(float cameraDistance)
		{
			return cameraDistance * 0.5f * 0.015f + 0.35f;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_custom_maximum_map_height", "campaign")]
		protected static string SetCustomMaximumHeight(List<string> strings)
		{
			string text = string.Format("Format is \"campaign.set_custom_maximum_map_height [MaxHeight]\".\n If the given number is below the current base maximum: {0}, it won't be used.", Campaign.MapMaximumHeight);
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			int num;
			if (CampaignCheats.CheckParameters(strings, 1) && int.TryParse(strings[0], out num))
			{
				MapCameraView.Instance._customMaximumCameraHeight = (float)num;
			}
			return text;
		}

		private const float VerticalHalfViewAngle = 0.34906584f;

		private Vec3 _cameraTarget;

		private bool _doFastCameraMovementToTarget;

		private float _cameraElevation;

		private Vec2 _lastUsedIdealCameraTarget;

		private Vec2 _cameraAnimationTarget;

		private float _cameraAnimationStopDuration;

		private readonly Scene _mapScene;

		protected float _customMaximumCameraHeight;

		private MatrixFrame _cameraFrame;

		public enum CameraFollowMode
		{
			Free,
			FollowParty,
			MoveToPosition
		}

		public struct InputInformation
		{
			public bool IsMainPartyValid;

			public bool IsMapReady;

			public bool IsControlDown;

			public bool IsMouseActive;

			public bool CheatModeEnabled;

			public bool LeftMouseButtonPressed;

			public bool LeftMouseButtonDown;

			public bool LeftMouseButtonReleased;

			public bool MiddleMouseButtonDown;

			public bool RightMouseButtonDown;

			public bool RotateLeftKeyDown;

			public bool RotateRightKeyDown;

			public bool PartyMoveUpKey;

			public bool PartyMoveDownKey;

			public bool PartyMoveLeftKey;

			public bool PartyMoveRightKey;

			public bool CameraFollowModeKeyPressed;

			public bool LeftButtonDraggingMode;

			public bool IsInMenu;

			public bool RayCastForClosestEntityOrTerrainCondition;

			public float MapZoomIn;

			public float MapZoomOut;

			public float DeltaMouseScroll;

			public float MouseSensitivity;

			public float MouseMoveX;

			public float MouseMoveY;

			public float HorizontalCameraInput;

			public float RX;

			public float RY;

			public float RS;

			public float Dt;

			public Vec2 MousePositionPixel;

			public Vec2 ClickedPositionPixel;

			public Vec3 ClickedPosition;

			public Vec3 ProjectedPosition;

			public Vec3 WorldMouseNear;

			public Vec3 WorldMouseFar;
		}
	}
}
