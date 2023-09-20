using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionAgentLockVisualizerVM : ViewModel
	{
		public MissionAgentLockVisualizerVM()
		{
			this._allTrackedAgentsSet = new Dictionary<Agent, MissionAgentLockItemVM>();
			this.AllTrackedAgents = new MBBindingList<MissionAgentLockItemVM>();
			this.IsEnabled = true;
		}

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

		private readonly Dictionary<Agent, MissionAgentLockItemVM> _allTrackedAgentsSet;

		private MBBindingList<MissionAgentLockItemVM> _allTrackedAgents;

		private bool _isEnabled;
	}
}
