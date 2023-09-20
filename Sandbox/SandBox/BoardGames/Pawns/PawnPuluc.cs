using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000BC RID: 188
	public class PawnPuluc : PawnBase
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000B63 RID: 2915 RVA: 0x0005BFF5 File Offset: 0x0005A1F5
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

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000B64 RID: 2916 RVA: 0x0005C02D File Offset: 0x0005A22D
		public override Vec3 PosBeforeMoving
		{
			get
			{
				return this.PosBeforeMovingBase - new Vec3(0f, 0f, this.Height * (float)this.PawnsBelow.Count, -1f);
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000B65 RID: 2917 RVA: 0x0005C061 File Offset: 0x0005A261
		public override bool IsPlaced
		{
			get
			{
				return (this.InPlay || this.IsInSpawn) && this.IsTopPawn;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000B66 RID: 2918 RVA: 0x0005C07B File Offset: 0x0005A27B
		// (set) Token: 0x06000B67 RID: 2919 RVA: 0x0005C083 File Offset: 0x0005A283
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

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x0005C0A4 File Offset: 0x0005A2A4
		public List<PawnPuluc> PawnsBelow { get; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x0005C0AC File Offset: 0x0005A2AC
		public bool InPlay
		{
			get
			{
				return this.X >= 0 && this.X < 11;
			}
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0005C0C3 File Offset: 0x0005A2C3
		public PawnPuluc(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.PawnsBelow = new List<PawnPuluc>();
			this.SpawnPos = base.CurrentPos;
			this.X = -1;
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0005C0F9 File Offset: 0x0005A2F9
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

		// Token: 0x06000B6C RID: 2924 RVA: 0x0005C130 File Offset: 0x0005A330
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

		// Token: 0x06000B6D RID: 2925 RVA: 0x0005C1C0 File Offset: 0x0005A3C0
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

		// Token: 0x06000B6E RID: 2926 RVA: 0x0005C234 File Offset: 0x0005A434
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

		// Token: 0x06000B6F RID: 2927 RVA: 0x0005C2C0 File Offset: 0x0005A4C0
		public override void EnableCollisionBody()
		{
			base.EnableCollisionBody();
			foreach (PawnPuluc pawnPuluc in this.PawnsBelow)
			{
				pawnPuluc.Entity.BodyFlag &= -2;
			}
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0005C324 File Offset: 0x0005A524
		public override void DisableCollisionBody()
		{
			base.DisableCollisionBody();
			foreach (PawnPuluc pawnPuluc in this.PawnsBelow)
			{
				pawnPuluc.Entity.BodyFlag |= 1;
			}
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0005C388 File Offset: 0x0005A588
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

		// Token: 0x04000406 RID: 1030
		public PawnPuluc.MovementState State;

		// Token: 0x04000407 RID: 1031
		public PawnPuluc CapturedBy;

		// Token: 0x04000408 RID: 1032
		public Vec3 SpawnPos;

		// Token: 0x04000409 RID: 1033
		public bool IsInSpawn = true;

		// Token: 0x0400040A RID: 1034
		public bool IsTopPawn = true;

		// Token: 0x0400040B RID: 1035
		private static float _height;

		// Token: 0x0400040C RID: 1036
		private int _x;

		// Token: 0x020001B8 RID: 440
		public enum MovementState
		{
			// Token: 0x04000827 RID: 2087
			MovingForward,
			// Token: 0x04000828 RID: 2088
			MovingBackward,
			// Token: 0x04000829 RID: 2089
			ChangingDirection
		}
	}
}
