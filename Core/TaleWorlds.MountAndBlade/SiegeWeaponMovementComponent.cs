using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeWeaponMovementComponent : UsableMissionObjectComponent
	{
		public bool HasApproachedTarget
		{
			get
			{
				return !this._pathTracker.PathExists() || this._pathTracker.PathTraveledPercentage > 0.7f;
			}
		}

		public Vec3 Velocity { get; private set; }

		protected internal override void OnAdded(Scene scene)
		{
			base.OnAdded(scene);
			this._path = scene.GetPathWithName(this.PathEntityName);
			MatrixFrame matrixFrame = this.MainObject.GameEntity.GetFrame();
			Vec3 scaleVector = matrixFrame.rotation.GetScaleVector();
			this._wheels = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("wheel");
			this._standingPoints = this.MainObject.GameEntity.CollectObjectsWithTag("move");
			this._pathTracker = new PathTracker(this._path, scaleVector);
			this._pathTracker.Reset();
			this.SetTargetFrame();
			MatrixFrame globalFrame = this.MainObject.GameEntity.GetGlobalFrame();
			this._standingPointLocalIKFrames = new MatrixFrame[this._standingPoints.Count];
			for (int i = 0; i < this._standingPoints.Count; i++)
			{
				MatrixFrame[] standingPointLocalIKFrames = this._standingPointLocalIKFrames;
				int num = i;
				matrixFrame = this._standingPoints[i].GameEntity.GetGlobalFrame();
				standingPointLocalIKFrames[num] = matrixFrame.TransformToLocal(globalFrame);
				this._standingPoints[i].AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
			}
			this.Velocity = Vec3.Zero;
		}

		public void HighlightPath()
		{
			MatrixFrame[] array = new MatrixFrame[this._path.NumberOfPoints];
			this._path.GetPoints(array);
			for (int i = 1; i < this._path.NumberOfPoints; i++)
			{
				MatrixFrame matrixFrame = array[i];
			}
		}

		public void SetupGhostEntity()
		{
			Path pathWithName = this.MainObject.Scene.GetPathWithName(this.PathEntityName);
			Vec3 scaleVector = this.MainObject.GameEntity.GetFrame().rotation.GetScaleVector();
			this._pathTracker = new PathTracker(pathWithName, scaleVector);
			this._ghostEntityPathTracker = new PathTracker(pathWithName, scaleVector);
			this._ghostObjectPos = ((pathWithName != null) ? pathWithName.GetTotalLength() : 0f);
			this._wheels = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("wheel");
		}

		public bool HasArrivedAtTarget
		{
			get
			{
				return !this._pathTracker.PathExists() || this._pathTracker.HasReachedEnd;
			}
		}

		private void SetPath()
		{
			Path pathWithName = this.MainObject.Scene.GetPathWithName(this.PathEntityName);
			Vec3 scaleVector = this.MainObject.GameEntity.GetFrame().rotation.GetScaleVector();
			this._pathTracker = new PathTracker(pathWithName, scaleVector);
			this._ghostEntityPathTracker = new PathTracker(pathWithName, scaleVector);
			this._ghostObjectPos = ((pathWithName != null) ? pathWithName.GetTotalLength() : 0f);
			this.UpdateGhostObject(0f);
		}

		public float CurrentSpeed { get; private set; }

		public int MovementSoundCodeID { get; set; }

		public float MinSpeed { get; set; }

		public float MaxSpeed { get; set; }

		public string PathEntityName { get; set; }

		public float GhostEntitySpeedMultiplier { get; set; }

		public float WheelDiameter
		{
			set
			{
				this._wheelDiameter = value;
				this._wheelCircumference = this._wheelDiameter * 3.1415927f;
			}
		}

		public SynchedMissionObject MainObject { get; set; }

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.UpdateGhostObject(dt);
		}

		public void SetGhostVisibility(bool isVisible)
		{
			this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("ghost_object").FirstOrDefault<GameEntity>().SetVisibilityExcludeParents(isVisible);
		}

		public void OnEditorInit()
		{
			this.SetPath();
			this._wheels = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("wheel");
		}

		private void UpdateGhostObject(float dt)
		{
			if (this._pathTracker.HasChanged)
			{
				this.SetPath();
				this._pathTracker.Advance(this._pathTracker.GetPathLength());
				this._ghostEntityPathTracker.Advance(this._ghostEntityPathTracker.GetPathLength());
			}
			List<GameEntity> list = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("ghost_object");
			if (this.MainObject.GameEntity.IsSelectedOnEditor())
			{
				if (this._pathTracker.IsValid)
				{
					float num = 10f;
					if (Input.DebugInput.IsShiftDown())
					{
						num = 1f;
					}
					if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollUp))
					{
						this._ghostObjectPos += dt * num;
					}
					else if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollDown))
					{
						this._ghostObjectPos -= dt * num;
					}
					this._ghostObjectPos = MBMath.ClampFloat(this._ghostObjectPos, 0f, this._pathTracker.GetPathLength());
				}
				else
				{
					this._ghostObjectPos = 0f;
				}
			}
			if (list.Count > 0)
			{
				GameEntity gameEntity = list[0];
				IPathHolder pathHolder;
				if ((pathHolder = this.MainObject as IPathHolder) != null && pathHolder.EditorGhostEntityMove)
				{
					if (this._ghostEntityPathTracker.IsValid)
					{
						this._ghostEntityPathTracker.Advance(0.05f * this.GhostEntitySpeedMultiplier);
						MatrixFrame matrixFrame = MatrixFrame.Identity;
						matrixFrame = this.LinearInterpolatedIK(ref this._ghostEntityPathTracker);
						gameEntity.SetGlobalFrame(matrixFrame);
						if (this._ghostEntityPathTracker.HasReachedEnd)
						{
							this._ghostEntityPathTracker.Reset();
							return;
						}
					}
				}
				else if (this._pathTracker.IsValid)
				{
					this._pathTracker.Advance(this._ghostObjectPos);
					MatrixFrame matrixFrame2 = this.LinearInterpolatedIK(ref this._pathTracker);
					GameEntity gameEntity2 = gameEntity;
					MatrixFrame matrixFrame3 = this.FindGroundFrameForWheels(ref matrixFrame2);
					gameEntity2.SetGlobalFrame(matrixFrame3);
					this._pathTracker.Reset();
				}
			}
		}

		private void RotateWheels(float angleInRadian)
		{
			foreach (GameEntity gameEntity in this._wheels)
			{
				MatrixFrame frame = gameEntity.GetFrame();
				frame.rotation.RotateAboutSide(angleInRadian);
				gameEntity.SetFrame(ref frame);
			}
		}

		private MatrixFrame LinearInterpolatedIK(ref PathTracker pathTracker)
		{
			MatrixFrame matrixFrame;
			Vec3 vec;
			pathTracker.CurrentFrameAndColor(out matrixFrame, out vec);
			MatrixFrame matrixFrame2 = this.FindGroundFrameForWheels(ref matrixFrame);
			return MatrixFrame.Lerp(matrixFrame, matrixFrame2, vec.x);
		}

		public void SetDistanceTraveledAsClient(float distance)
		{
			this._advancementError = distance - this._pathTracker.TotalDistanceTraveled;
		}

		public override bool IsOnTickRequired()
		{
			return true;
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._ghostEntityPathTracker != null)
			{
				this.UpdateGhostObject(dt);
			}
			if (!this._pathTracker.PathExists() || this._pathTracker.HasReachedEnd)
			{
				this.CurrentSpeed = 0f;
				if (!GameNetwork.IsClientOrReplay)
				{
					foreach (StandingPoint standingPoint in this._standingPoints)
					{
						standingPoint.SetIsDeactivatedSynched(true);
					}
				}
			}
			this.TickSound();
		}

		public void TickParallelManually(float dt)
		{
			if (this._pathTracker.PathExists() && !this._pathTracker.HasReachedEnd)
			{
				int num = 0;
				foreach (StandingPoint standingPoint in this._standingPoints)
				{
					if (standingPoint.HasUser && !standingPoint.UserAgent.IsInBeingStruckAction)
					{
						num++;
					}
				}
				if (num > 0)
				{
					int count = this._standingPoints.Count;
					this.CurrentSpeed = MBMath.Lerp(this.MinSpeed, this.MaxSpeed, (float)(num - 1) / (float)(count - 1), 1E-05f);
					MatrixFrame globalFrame = this.MainObject.GameEntity.GetGlobalFrame();
					for (int i = 0; i < this._standingPoints.Count; i++)
					{
						StandingPoint standingPoint2 = this._standingPoints[i];
						if (standingPoint2.HasUser)
						{
							Agent userAgent = standingPoint2.UserAgent;
							ActionIndexValueCache actionIndexValueCache = userAgent.GetCurrentActionValue(0);
							ActionIndexValueCache actionIndexValueCache2 = userAgent.GetCurrentActionValue(1);
							if (actionIndexValueCache != SiegeWeaponMovementComponent.act_usage_siege_machine_push)
							{
								if (userAgent.SetActionChannel(0, SiegeWeaponMovementComponent.act_usage_siege_machine_push, false, 0UL, 0f, this.CurrentSpeed, MBAnimation.GetAnimationBlendInPeriod(MBActionSet.GetAnimationIndexOfAction(userAgent.ActionSet, SiegeWeaponMovementComponent.act_usage_siege_machine_push)) * this.CurrentSpeed, 0.4f, 0f, false, -0.2f, 0, true))
								{
									actionIndexValueCache = ActionIndexValueCache.Create(SiegeWeaponMovementComponent.act_usage_siege_machine_push);
								}
								else if (MBMath.IsBetween((int)userAgent.GetCurrentActionType(0), 47, 51) && actionIndexValueCache != SiegeWeaponMovementComponent.act_strike_bent_over && userAgent.SetActionChannel(0, SiegeWeaponMovementComponent.act_strike_bent_over, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
								{
									actionIndexValueCache = ActionIndexValueCache.Create(SiegeWeaponMovementComponent.act_strike_bent_over);
								}
							}
							if (actionIndexValueCache2 != SiegeWeaponMovementComponent.act_usage_siege_machine_push)
							{
								if (userAgent.SetActionChannel(1, SiegeWeaponMovementComponent.act_usage_siege_machine_push, false, 0UL, 0f, this.CurrentSpeed, MBAnimation.GetAnimationBlendInPeriod(MBActionSet.GetAnimationIndexOfAction(userAgent.ActionSet, SiegeWeaponMovementComponent.act_usage_siege_machine_push)) * this.CurrentSpeed, 0.4f, 0f, false, -0.2f, 0, true))
								{
									actionIndexValueCache2 = ActionIndexValueCache.Create(SiegeWeaponMovementComponent.act_usage_siege_machine_push);
								}
								else if (MBMath.IsBetween((int)userAgent.GetCurrentActionType(1), 47, 51) && actionIndexValueCache2 != SiegeWeaponMovementComponent.act_strike_bent_over && userAgent.SetActionChannel(1, SiegeWeaponMovementComponent.act_strike_bent_over, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
								{
									actionIndexValueCache2 = ActionIndexValueCache.Create(SiegeWeaponMovementComponent.act_strike_bent_over);
								}
							}
							if (actionIndexValueCache == SiegeWeaponMovementComponent.act_usage_siege_machine_push)
							{
								userAgent.SetCurrentActionSpeed(0, this.CurrentSpeed);
							}
							if (actionIndexValueCache2 == SiegeWeaponMovementComponent.act_usage_siege_machine_push)
							{
								userAgent.SetCurrentActionSpeed(1, this.CurrentSpeed);
							}
							if ((actionIndexValueCache == SiegeWeaponMovementComponent.act_usage_siege_machine_push || actionIndexValueCache == SiegeWeaponMovementComponent.act_strike_bent_over) && (actionIndexValueCache2 == SiegeWeaponMovementComponent.act_usage_siege_machine_push || actionIndexValueCache2 == SiegeWeaponMovementComponent.act_strike_bent_over))
							{
								standingPoint2.UserAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this._standingPointLocalIKFrames[i], globalFrame, 0f);
							}
							else
							{
								standingPoint2.UserAgent.ClearHandInverseKinematics();
								if (!GameNetwork.IsClientOrReplay && userAgent.Controller != Agent.ControllerType.AI)
								{
									userAgent.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
								}
							}
						}
					}
				}
				else
				{
					this.CurrentSpeed = this._advancementError;
				}
				if (!this.CurrentSpeed.ApproximatelyEqualsTo(0f, 1E-05f))
				{
					float num2 = this.CurrentSpeed * dt;
					if (!this._advancementError.ApproximatelyEqualsTo(0f, 1E-05f))
					{
						float num3 = 3f * this.CurrentSpeed * dt * (float)MathF.Sign(this._advancementError);
						if (MathF.Abs(num3) >= MathF.Abs(this._advancementError))
						{
							num3 = this._advancementError;
							this._advancementError = 0f;
						}
						else
						{
							this._advancementError -= num3;
						}
						num2 += num3;
					}
					this._pathTracker.Advance(num2);
					this.SetTargetFrame();
					float num4 = num2 / this._wheelCircumference * 2f * 3.1415927f;
					this.RotateWheels(num4);
					if (GameNetwork.IsServerOrRecorder && this._pathTracker.TotalDistanceTraveled - this._lastSynchronizedDistance > 1f)
					{
						this._lastSynchronizedDistance = this._pathTracker.TotalDistanceTraveled;
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetSiegeMachineMovementDistance(this.MainObject.Id, this._lastSynchronizedDistance));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
				}
			}
		}

		public MatrixFrame GetInitialFrame()
		{
			PathTracker pathTracker = new PathTracker(this._path, Vec3.One);
			pathTracker.Reset();
			return this.LinearInterpolatedIK(ref pathTracker);
		}

		private void SetTargetFrame()
		{
			if (!this._pathTracker.PathExists())
			{
				return;
			}
			MatrixFrame matrixFrame = this.LinearInterpolatedIK(ref this._pathTracker);
			GameEntity gameEntity = this.MainObject.GameEntity;
			this.Velocity = gameEntity.GlobalPosition;
			gameEntity.SetGlobalFrameMT(matrixFrame);
			this.Velocity = (gameEntity.GlobalPosition - this.Velocity).NormalizedCopy() * this.CurrentSpeed;
		}

		public MatrixFrame GetTargetFrame()
		{
			float totalDistanceTraveled = this._pathTracker.TotalDistanceTraveled;
			this._pathTracker.Advance(1000000f);
			MatrixFrame currentFrame = this._pathTracker.CurrentFrame;
			this._pathTracker.Reset();
			this._pathTracker.Advance(totalDistanceTraveled);
			return currentFrame;
		}

		public void SetDestinationNavMeshIdState(bool enabled)
		{
			if (this.NavMeshIdToDisableOnDestination != -1)
			{
				Mission.Current.Scene.SetAbilityOfFacesWithId(this.NavMeshIdToDisableOnDestination, enabled);
			}
		}

		public void MoveToTargetAsClient()
		{
			if (this._pathTracker.IsValid)
			{
				float totalDistanceTraveled = this._pathTracker.TotalDistanceTraveled;
				this._pathTracker.Advance(1000000f);
				this.SetTargetFrame();
				float num = (this._pathTracker.TotalDistanceTraveled - totalDistanceTraveled) / this._wheelCircumference * 2f * 3.1415927f;
				this.RotateWheels(num);
			}
		}

		private void TickSound()
		{
			if (this.CurrentSpeed > 0f)
			{
				this.PlayMovementSound();
				return;
			}
			this.StopMovementSound();
		}

		private void PlayMovementSound()
		{
			if (!this._isMoveSoundPlaying)
			{
				this._movementSound = SoundEvent.CreateEvent(this.MovementSoundCodeID, this.MainObject.GameEntity.Scene);
				this._movementSound.Play();
				this._isMoveSoundPlaying = true;
			}
			this._movementSound.SetPosition(this.MainObject.GameEntity.GlobalPosition);
		}

		private void StopMovementSound()
		{
			if (this._isMoveSoundPlaying)
			{
				this._movementSound.Stop();
				this._isMoveSoundPlaying = false;
			}
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this.CurrentSpeed = 0f;
			this._lastSynchronizedDistance = 0f;
			this._advancementError = 0f;
			this._pathTracker.Reset();
			this.SetTargetFrame();
		}

		public float GetTotalDistanceTraveledForPathTracker()
		{
			return this._pathTracker.TotalDistanceTraveled;
		}

		private MatrixFrame FindGroundFrameForWheels(ref MatrixFrame frame)
		{
			return SiegeWeaponMovementComponent.FindGroundFrameForWheelsStatic(ref frame, this.AxleLength, this._wheelDiameter, this.MainObject.GameEntity, this._wheels, this.MainObject.Scene);
		}

		public void SetTotalDistanceTraveledForPathTracker(float distanceTraveled)
		{
			this._pathTracker.TotalDistanceTraveled = distanceTraveled;
		}

		public void SetTargetFrameForPathTracker()
		{
			this.SetTargetFrame();
		}

		public static MatrixFrame FindGroundFrameForWheelsStatic(ref MatrixFrame frame, float axleLength, float wheelDiameter, GameEntity gameEntity, List<GameEntity> wheels, Scene scene)
		{
			Vec3.StackArray8Vec3 stackArray8Vec = default(Vec3.StackArray8Vec3);
			bool visibilityExcludeParents = gameEntity.GetVisibilityExcludeParents();
			if (visibilityExcludeParents)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
			int num = 0;
			using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
			{
				foreach (GameEntity gameEntity2 in wheels)
				{
					Vec3 vec = frame.TransformToParent(gameEntity2.GetFrame().origin);
					Vec3 vec2 = vec + frame.rotation.s * axleLength + (wheelDiameter * 0.5f + 0.5f) * frame.rotation.u;
					Vec3 vec3 = vec - frame.rotation.s * axleLength + (wheelDiameter * 0.5f + 0.5f) * frame.rotation.u;
					vec2.z = scene.GetGroundHeightAtPositionMT(vec2, BodyFlags.CommonCollisionExcludeFlags);
					vec3.z = scene.GetGroundHeightAtPositionMT(vec3, BodyFlags.CommonCollisionExcludeFlags);
					stackArray8Vec[num++] = vec2;
					stackArray8Vec[num++] = vec3;
				}
			}
			if (visibilityExcludeParents)
			{
				gameEntity.SetVisibilityExcludeParents(true);
			}
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			Vec3 vec4 = default(Vec3);
			for (int i = 0; i < num; i++)
			{
				vec4 += stackArray8Vec[i];
			}
			vec4 /= (float)num;
			for (int j = 0; j < num; j++)
			{
				Vec3 vec5 = stackArray8Vec[j] - vec4;
				num2 += vec5.x * vec5.x;
				num3 += vec5.x * vec5.y;
				num4 += vec5.y * vec5.y;
				num5 += vec5.x * vec5.z;
				num6 += vec5.y * vec5.z;
			}
			float num7 = num2 * num4 - num3 * num3;
			float num8 = (num6 * num3 - num5 * num4) / num7;
			float num9 = (num3 * num5 - num2 * num6) / num7;
			MatrixFrame matrixFrame;
			matrixFrame.origin = vec4;
			matrixFrame.rotation.u = new Vec3(num8, num9, 1f, -1f);
			matrixFrame.rotation.u.Normalize();
			matrixFrame.rotation.f = frame.rotation.f;
			matrixFrame.rotation.f = matrixFrame.rotation.f - Vec3.DotProduct(matrixFrame.rotation.f, matrixFrame.rotation.u) * matrixFrame.rotation.u;
			matrixFrame.rotation.f.Normalize();
			matrixFrame.rotation.s = Vec3.CrossProduct(matrixFrame.rotation.f, matrixFrame.rotation.u);
			matrixFrame.rotation.s.Normalize();
			return matrixFrame;
		}

		public const string GhostObjectTag = "ghost_object";

		private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create("act_strike_bent_over");

		private static readonly ActionIndexCache act_usage_siege_machine_push = ActionIndexCache.Create("act_usage_siege_machine_push");

		private const string WheelTag = "wheel";

		public const string MoveStandingPointTag = "move";

		public float AxleLength = 2.45f;

		public int NavMeshIdToDisableOnDestination = -1;

		private float _ghostObjectPos;

		private List<GameEntity> _wheels;

		private List<StandingPoint> _standingPoints;

		private MatrixFrame[] _standingPointLocalIKFrames;

		private SoundEvent _movementSound;

		private float _wheelCircumference;

		private bool _isMoveSoundPlaying;

		private float _wheelDiameter;

		private Path _path;

		private PathTracker _pathTracker;

		private PathTracker _ghostEntityPathTracker;

		private float _advancementError;

		private float _lastSynchronizedDistance;
	}
}
