using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.BoardGames.Pawns
{
	public abstract class PawnBase
	{
		public static int PawnMoveSoundCodeID { get; set; }

		public static int PawnSelectSoundCodeID { get; set; }

		public static int PawnTapSoundCodeID { get; set; }

		public static int PawnRemoveSoundCodeID { get; set; }

		public abstract bool IsPlaced { get; }

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

		public GameEntity Entity { get; }

		protected List<Vec3> GoalPositions { get; }

		private protected Vec3 CurrentPos { protected get; private set; }

		public bool Captured { get; set; }

		public bool MovingToDifferentTile { get; set; }

		public bool Moving { get; private set; }

		public bool PlayerOne { get; private set; }

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

		public virtual void AddGoalPosition(Vec3 goal)
		{
			this.GoalPositions.Add(goal);
		}

		public virtual void SetPawnAtPosition(Vec3 position)
		{
			MatrixFrame globalFrame = this.Entity.GetGlobalFrame();
			globalFrame.origin = position;
			this.Entity.SetGlobalFrame(ref globalFrame);
		}

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

		public virtual void EnableCollisionBody()
		{
			this.Entity.BodyFlag &= -2;
		}

		public virtual void DisableCollisionBody()
		{
			this.Entity.BodyFlag |= 1;
		}

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

		public void SetPlayerOne(bool playerOne)
		{
			this.PlayerOne = playerOne;
		}

		public void ClearGoalPositions()
		{
			this.MovingToDifferentTile = false;
			this.GoalPositions.Clear();
		}

		public void UpdatePawnPosition()
		{
			this.PosBeforeMoving = this.Entity.GlobalPosition;
		}

		public void PlayPawnSelectSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnSelectSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		private void PlayPawnTapSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnTapSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		private void PlayPawnRemovedTapSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnRemoveSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		private void PlayPawnMoveSound()
		{
			Mission.Current.MakeSound(PawnBase.PawnMoveSoundCodeID, this.CurrentPos, true, false, -1, -1);
		}

		public Action<PawnBase, Vec3, Vec3> OnArrivedIntermediateGoalPosition;

		public Action<PawnBase, Vec3, Vec3> OnArrivedFinalGoalPosition;

		protected Vec3 PosBeforeMovingBase;

		private int _currentGoalPos;

		private float _dtCounter;

		private float _movePauseDuration;

		private float _movePauseTimer;

		private float _moveSpeed;

		private bool _moveTiming;

		private bool _dragged;

		private bool _freePathToDestination;
	}
}
