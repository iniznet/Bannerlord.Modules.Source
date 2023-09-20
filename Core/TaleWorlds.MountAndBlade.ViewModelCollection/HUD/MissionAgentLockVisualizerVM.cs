using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000D9 RID: 217
	public class MissionAgentLockVisualizerVM : ViewModel
	{
		// Token: 0x06001420 RID: 5152 RVA: 0x00041D15 File Offset: 0x0003FF15
		public MissionAgentLockVisualizerVM()
		{
			this._allTrackedAgentsSet = new Dictionary<Agent, MissionAgentLockItemVM>();
			this.AllTrackedAgents = new MBBindingList<MissionAgentLockItemVM>();
			this.IsEnabled = true;
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x00041D3C File Offset: 0x0003FF3C
		public void OnActiveLockAgentChange(Agent oldAgent, Agent newAgent)
		{
			if (oldAgent != null && this._allTrackedAgentsSet.ContainsKey(oldAgent))
			{
				this.AllTrackedAgents.Remove(this._allTrackedAgentsSet[oldAgent]);
				this._allTrackedAgentsSet.Remove(oldAgent);
			}
			if (newAgent != null)
			{
				if (this._allTrackedAgentsSet.ContainsKey(newAgent))
				{
					this._allTrackedAgentsSet[newAgent].SetLockState(MissionAgentLockItemVM.LockStates.Active);
					return;
				}
				MissionAgentLockItemVM missionAgentLockItemVM = new MissionAgentLockItemVM(newAgent, MissionAgentLockItemVM.LockStates.Active);
				this._allTrackedAgentsSet.Add(newAgent, missionAgentLockItemVM);
				this.AllTrackedAgents.Add(missionAgentLockItemVM);
			}
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x00041DC4 File Offset: 0x0003FFC4
		public void OnPossibleLockAgentChange(Agent oldPossibleAgent, Agent newPossibleAgent)
		{
			if (oldPossibleAgent != null && this._allTrackedAgentsSet.ContainsKey(oldPossibleAgent))
			{
				this.AllTrackedAgents.Remove(this._allTrackedAgentsSet[oldPossibleAgent]);
				this._allTrackedAgentsSet.Remove(oldPossibleAgent);
			}
			if (newPossibleAgent != null)
			{
				if (this._allTrackedAgentsSet.ContainsKey(newPossibleAgent))
				{
					this._allTrackedAgentsSet[newPossibleAgent].SetLockState(MissionAgentLockItemVM.LockStates.Possible);
					return;
				}
				MissionAgentLockItemVM missionAgentLockItemVM = new MissionAgentLockItemVM(newPossibleAgent, MissionAgentLockItemVM.LockStates.Possible);
				this._allTrackedAgentsSet.Add(newPossibleAgent, missionAgentLockItemVM);
				this.AllTrackedAgents.Add(missionAgentLockItemVM);
			}
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x00041E4C File Offset: 0x0004004C
		// (set) Token: 0x06001424 RID: 5156 RVA: 0x00041E54 File Offset: 0x00040054
		[DataSourceProperty]
		public MBBindingList<MissionAgentLockItemVM> AllTrackedAgents
		{
			get
			{
				return this._allTrackedAgents;
			}
			set
			{
				if (value != this._allTrackedAgents)
				{
					this._allTrackedAgents = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionAgentLockItemVM>>(value, "AllTrackedAgents");
				}
			}
		}

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x00041E72 File Offset: 0x00040072
		// (set) Token: 0x06001426 RID: 5158 RVA: 0x00041E7A File Offset: 0x0004007A
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					if (!value)
					{
						this.AllTrackedAgents.Clear();
						this._allTrackedAgentsSet.Clear();
					}
				}
			}
		}

		// Token: 0x040009A3 RID: 2467
		private readonly Dictionary<Agent, MissionAgentLockItemVM> _allTrackedAgentsSet;

		// Token: 0x040009A4 RID: 2468
		private MBBindingList<MissionAgentLockItemVM> _allTrackedAgents;

		// Token: 0x040009A5 RID: 2469
		private bool _isEnabled;
	}
}
