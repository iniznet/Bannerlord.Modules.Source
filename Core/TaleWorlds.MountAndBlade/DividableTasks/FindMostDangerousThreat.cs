using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.DividableTasks
{
	// Token: 0x0200040F RID: 1039
	public class FindMostDangerousThreat : DividableTask
	{
		// Token: 0x06003591 RID: 13713 RVA: 0x000DE582 File Offset: 0x000DC782
		public FindMostDangerousThreat(DividableTask continueToTask = null)
			: base(continueToTask)
		{
			base.SetTaskFinished(false);
			this._formationSearchThreatTask = new FormationSearchThreatTask();
		}

		// Token: 0x06003592 RID: 13714 RVA: 0x000DE5A0 File Offset: 0x000DC7A0
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

		// Token: 0x06003593 RID: 13715 RVA: 0x000DE6F0 File Offset: 0x000DC8F0
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

		// Token: 0x06003594 RID: 13716 RVA: 0x000DE7B8 File Offset: 0x000DC9B8
		public Threat GetResult(out Agent targetAgent)
		{
			targetAgent = this._targetAgent;
			return this._currentThreat;
		}

		// Token: 0x040016EC RID: 5868
		private Agent _targetAgent;

		// Token: 0x040016ED RID: 5869
		private FormationSearchThreatTask _formationSearchThreatTask;

		// Token: 0x040016EE RID: 5870
		private List<Threat> _threats;

		// Token: 0x040016EF RID: 5871
		private RangedSiegeWeapon _weapon;

		// Token: 0x040016F0 RID: 5872
		private Threat _currentThreat;

		// Token: 0x040016F1 RID: 5873
		private bool _hasOngoingThreatTask;
	}
}
