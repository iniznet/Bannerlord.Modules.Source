using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorShootFromSiegeTower : BehaviorComponent
	{
		public BehaviorShootFromSiegeTower(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._siegeTower = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeTower>().FirstOrDefault((SiegeTower st) => st.WeaponSide == this._behaviorSide);
		}

		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (base.Formation.AI.Side != this._behaviorSide)
			{
				this._behaviorSide = base.Formation.AI.Side;
				this._siegeTower = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeTower>().FirstOrDefault((SiegeTower st) => st.WeaponSide == this._behaviorSide);
			}
			if (this._siegeTower == null || this._siegeTower.IsDestroyed)
			{
				return;
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		protected override float GetAiWeight()
		{
			return 0f;
		}

		private SiegeTower _siegeTower;
	}
}
