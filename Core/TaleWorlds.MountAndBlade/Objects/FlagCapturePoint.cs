using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects
{
	public class FlagCapturePoint : SynchedMissionObject
	{
		[EditableScriptComponentVariable(false)]
		public Vec3 Position
		{
			get
			{
				return base.GameEntity.GlobalPosition;
			}
		}

		public int FlagChar
		{
			get
			{
				return 65 + this.FlagIndex;
			}
		}

		public bool IsContested
		{
			get
			{
				return this._currentDirection == CaptureTheFlagFlagDirection.Down;
			}
		}

		public bool IsFullyRaised
		{
			get
			{
				return this._currentDirection == CaptureTheFlagFlagDirection.None;
			}
		}

		public bool IsDeactivated
		{
			get
			{
				return !base.GameEntity.IsVisibleIncludeParents();
			}
		}

		protected internal override void OnMissionReset()
		{
			this._currentDirection = CaptureTheFlagFlagDirection.None;
		}

		public void ResetPointAsServer(uint defaultColor, uint defaultColor2)
		{
			MatrixFrame globalFrame = this._flagTopBoundary.GetGlobalFrame();
			this._flagHolder.SetGlobalFrameSynched(ref globalFrame, false);
			this.SetTeamColorsWithAllSynched(defaultColor, defaultColor2);
			this.SetVisibleWithAllSynched(true, false);
		}

		public void RemovePointAsServer()
		{
			this.SetVisibleWithAllSynched(false, false);
		}

		protected internal override void OnInit()
		{
			this._flagHolder = base.GameEntity.CollectChildrenEntitiesWithTag("score_stand").SingleOrDefault<GameEntity>().GetScriptComponents<SynchedMissionObject>()
				.SingleOrDefault<SynchedMissionObject>();
			this._theFlag = this._flagHolder.GameEntity.CollectChildrenEntitiesWithTag("flag_white").SingleOrDefault<GameEntity>().GetScriptComponents<SynchedMissionObject>()
				.SingleOrDefault<SynchedMissionObject>();
			this._flagBottomBoundary = base.GameEntity.GetChildren().Single((GameEntity q) => q.HasTag("flag_raising_bottom"));
			this._flagTopBoundary = base.GameEntity.GetChildren().Single((GameEntity q) => q.HasTag("flag_raising_top"));
			MatrixFrame globalFrame = this._flagTopBoundary.GetGlobalFrame();
			this._flagHolder.GameEntity.SetGlobalFrame(globalFrame);
			this._flagDependentObjects = new List<SynchedMissionObject>();
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("depends_flag_" + this.FlagIndex).ToList<GameEntity>())
			{
				this._flagDependentObjects.Add(gameEntity.GetScriptComponents<SynchedMissionObject>().SingleOrDefault<SynchedMissionObject>());
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (MBEditor.IsEntitySelected(base.GameEntity))
			{
				DebugExtensions.RenderDebugCircleOnTerrain(base.Scene, base.GameEntity.GetGlobalFrame(), 4f, 2852192000U, true, false);
				DebugExtensions.RenderDebugCircleOnTerrain(base.Scene, base.GameEntity.GetGlobalFrame(), 6f, 2868838400U, true, false);
			}
		}

		public void OnAfterTick(bool canOwnershipChange, out bool ownerTeamChanged)
		{
			ownerTeamChanged = false;
			if (this._flagHolder.SynchronizeCompleted)
			{
				bool flag = this._flagHolder.GameEntity.GlobalPosition.DistanceSquared(this._flagTopBoundary.GlobalPosition).ApproximatelyEqualsTo(0f, 1E-05f);
				if (canOwnershipChange)
				{
					if (!flag)
					{
						ownerTeamChanged = true;
						return;
					}
					this._currentDirection = CaptureTheFlagFlagDirection.None;
					return;
				}
				else if (flag)
				{
					this._currentDirection = CaptureTheFlagFlagDirection.None;
				}
			}
		}

		public void SetMoveFlag(CaptureTheFlagFlagDirection directionTo, float speedMultiplier = 1f)
		{
			float flagProgress = this.GetFlagProgress();
			float num = 1f / speedMultiplier;
			float num2 = ((directionTo == CaptureTheFlagFlagDirection.Up) ? (1f - flagProgress) : flagProgress);
			float num3 = 10f * num;
			float num4 = num2 * num3;
			this._currentDirection = directionTo;
			MatrixFrame matrixFrame;
			if (directionTo != CaptureTheFlagFlagDirection.Up)
			{
				if (directionTo != CaptureTheFlagFlagDirection.Down)
				{
					throw new ArgumentOutOfRangeException("directionTo", directionTo, null);
				}
				matrixFrame = this._flagBottomBoundary.GetFrame();
			}
			else
			{
				matrixFrame = this._flagTopBoundary.GetFrame();
			}
			this._flagHolder.SetFrameSynchedOverTime(ref matrixFrame, num4, false);
		}

		public void ChangeMovementSpeed(float speedMultiplier)
		{
			if (this._currentDirection != CaptureTheFlagFlagDirection.None)
			{
				this.SetMoveFlag(this._currentDirection, speedMultiplier);
			}
		}

		public void SetMoveNone()
		{
			this._currentDirection = CaptureTheFlagFlagDirection.None;
			MatrixFrame frame = this._flagHolder.GameEntity.GetFrame();
			this._flagHolder.SetFrameSynched(ref frame, false);
		}

		public void SetVisibleWithAllSynched(bool value, bool forceChildrenVisible = false)
		{
			this.SetVisibleSynched(value, forceChildrenVisible);
			foreach (SynchedMissionObject synchedMissionObject in this._flagDependentObjects)
			{
				synchedMissionObject.SetVisibleSynched(value, false);
			}
		}

		public void SetTeamColorsWithAllSynched(uint color, uint color2)
		{
			this._theFlag.SetTeamColorsSynched(color, color2);
			foreach (SynchedMissionObject synchedMissionObject in this._flagDependentObjects)
			{
				synchedMissionObject.SetTeamColorsSynched(color, color2);
			}
		}

		public uint GetFlagColor()
		{
			return this._theFlag.Color;
		}

		public uint GetFlagColor2()
		{
			return this._theFlag.Color2;
		}

		public float GetFlagProgress()
		{
			return MathF.Clamp((this._theFlag.GameEntity.GlobalPosition.z - this._flagBottomBoundary.GlobalPosition.z) / (this._flagTopBoundary.GlobalPosition.z - this._flagBottomBoundary.GlobalPosition.z), 0f, 1f);
		}

		public const float PointRadius = 4f;

		public const float RadiusMultiplierForContestedArea = 1.5f;

		private const float TimeToTravelBetweenBoundaries = 10f;

		public int FlagIndex;

		private SynchedMissionObject _theFlag;

		private SynchedMissionObject _flagHolder;

		private GameEntity _flagBottomBoundary;

		private GameEntity _flagTopBoundary;

		private List<SynchedMissionObject> _flagDependentObjects;

		private CaptureTheFlagFlagDirection _currentDirection = CaptureTheFlagFlagDirection.None;
	}
}
