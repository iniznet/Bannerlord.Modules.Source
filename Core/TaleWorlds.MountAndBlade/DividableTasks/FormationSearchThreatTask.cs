using System;

namespace TaleWorlds.MountAndBlade.DividableTasks
{
	// Token: 0x02000410 RID: 1040
	public class FormationSearchThreatTask : DividableTask
	{
		// Token: 0x06003596 RID: 13718 RVA: 0x000DE7D8 File Offset: 0x000DC9D8
		protected override bool UpdateExtra()
		{
			this._result = this._formation.HasUnitWithConditionLimitedRandom((Agent agent) => this._weapon.CanShootAtAgent(agent), this._storedIndex, this._checkCountPerTick, out this._targetAgent);
			this._storedIndex += this._checkCountPerTick;
			return this._storedIndex >= this._formation.CountOfUnits || this._result;
		}

		// Token: 0x06003597 RID: 13719 RVA: 0x000DE842 File Offset: 0x000DCA42
		public void Prepare(Formation formation, RangedSiegeWeapon weapon)
		{
			base.ResetTaskStatus();
			this._formation = formation;
			this._weapon = weapon;
			this._storedIndex = 0;
			this._checkCountPerTick = (int)((float)this._formation.CountOfUnits * 0.1f) + 1;
		}

		// Token: 0x06003598 RID: 13720 RVA: 0x000DE87A File Offset: 0x000DCA7A
		public bool GetResult(out Agent targetAgent)
		{
			targetAgent = this._targetAgent;
			return this._result;
		}

		// Token: 0x06003599 RID: 13721 RVA: 0x000DE88A File Offset: 0x000DCA8A
		public FormationSearchThreatTask()
			: base(null)
		{
		}

		// Token: 0x040016F2 RID: 5874
		private Agent _targetAgent;

		// Token: 0x040016F3 RID: 5875
		private const float CheckCountRatio = 0.1f;

		// Token: 0x040016F4 RID: 5876
		private RangedSiegeWeapon _weapon;

		// Token: 0x040016F5 RID: 5877
		private Formation _formation;

		// Token: 0x040016F6 RID: 5878
		private int _storedIndex;

		// Token: 0x040016F7 RID: 5879
		private int _checkCountPerTick;

		// Token: 0x040016F8 RID: 5880
		private bool _result;
	}
}
