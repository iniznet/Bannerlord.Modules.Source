using System;

namespace TaleWorlds.MountAndBlade.DividableTasks
{
	public class FormationSearchThreatTask : DividableTask
	{
		protected override bool UpdateExtra()
		{
			this._result = this._formation.HasUnitWithConditionLimitedRandom((Agent agent) => this._weapon.CanShootAtAgent(agent), this._storedIndex, this._checkCountPerTick, out this._targetAgent);
			this._storedIndex += this._checkCountPerTick;
			return this._storedIndex >= this._formation.CountOfUnits || this._result;
		}

		public void Prepare(Formation formation, RangedSiegeWeapon weapon)
		{
			base.ResetTaskStatus();
			this._formation = formation;
			this._weapon = weapon;
			this._storedIndex = 0;
			this._checkCountPerTick = (int)((float)this._formation.CountOfUnits * 0.1f) + 1;
		}

		public bool GetResult(out Agent targetAgent)
		{
			targetAgent = this._targetAgent;
			return this._result;
		}

		public FormationSearchThreatTask()
			: base(null)
		{
		}

		private Agent _targetAgent;

		private const float CheckCountRatio = 0.1f;

		private RangedSiegeWeapon _weapon;

		private Formation _formation;

		private int _storedIndex;

		private int _checkCountPerTick;

		private bool _result;
	}
}
