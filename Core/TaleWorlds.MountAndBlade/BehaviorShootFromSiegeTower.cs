using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200011D RID: 285
	public class BehaviorShootFromSiegeTower : BehaviorComponent
	{
		// Token: 0x06000D87 RID: 3463 RVA: 0x00023477 File Offset: 0x00021677
		public BehaviorShootFromSiegeTower(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._siegeTower = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeTower>().FirstOrDefault((SiegeTower st) => st.WeaponSide == this._behaviorSide);
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x000234B8 File Offset: 0x000216B8
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

		// Token: 0x06000D89 RID: 3465 RVA: 0x00023546 File Offset: 0x00021746
		protected override float GetAiWeight()
		{
			return 0f;
		}

		// Token: 0x04000345 RID: 837
		private SiegeTower _siegeTower;
	}
}
