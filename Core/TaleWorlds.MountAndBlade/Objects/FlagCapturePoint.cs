using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects
{
	// Token: 0x020003A1 RID: 929
	public class FlagCapturePoint : SynchedMissionObject
	{
		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x060032A5 RID: 12965 RVA: 0x000D1867 File Offset: 0x000CFA67
		[EditableScriptComponentVariable(false)]
		public Vec3 Position
		{
			get
			{
				return base.GameEntity.GlobalPosition;
			}
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x060032A6 RID: 12966 RVA: 0x000D1874 File Offset: 0x000CFA74
		public int FlagChar
		{
			get
			{
				return 65 + this.FlagIndex;
			}
		}

		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x060032A7 RID: 12967 RVA: 0x000D187F File Offset: 0x000CFA7F
		public bool IsContested
		{
			get
			{
				return this._currentDirection == CaptureTheFlagFlagDirection.Down;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x060032A8 RID: 12968 RVA: 0x000D188A File Offset: 0x000CFA8A
		public bool IsFullyRaised
		{
			get
			{
				return this._currentDirection == CaptureTheFlagFlagDirection.None;
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x060032A9 RID: 12969 RVA: 0x000D1895 File Offset: 0x000CFA95
		public bool IsDeactivated
		{
			get
			{
				return !base.GameEntity.IsVisibleIncludeParents();
			}
		}

		// Token: 0x060032AA RID: 12970 RVA: 0x000D18A5 File Offset: 0x000CFAA5
		protected internal override void OnMissionReset()
		{
			this._currentDirection = CaptureTheFlagFlagDirection.None;
		}

		// Token: 0x060032AB RID: 12971 RVA: 0x000D18B0 File Offset: 0x000CFAB0
		public void ResetPointAsServer(uint defaultColor, uint defaultColor2)
		{
			MatrixFrame globalFrame = this._flagTopBoundary.GetGlobalFrame();
			this._flagHolder.SetGlobalFrameSynched(ref globalFrame, false);
			this.SetTeamColorsWithAllSynched(defaultColor, defaultColor2);
			this.SetVisibleWithAllSynched(true, false);
		}

		// Token: 0x060032AC RID: 12972 RVA: 0x000D18E7 File Offset: 0x000CFAE7
		public void RemovePointAsServer()
		{
			this.SetVisibleWithAllSynched(false, false);
		}

		// Token: 0x060032AD RID: 12973 RVA: 0x000D18F4 File Offset: 0x000CFAF4
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

		// Token: 0x060032AE RID: 12974 RVA: 0x000D1A5C File Offset: 0x000CFC5C
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (MBEditor.IsEntitySelected(base.GameEntity))
			{
				DebugExtensions.RenderDebugCircleOnTerrain(base.Scene, base.GameEntity.GetGlobalFrame(), 4f, 2852192000U, true, false);
				DebugExtensions.RenderDebugCircleOnTerrain(base.Scene, base.GameEntity.GetGlobalFrame(), 6f, 2868838400U, true, false);
			}
		}

		// Token: 0x060032AF RID: 12975 RVA: 0x000D1AC4 File Offset: 0x000CFCC4
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

		// Token: 0x060032B0 RID: 12976 RVA: 0x000D1B30 File Offset: 0x000CFD30
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

		// Token: 0x060032B1 RID: 12977 RVA: 0x000D1BB3 File Offset: 0x000CFDB3
		public void ChangeMovementSpeed(float speedMultiplier)
		{
			if (this._currentDirection != CaptureTheFlagFlagDirection.None)
			{
				this.SetMoveFlag(this._currentDirection, speedMultiplier);
			}
		}

		// Token: 0x060032B2 RID: 12978 RVA: 0x000D1BCC File Offset: 0x000CFDCC
		public void SetMoveNone()
		{
			this._currentDirection = CaptureTheFlagFlagDirection.None;
			MatrixFrame frame = this._flagHolder.GameEntity.GetFrame();
			this._flagHolder.SetFrameSynched(ref frame, false);
		}

		// Token: 0x060032B3 RID: 12979 RVA: 0x000D1C00 File Offset: 0x000CFE00
		public void SetVisibleWithAllSynched(bool value, bool forceChildrenVisible = false)
		{
			this.SetVisibleSynched(value, forceChildrenVisible);
			foreach (SynchedMissionObject synchedMissionObject in this._flagDependentObjects)
			{
				synchedMissionObject.SetVisibleSynched(value, false);
			}
		}

		// Token: 0x060032B4 RID: 12980 RVA: 0x000D1C5C File Offset: 0x000CFE5C
		public void SetTeamColorsWithAllSynched(uint color, uint color2)
		{
			this._theFlag.SetTeamColorsSynched(color, color2);
			foreach (SynchedMissionObject synchedMissionObject in this._flagDependentObjects)
			{
				synchedMissionObject.SetTeamColorsSynched(color, color2);
			}
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x000D1CBC File Offset: 0x000CFEBC
		public uint GetFlagColor()
		{
			return this._theFlag.Color;
		}

		// Token: 0x060032B6 RID: 12982 RVA: 0x000D1CC9 File Offset: 0x000CFEC9
		public uint GetFlagColor2()
		{
			return this._theFlag.Color2;
		}

		// Token: 0x060032B7 RID: 12983 RVA: 0x000D1CD8 File Offset: 0x000CFED8
		public float GetFlagProgress()
		{
			return MathF.Clamp((this._theFlag.GameEntity.GlobalPosition.z - this._flagBottomBoundary.GlobalPosition.z) / (this._flagTopBoundary.GlobalPosition.z - this._flagBottomBoundary.GlobalPosition.z), 0f, 1f);
		}

		// Token: 0x04001557 RID: 5463
		public const float PointRadius = 4f;

		// Token: 0x04001558 RID: 5464
		public const float RadiusMultiplierForContestedArea = 1.5f;

		// Token: 0x04001559 RID: 5465
		private const float TimeToTravelBetweenBoundaries = 10f;

		// Token: 0x0400155A RID: 5466
		public int FlagIndex;

		// Token: 0x0400155B RID: 5467
		private SynchedMissionObject _theFlag;

		// Token: 0x0400155C RID: 5468
		private SynchedMissionObject _flagHolder;

		// Token: 0x0400155D RID: 5469
		private GameEntity _flagBottomBoundary;

		// Token: 0x0400155E RID: 5470
		private GameEntity _flagTopBoundary;

		// Token: 0x0400155F RID: 5471
		private List<SynchedMissionObject> _flagDependentObjects;

		// Token: 0x04001560 RID: 5472
		private CaptureTheFlagFlagDirection _currentDirection = CaptureTheFlagFlagDirection.None;
	}
}
