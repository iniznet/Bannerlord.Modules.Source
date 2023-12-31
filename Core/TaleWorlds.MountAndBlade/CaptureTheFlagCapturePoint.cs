﻿using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CaptureTheFlagCapturePoint
	{
		public float Progress { get; set; }

		public CaptureTheFlagFlagDirection Direction { get; set; }

		public float Speed { get; set; }

		public MatrixFrame InitialFlagFrame { get; private set; }

		public GameEntity FlagEntity { get; private set; }

		public SynchedMissionObject FlagHolder { get; private set; }

		public GameEntity FlagBottomBoundary { get; private set; }

		public GameEntity FlagTopBoundary { get; private set; }

		public BattleSideEnum BattleSide { get; }

		public int Index { get; }

		public bool UpdateFlag { get; set; }

		public CaptureTheFlagCapturePoint(GameEntity flagPole, BattleSideEnum battleSide, int index)
		{
			this.Reset();
			this.BattleSide = battleSide;
			this.Index = index;
			this.FlagHolder = flagPole.CollectChildrenEntitiesWithTag("score_stand").SingleOrDefault<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
			this.FlagEntity = this.FlagHolder.GameEntity.GetChildren().Single((GameEntity q) => q.HasTag("flag"));
			this.FlagHolder.GameEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
			this.FlagEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
			this.FlagBottomBoundary = flagPole.GetChildren().Single((GameEntity q) => q.HasTag("flag_raising_bottom"));
			this.FlagTopBoundary = flagPole.GetChildren().Single((GameEntity q) => q.HasTag("flag_raising_top"));
			MatrixFrame globalFrame = this.FlagHolder.GameEntity.GetGlobalFrame();
			globalFrame.origin.z = this.FlagBottomBoundary.GetGlobalFrame().origin.z;
			this.InitialFlagFrame = globalFrame;
		}

		public void Reset()
		{
			this.Progress = 0f;
			this.Direction = CaptureTheFlagFlagDirection.None;
			this.Speed = 0f;
			this.UpdateFlag = false;
		}
	}
}
