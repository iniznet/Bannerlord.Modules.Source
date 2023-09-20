using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000145 RID: 325
	public abstract class OrderComponent
	{
		// Token: 0x060010AF RID: 4271 RVA: 0x00036DF8 File Offset: 0x00034FF8
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

		// Token: 0x060010B0 RID: 4272 RVA: 0x00036E3F File Offset: 0x0003503F
		protected void CopyPositionAndDirectionFrom(OrderComponent order)
		{
			this.Position = order.Position;
			this.Direction = order.Direction;
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00036E59 File Offset: 0x00035059
		protected OrderComponent(float tickTimerDuration = 0.5f)
		{
			this._tickTimer = new Timer(Mission.Current.CurrentTime, tickTimerDuration, true);
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x060010B2 RID: 4274
		public abstract OrderType OrderType { get; }

		// Token: 0x060010B3 RID: 4275 RVA: 0x00036E83 File Offset: 0x00035083
		internal bool Tick(Formation formation)
		{
			bool flag = this._tickTimer.Check(Mission.Current.CurrentTime);
			if (flag)
			{
				this.TickOccasionally(formation, this._tickTimer.PreviousDeltaTime);
			}
			return flag;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00036EAF File Offset: 0x000350AF
		[Conditional("DEBUG")]
		protected virtual void TickDebug(Formation formation)
		{
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00036EB1 File Offset: 0x000350B1
		protected internal virtual void TickOccasionally(Formation formation, float dt)
		{
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00036EB3 File Offset: 0x000350B3
		protected internal virtual void OnApply(Formation formation)
		{
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00036EB5 File Offset: 0x000350B5
		protected internal virtual void OnCancel(Formation formation)
		{
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00036EB7 File Offset: 0x000350B7
		protected internal virtual void OnUnitJoinOrLeave(Agent unit, bool isJoining)
		{
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00036EB9 File Offset: 0x000350B9
		protected internal virtual bool IsApplicable(Formation formation)
		{
			return true;
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x00036EBC File Offset: 0x000350BC
		protected internal virtual bool CanStack
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x060010BB RID: 4283 RVA: 0x00036EBF File Offset: 0x000350BF
		protected internal virtual bool CancelsPreviousDirectionOrder
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x060010BC RID: 4284 RVA: 0x00036EC2 File Offset: 0x000350C2
		protected internal virtual bool CancelsPreviousArrangementOrder
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00036EC5 File Offset: 0x000350C5
		protected internal virtual MovementOrder GetSubstituteOrder(Formation formation)
		{
			return MovementOrder.MovementOrderCharge;
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00036ECC File Offset: 0x000350CC
		protected internal virtual void OnArrangementChanged(Formation formation)
		{
		}

		// Token: 0x04000437 RID: 1079
		private readonly Timer _tickTimer;

		// Token: 0x04000438 RID: 1080
		protected Func<Formation, Vec3> Position;

		// Token: 0x04000439 RID: 1081
		protected Func<Formation, Vec2> Direction;

		// Token: 0x0400043A RID: 1082
		private Vec2 _previousDirection = Vec2.Invalid;
	}
}
