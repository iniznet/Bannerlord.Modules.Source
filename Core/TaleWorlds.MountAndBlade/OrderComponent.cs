using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class OrderComponent
	{
		public Vec2 GetDirection(Formation f)
		{
			Vec2 vec = this.Direction(f);
			if (f.IsAIControlled && vec.DotProduct(this._previousDirection) > 0.87f)
			{
				vec = this._previousDirection;
			}
			else
			{
				this._previousDirection = vec;
			}
			return vec;
		}

		protected void CopyPositionAndDirectionFrom(OrderComponent order)
		{
			this.Position = order.Position;
			this.Direction = order.Direction;
		}

		protected OrderComponent(float tickTimerDuration = 0.5f)
		{
			this._tickTimer = new Timer(Mission.Current.CurrentTime, tickTimerDuration, true);
		}

		public abstract OrderType OrderType { get; }

		internal bool Tick(Formation formation)
		{
			bool flag = this._tickTimer.Check(Mission.Current.CurrentTime);
			if (flag)
			{
				this.TickOccasionally(formation, this._tickTimer.PreviousDeltaTime);
			}
			return flag;
		}

		[Conditional("DEBUG")]
		protected virtual void TickDebug(Formation formation)
		{
		}

		protected internal virtual void TickOccasionally(Formation formation, float dt)
		{
		}

		protected internal virtual void OnApply(Formation formation)
		{
		}

		protected internal virtual void OnCancel(Formation formation)
		{
		}

		protected internal virtual void OnUnitJoinOrLeave(Agent unit, bool isJoining)
		{
		}

		protected internal virtual bool IsApplicable(Formation formation)
		{
			return true;
		}

		protected internal virtual bool CanStack
		{
			get
			{
				return false;
			}
		}

		protected internal virtual bool CancelsPreviousDirectionOrder
		{
			get
			{
				return false;
			}
		}

		protected internal virtual bool CancelsPreviousArrangementOrder
		{
			get
			{
				return false;
			}
		}

		protected internal virtual MovementOrder GetSubstituteOrder(Formation formation)
		{
			return MovementOrder.MovementOrderCharge;
		}

		protected internal virtual void OnArrangementChanged(Formation formation)
		{
		}

		private readonly Timer _tickTimer;

		protected Func<Formation, Vec3> Position;

		protected Func<Formation, Vec2> Direction;

		private Vec2 _previousDirection = Vec2.Invalid;
	}
}
