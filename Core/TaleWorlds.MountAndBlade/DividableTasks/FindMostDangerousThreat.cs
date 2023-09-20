using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.DividableTasks
{
	public class FindMostDangerousThreat : DividableTask
	{
		public FindMostDangerousThreat(DividableTask continueToTask = null)
			: base(continueToTask)
		{
			base.SetTaskFinished(false);
			this._formationSearchThreatTask = new FormationSearchThreatTask();
		}

		protected override bool UpdateExtra()
		{
			bool flag = false;
			if (this._hasOngoingThreatTask)
			{
				if (this._formationSearchThreatTask.Update())
				{
					this._hasOngoingThreatTask = false;
					if (!(flag = this._formationSearchThreatTask.GetResult(out this._targetAgent)))
					{
						this._threats.Remove(this._currentThreat);
						this._currentThreat = null;
					}
				}
			}
			else
			{
				for (;;)
				{
					flag = true;
					int num = -1;
					float num2 = float.MinValue;
					for (int i = 0; i < this._threats.Count; i++)
					{
						Threat threat = this._threats[i];
						if (threat.ThreatValue > num2)
						{
							num2 = threat.ThreatValue;
							num = i;
						}
					}
					if (num >= 0)
					{
						this._currentThreat = this._threats[num];
						if (this._currentThreat.Formation != null)
						{
							break;
						}
						if ((this._currentThreat.WeaponEntity == null && this._currentThreat.Agent == null) || !this._weapon.CanShootAtThreat(this._currentThreat))
						{
							this._currentThreat = null;
							this._threats.RemoveAt(num);
							flag = false;
						}
					}
					if (flag)
					{
						goto IL_12E;
					}
				}
				this._formationSearchThreatTask.Prepare(this._currentThreat.Formation, this._weapon);
				this._hasOngoingThreatTask = true;
				flag = false;
			}
			IL_12E:
			return flag || this._threats.Count == 0;
		}

		public void Prepare(List<Threat> threats, RangedSiegeWeapon weapon)
		{
			base.ResetTaskStatus();
			this._hasOngoingThreatTask = false;
			this._weapon = weapon;
			this._threats = threats;
			foreach (Threat threat in this._threats)
			{
				threat.ThreatValue *= 0.9f + MBRandom.RandomFloat * 0.2f;
			}
			if (this._currentThreat != null)
			{
				this._currentThreat = this._threats.SingleOrDefault((Threat t) => t.Equals(this._currentThreat));
				if (this._currentThreat != null)
				{
					this._currentThreat.ThreatValue *= 2f;
				}
			}
		}

		public Threat GetResult(out Agent targetAgent)
		{
			targetAgent = this._targetAgent;
			return this._currentThreat;
		}

		private Agent _targetAgent;

		private FormationSearchThreatTask _formationSearchThreatTask;

		private List<Threat> _threats;

		private RangedSiegeWeapon _weapon;

		private Threat _currentThreat;

		private bool _hasOngoingThreatTask;
	}
}
