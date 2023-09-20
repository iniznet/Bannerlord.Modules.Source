using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Pawns
{
	public class PawnPuluc : PawnBase
	{
		public float Height
		{
			get
			{
				if (PawnPuluc._height == 0f)
				{
					PawnPuluc._height = (base.Entity.GetBoundingBoxMax() - base.Entity.GetBoundingBoxMin()).z;
				}
				return PawnPuluc._height;
			}
		}

		public override Vec3 PosBeforeMoving
		{
			get
			{
				return this.PosBeforeMovingBase - new Vec3(0f, 0f, this.Height * (float)this.PawnsBelow.Count, -1f);
			}
		}

		public override bool IsPlaced
		{
			get
			{
				return (this.InPlay || this.IsInSpawn) && this.IsTopPawn;
			}
		}

		public int X
		{
			get
			{
				return this._x;
			}
			set
			{
				this._x = value;
				if (value >= 0 && value < 11)
				{
					this.IsInSpawn = false;
					return;
				}
				this.IsInSpawn = true;
			}
		}

		public List<PawnPuluc> PawnsBelow { get; }

		public bool InPlay
		{
			get
			{
				return this.X >= 0 && this.X < 11;
			}
		}

		public PawnPuluc(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.PawnsBelow = new List<PawnPuluc>();
			this.SpawnPos = base.CurrentPos;
			this.X = -1;
		}

		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.State = PawnPuluc.MovementState.MovingForward;
			this.IsTopPawn = true;
			this.IsInSpawn = true;
			this.CapturedBy = null;
			this.PawnsBelow.Clear();
		}

		public override void AddGoalPosition(Vec3 goal)
		{
			if (this.IsTopPawn)
			{
				goal.z += this.Height * (float)this.PawnsBelow.Count;
				int count = this.PawnsBelow.Count;
				for (int i = 0; i < count; i++)
				{
					this.PawnsBelow[i].AddGoalPosition(goal - new Vec3(0f, 0f, (float)(i + 1) * this.Height, -1f));
				}
			}
			base.GoalPositions.Add(goal);
		}

		public override void MovePawnToGoalPositions(bool instantMove, float speed, bool dragged = false)
		{
			if (base.GoalPositions.Count == 0)
			{
				return;
			}
			base.MovePawnToGoalPositions(instantMove, speed, dragged);
			if (this.IsTopPawn)
			{
				foreach (PawnPuluc pawnPuluc in this.PawnsBelow)
				{
					pawnPuluc.MovePawnToGoalPositions(instantMove, speed, dragged);
				}
			}
		}

		public override void SetPawnAtPosition(Vec3 position)
		{
			base.SetPawnAtPosition(position);
			if (this.IsTopPawn)
			{
				int num = 1;
				foreach (PawnPuluc pawnPuluc in this.PawnsBelow)
				{
					pawnPuluc.SetPawnAtPosition(new Vec3(position.x, position.y, position.z - this.Height * (float)num, -1f));
					num++;
				}
			}
		}

		public override void EnableCollisionBody()
		{
			base.EnableCollisionBody();
			foreach (PawnPuluc pawnPuluc in this.PawnsBelow)
			{
				pawnPuluc.Entity.BodyFlag &= -2;
			}
		}

		public override void DisableCollisionBody()
		{
			base.DisableCollisionBody();
			foreach (PawnPuluc pawnPuluc in this.PawnsBelow)
			{
				pawnPuluc.Entity.BodyFlag |= 1;
			}
		}

		public void MovePawnBackToSpawn(bool instantMove, float speed, bool fake = false)
		{
			this.X = -1;
			this.State = PawnPuluc.MovementState.MovingForward;
			this.IsTopPawn = true;
			this.IsInSpawn = true;
			base.Captured = false;
			this.CapturedBy = null;
			this.PawnsBelow.Clear();
			if (!fake)
			{
				this.AddGoalPosition(this.SpawnPos);
				this.MovePawnToGoalPositions(instantMove, speed, false);
			}
		}

		public PawnPuluc.MovementState State;

		public PawnPuluc CapturedBy;

		public Vec3 SpawnPos;

		public bool IsInSpawn = true;

		public bool IsTopPawn = true;

		private static float _height;

		private int _x;

		public enum MovementState
		{
			MovingForward,
			MovingBackward,
			ChangingDirection
		}
	}
}
