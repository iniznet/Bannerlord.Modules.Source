using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000B9 RID: 185
	public abstract class PawnBase
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x0005B7CE File Offset: 0x000599CE
		// (set) Token: 0x06000B34 RID: 2868 RVA: 0x0005B7D5 File Offset: 0x000599D5
		public static int PawnMoveSoundCodeID { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000B35 RID: 2869 RVA: 0x0005B7DD File Offset: 0x000599DD
		// (set) Token: 0x06000B36 RID: 2870 RVA: 0x0005B7E4 File Offset: 0x000599E4
		public static int PawnSelectSoundCodeID { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000B37 RID: 2871 RVA: 0x0005B7EC File Offset: 0x000599EC
		// (set) Token: 0x06000B38 RID: 2872 RVA: 0x0005B7F3 File Offset: 0x000599F3
		public static int PawnTapSoundCodeID { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000B39 RID: 2873 RVA: 0x0005B7FB File Offset: 0x000599FB
		// (set) Token: 0x06000B3A RID: 2874 RVA: 0x0005B802 File Offset: 0x00059A02
		public static int PawnRemoveSoundCodeID { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000B3B RID: 2875
		public abstract bool IsPlaced { get; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000B3C RID: 2876 RVA: 0x0005B80A File Offset: 0x00059A0A
		// (set) Token: 0x06000B3D RID: 2877 RVA: 0x0005B812 File Offset: 0x00059A12
		public virtual Vec3 PosBeforeMoving
		{
			get
			{
				return this.PosBeforeMovingBase;
			}
			protected set
			{
				this.PosBeforeMovingBase = value;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000B3E RID: 2878 RVA: 0x0005B81B File Offset: 0x00059A1B
		public GameEntity Entity { get; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x0005B823 File Offset: 0x00059A23
		protected List<Vec3> GoalPositions { get; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000B40 RID: 2880 RVA: 0x0005B82B File Offset: 0x00059A2B
		// (set) Token: 0x06000B41 RID: 2881 RVA: 0x0005B833 File Offset: 0x00059A33
		private protected Vec3 CurrentPos { protected get; private set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000B42 RID: 2882 RVA: 0x0005B83C File Offset: 0x00059A3C
		// (set) Token: 0x06000B43 RID: 2883 RVA: 0x0005B844 File Offset: 0x00059A44
		public bool Captured { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000B44 RID: 2884 RVA: 0x0005B84D File Offset: 0x00059A4D
		// (set) Token: 0x06000B45 RID: 2885 RVA: 0x0005B855 File Offset: 0x00059A55
		public bool MovingToDifferentTile { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0005B85E File Offset: 0x00059A5E
		// (set) Token: 0x06000B47 RID: 2887 RVA: 0x0005B866 File Offset: 0x00059A66
		public bool Moving { get; private set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x0005B86F File Offset: 0x00059A6F
		// (set) Token: 0x06000B49 RID: 2889 RVA: 0x0005B877 File Offset: 0x00059A77
		public bool PlayerOne { get; private set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x0005B880 File Offset: 0x00059A80
		public bool HasAnyGoalPosition
		{
			get
			{
				bool flag = false;
				if (this.GoalPositions != null)
				{
					flag = !Extensions.IsEmpty<Vec3>(this.GoalPositions);
				}
				return flag;
			}
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x0005B8A8 File Offset: 0x00059AA8
		protected PawnBase(GameEntity entity, bool playerOne)
		{
			this.Entity = entity;
			this.PlayerOne = playerOne;
			this.CurrentPos = this.Entity.GetGlobalFrame().origin;
			this.PosBeforeMoving = this.CurrentPos;
			this.Moving = false;
			this._dragged = false;
			this.Captured = false;
			this._movePauseDuration = 0.3f;
			this.GoalPositions = new List<Vec3>();
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0005B918 File Offset: 0x00059B18
		public virtual void Reset()
		{
			this.ClearGoalPositions();
			this.Moving = false;
			this.MovingToDifferentTile = false;
			this._movePauseDuration = 0.3f;
			this._movePauseTimer = 0f;
			this._moveTiming = false;
			this._dragged = false;
			this.Captured = false;
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0005B964 File Offset: 0x00059B64
		public virtual void AddGoalPosition(Vec3 goal)
		{
			this.GoalPositions.Add(goal);
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x0005B974 File Offset: 0x00059B74
		public virtual void SetPawnAtPosition(Vec3 position)
		{
			MatrixFrame globalFrame = this.Entity.GetGlobalFrame();
			globalFrame.origin = position;
			this.Entity.SetGlobalFrame(ref globalFrame);
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x0005B9A4 File Offset: 0x00059BA4
		public virtual void MovePawnToGoalPositions(bool instantMove, float speed, bool dragged = false)
		{
			this.PosBeforeMoving = this.Entity.GlobalPosition;
			this._moveSpeed = speed;
			this._currentGoalPos = 0;
			this._movePauseTimer = 0f;
			this._dtCounter = 0f;
			this._moveTiming = false;
			this._dragged = dragged;
			if (this.GoalPositions.Count == 1 && this.PosBeforeMoving.Equals(this.GoalPositions[0]))
			{
				instantMove = true;
			}
			if (instantMove)
			{
				MatrixFrame globalFrame = this.Entity.GetGlobalFrame();
				globalFrame.origin = this.GoalPositions[this.GoalPositions.Count - 1];
				this.Entity.SetGlobalFrame(ref globalFrame);
				this.ClearGoalPositions();
				return;
			}
			this.Moving = true;
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x0005BA75 File Offset: 0x00059C75
		public virtual void EnableCollisionBody()
		{
			this.Entity.BodyFlag &= -2;
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x0005BA8B File Offset: 0x00059C8B
		public virtual void DisableCollisionBody()
		{
			this.Entity.BodyFlag |= 1;
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x0005BAA0 File Offset: 0x00059CA0
		public void Tick(float dt)
		{
			if (this._moveTiming)
			{
				this._movePauseTimer += dt;
				if (this._movePauseTimer >= this._movePauseDuration)
				{
					this._moveTiming = false;
					this._movePauseTimer = 0f;
				}
				return;
			}
			if (this.Moving)
			{
				Vec3 vec;
				vec..ctor(0f, 0f, 0f, -1f);
				Vec3 vec2 = this.GoalPositions[this._currentGoalPos] - this.PosBeforeMoving;
				float num = vec2.Normalize();
				float num2 = num / this._moveSpeed;
				float num3 = this._dtCounter / num2;
				if (this._dtCounter.Equals(0f))
				{
					float x = (this.Entity.GlobalBoxMax - this.Entity.GlobalBoxMin).x;
					float z = (this.Entity.GlobalBoxMax - this.Entity.GlobalBoxMin).z;
					Vec3 vec3;
					vec3..ctor(0f, 0f, z / 2f, -1f);
					Vec3 vec4 = this.Entity.GetGlobalFrame().origin + vec3 + vec2 * (x / 1.8f);
					Vec3 vec5 = this.GoalPositions[this._currentGoalPos] + vec3;
					float num4;
					if (Mission.Current.Scene.RayCastForClosestEntityOrTerrain(vec4, vec5, ref num4, 0.001f, 0))
					{
						this._freePathToDestination = false;
						num = num4;
					}
					else
					{
						this._freePathToDestination = true;
						if (!this._dragged)
						{
							this.PlayPawnMoveSound();
						}
						else
						{
							this.PlayPawnTapSound();
						}
					}
				}
				if (!this._freePathToDestination)
				{
					float num5 = MathF.Sin(num3 * 3.1415927f);
					float num6 = num / 6f;
					num5 *= num6;
					vec += new Vec3(0f, 0f, num5, -1f);
				}
				float dtCounter = this._dtCounter;
				this._dtCounter += dt;
				if (num3 >= 1f)
				{
					this._dtCounter = 0f;
					this.CurrentPos = this.GoalPositions[this._currentGoalPos];
					vec = Vec3.Zero;
					if (!this._freePathToDestination && this.IsPlaced)
					{
						this.PlayPawnTapSound();
					}
					else if (!this.IsPlaced)
					{
						this.PlayPawnRemovedTapSound();
					}
					Vec3 vec6 = this.GoalPositions[this._currentGoalPos];
					bool flag = true;
					while (this._currentGoalPos < this.GoalPositions.Count - 1)
					{
						this._currentGoalPos++;
						Vec3 vec7 = this.GoalPositions[this._currentGoalPos];
						if ((vec6 - vec7).LengthSquared > 0f)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						Action<PawnBase, Vec3, Vec3> onArrivedFinalGoalPosition = this.OnArrivedFinalGoalPosition;
						if (onArrivedFinalGoalPosition != null)
						{
							onArrivedFinalGoalPosition(this, this.PosBeforeMoving, this.CurrentPos);
						}
						this.Moving = false;
						this.ClearGoalPositions();
					}
					else
					{
						Action<PawnBase, Vec3, Vec3> onArrivedIntermediateGoalPosition = this.OnArrivedIntermediateGoalPosition;
						if (onArrivedIntermediateGoalPosition != null)
						{
							onArrivedIntermediateGoalPosition(this, this.PosBeforeMoving, this.CurrentPos);
						}
						this._movePauseDuration = 0.3f;
						this._moveTiming = true;
					}
					this.PosBeforeMoving = this.CurrentPos;
				}
				else
				{
					this.Moving = true;
					this.CurrentPos = MBMath.Lerp(this.PosBeforeMoving, this.GoalPositions[this._currentGoalPos], num3, 0.005f);
				}
				MatrixFrame matrixFrame;
				matrixFrame..ctor(this.Entity.GetGlobalFrame().rotation, this.CurrentPos + vec);
				this.Entity.SetGlobalFrame(ref matrixFrame);
			}
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0005BE34 File Offset: 0x0005A034
		public void MovePawnToGoalPositionsDelayed(bool instantMove, float speed, bool dragged, float delay)
		{
			if (this.GoalPositions.Count > 0)
			{
				if (this.GoalPositions.Count == 1 && this.PosBeforeMoving.Equals(this.GoalPositions[0]))
				{
					this.ClearGoalPositions();
					return;
				}
				this.MovePawnToGoalPositions(instantMove, speed, dragged);
				this._movePauseDuration = delay;
				this._moveTiming = delay > 0f;
			}
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0005BEAB File Offset: 0x0005A0AB
		public void SetPlayerOne(bool playerOne)
		{
			this.PlayerOne = playerOne;
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0005BEB4 File Offset: 0x0005A0B4
		public void ClearGoalPositions()
		{
			this.MovingToDifferentTile = false;
			this.GoalPositions.Clear();
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0005BEC8 File Offset: 0x0005A0C8
		public void UpdatePawnPosition()
		{
			this.PosBeforeMoving = this.Entity.GlobalPosition;
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x0005BEDB File Offset: 0x0005A0DB
		public void PlayPawnSelectSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnSelectSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0005BEF6 File Offset: 0x0005A0F6
		private void PlayPawnTapSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnTapSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0005BF11 File Offset: 0x0005A111
		private void PlayPawnRemovedTapSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnRemoveSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0005BF2C File Offset: 0x0005A12C
		private void PlayPawnMoveSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnMoveSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		// Token: 0x040003EF RID: 1007
		public Action<PawnBase, Vec3, Vec3> OnArrivedIntermediateGoalPosition;

		// Token: 0x040003F0 RID: 1008
		public Action<PawnBase, Vec3, Vec3> OnArrivedFinalGoalPosition;

		// Token: 0x040003F1 RID: 1009
		protected Vec3 PosBeforeMovingBase;

		// Token: 0x040003F2 RID: 1010
		private int _currentGoalPos;

		// Token: 0x040003F3 RID: 1011
		private float _dtCounter;

		// Token: 0x040003F4 RID: 1012
		private float _movePauseDuration;

		// Token: 0x040003F5 RID: 1013
		private float _movePauseTimer;

		// Token: 0x040003F6 RID: 1014
		private float _moveSpeed;

		// Token: 0x040003F7 RID: 1015
		private bool _moveTiming;

		// Token: 0x040003F8 RID: 1016
		private bool _dragged;

		// Token: 0x040003F9 RID: 1017
		private bool _freePathToDestination;
	}
}
