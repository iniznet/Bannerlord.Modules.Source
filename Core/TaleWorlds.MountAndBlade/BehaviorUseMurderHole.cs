using System;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorUseMurderHole : BehaviorComponent
	{
		public BehaviorUseMurderHole(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			WorldPosition worldPosition = new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, (formation.Team.TeamAI as TeamAISiegeDefender).MurderHolePosition, false);
			this._outerGate = (formation.Team.TeamAI as TeamAISiegeDefender).OuterGate;
			this._innerGate = (formation.Team.TeamAI as TeamAISiegeDefender).InnerGate;
			this._batteringRam = base.Formation.Team.Mission.ActiveMissionObjects.FindAllWithType<BatteringRam>().FirstOrDefault<BatteringRam>();
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			base.BehaviorCoherence = 0f;
		}

		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		public bool IsMurderHoleActive()
		{
			return (this._batteringRam != null && this._batteringRam.HasArrivedAtTarget && !this._innerGate.IsDestroyed) || (this._outerGate.IsDestroyed && !this._innerGate.IsDestroyed);
		}

		protected override float GetAiWeight()
		{
			return 10f * (this.IsMurderHoleActive() ? 1f : 0f);
		}

		private readonly CastleGate _outerGate;

		private readonly CastleGate _innerGate;

		private readonly BatteringRam _batteringRam;
	}
}
