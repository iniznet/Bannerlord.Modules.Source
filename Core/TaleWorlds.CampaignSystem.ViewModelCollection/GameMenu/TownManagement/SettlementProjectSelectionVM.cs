using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class SettlementProjectSelectionVM : ViewModel
	{
		public List<Building> LocalDevelopmentList { get; private set; }

		public SettlementProjectSelectionVM(Settlement settlement, Action onAnyChangeInQueue)
		{
			this._settlement = settlement;
			this._town = ((settlement != null) ? settlement.Town : null);
			this._onAnyChangeInQueue = onAnyChangeInQueue;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ProjectsText = new TextObject("{=LpsoPtOo}Projects", null).ToString();
			this.DailyDefaultsText = GameTexts.FindText("str_town_management_daily_defaults", null).ToString();
			this.DailyDefaultsExplanationText = GameTexts.FindText("str_town_management_daily_defaults_explanation", null).ToString();
			this.QueueText = GameTexts.FindText("str_town_management_queue", null).ToString();
			this.Refresh();
		}

		public void Refresh()
		{
			this.AvailableProjects = new MBBindingList<SettlementBuildingProjectVM>();
			this.DailyDefaultList = new MBBindingList<SettlementDailyProjectVM>();
			this.LocalDevelopmentList = new List<Building>();
			this.CurrentDevelopmentQueue = new MBBindingList<SettlementBuildingProjectVM>();
			this.AvailableProjects.Clear();
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.Town : null) != null)
			{
				foreach (Building building in Settlement.CurrentSettlement.Town.Buildings.Where((Building x) => x.BuildingType.BuildingLocation != BuildingLocation.Daily))
				{
					SettlementBuildingProjectVM settlementBuildingProjectVM = new SettlementBuildingProjectVM(new Action<SettlementProjectVM, bool>(this.OnCurrentProjectSelection), new Action<SettlementProjectVM>(this.OnCurrentProjectSet), new Action(this.OnResetCurrentProject), building);
					this.AvailableProjects.Add(settlementBuildingProjectVM);
					if (settlementBuildingProjectVM.Building == Settlement.CurrentSettlement.Town.CurrentBuilding)
					{
						this.CurrentSelectedProject = settlementBuildingProjectVM;
					}
				}
				if (Settlement.CurrentSettlement != null)
				{
					foreach (Building building2 in Settlement.CurrentSettlement.Town.Buildings.Where((Building x) => x.BuildingType.BuildingLocation == BuildingLocation.Daily))
					{
						SettlementDailyProjectVM settlementDailyProjectVM = new SettlementDailyProjectVM(new Action<SettlementProjectVM, bool>(this.OnCurrentProjectSelection), new Action<SettlementProjectVM>(this.OnCurrentProjectSet), new Action(this.OnResetCurrentProject), building2);
						this.DailyDefaultList.Add(settlementDailyProjectVM);
						if (settlementDailyProjectVM.Building == Settlement.CurrentSettlement.Town.CurrentDefaultBuilding)
						{
							this.CurrentDailyDefault = settlementDailyProjectVM;
							this.CurrentDailyDefault.IsDefault = true;
							settlementDailyProjectVM.IsDefault = true;
						}
					}
				}
				foreach (Building building3 in this._town.BuildingsInProgress)
				{
					this.LocalDevelopmentList.Add(building3);
				}
				this.RefreshDevelopmentsQueueIndex();
			}
		}

		private void OnCurrentProjectSet(SettlementProjectVM selectedItem)
		{
			if (selectedItem != this.CurrentSelectedProject)
			{
				this.CurrentSelectedProject = selectedItem;
				this.CurrentSelectedProject.RefreshProductionText();
			}
		}

		private void OnCurrentProjectSelection(SettlementProjectVM selectedItem, bool isSetAsActiveDevelopment)
		{
			if (!selectedItem.IsDaily)
			{
				if (isSetAsActiveDevelopment)
				{
					this.LocalDevelopmentList.Clear();
					this.LocalDevelopmentList.Add(selectedItem.Building);
				}
				else if (this.LocalDevelopmentList.Exists((Building d) => d == selectedItem.Building))
				{
					this.LocalDevelopmentList.Remove(selectedItem.Building);
				}
				else
				{
					this.LocalDevelopmentList.Add(selectedItem.Building);
				}
			}
			else
			{
				this.CurrentDailyDefault.IsDefault = false;
				this.CurrentDailyDefault = selectedItem as SettlementDailyProjectVM;
				(selectedItem as SettlementDailyProjectVM).IsDefault = true;
			}
			this.RefreshDevelopmentsQueueIndex();
			if (this.LocalDevelopmentList.Count == 0)
			{
				this.CurrentSelectedProject = this.CurrentDailyDefault;
			}
			else if (selectedItem != this.CurrentSelectedProject)
			{
				this.CurrentSelectedProject = selectedItem;
			}
			Action onAnyChangeInQueue = this._onAnyChangeInQueue;
			if (onAnyChangeInQueue == null)
			{
				return;
			}
			onAnyChangeInQueue();
		}

		private void OnResetCurrentProject()
		{
			this.CurrentSelectedProject = ((this.LocalDevelopmentList.Count > 0) ? this.AvailableProjects.First((SettlementBuildingProjectVM p) => p.Building == this.LocalDevelopmentList[0]) : this.CurrentDailyDefault);
			this.CurrentSelectedProject.RefreshProductionText();
		}

		private void RefreshDevelopmentsQueueIndex()
		{
			this.CurrentSelectedProject = null;
			this.CurrentDevelopmentQueue = new MBBindingList<SettlementBuildingProjectVM>();
			using (IEnumerator<SettlementBuildingProjectVM> enumerator = this.AvailableProjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SettlementBuildingProjectVM item = enumerator.Current;
					item.DevelopmentQueueIndex = -1;
					item.IsInQueue = this.LocalDevelopmentList.Any((Building d) => d.BuildingType == item.Building.BuildingType);
					item.IsCurrentActiveProject = false;
					if (item.IsInQueue)
					{
						int num = this.LocalDevelopmentList.IndexOf(item.Building);
						item.DevelopmentQueueIndex = num;
						if (num == 0)
						{
							this.CurrentSelectedProject = item;
							item.IsCurrentActiveProject = true;
						}
						this.CurrentDevelopmentQueue.Add(item);
					}
					Comparer<SettlementBuildingProjectVM> comparer = Comparer<SettlementBuildingProjectVM>.Create((SettlementBuildingProjectVM s1, SettlementBuildingProjectVM s2) => s1.DevelopmentQueueIndex.CompareTo(s2.DevelopmentQueueIndex));
					this.CurrentDevelopmentQueue.Sort(comparer);
					item.RefreshProductionText();
				}
			}
		}

		[DataSourceProperty]
		public string ProjectsText
		{
			get
			{
				return this._projectsText;
			}
			set
			{
				if (value != this._projectsText)
				{
					this._projectsText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProjectsText");
				}
			}
		}

		[DataSourceProperty]
		public string QueueText
		{
			get
			{
				return this._queueText;
			}
			set
			{
				if (value != this._queueText)
				{
					this._queueText = value;
					base.OnPropertyChangedWithValue<string>(value, "QueueText");
				}
			}
		}

		[DataSourceProperty]
		public string DailyDefaultsText
		{
			get
			{
				return this._dailyDefaultsText;
			}
			set
			{
				if (value != this._dailyDefaultsText)
				{
					this._dailyDefaultsText = value;
					base.OnPropertyChangedWithValue<string>(value, "DailyDefaultsText");
				}
			}
		}

		[DataSourceProperty]
		public string DailyDefaultsExplanationText
		{
			get
			{
				return this._dailyDefaultsExplanationText;
			}
			set
			{
				if (value != this._dailyDefaultsExplanationText)
				{
					this._dailyDefaultsExplanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "DailyDefaultsExplanationText");
				}
			}
		}

		[DataSourceProperty]
		public SettlementProjectVM CurrentSelectedProject
		{
			get
			{
				return this._currentSelectedProject;
			}
			set
			{
				if (value != this._currentSelectedProject)
				{
					this._currentSelectedProject = value;
					base.OnPropertyChangedWithValue<SettlementProjectVM>(value, "CurrentSelectedProject");
				}
			}
		}

		[DataSourceProperty]
		public SettlementDailyProjectVM CurrentDailyDefault
		{
			get
			{
				return this._currentDailyDefault;
			}
			set
			{
				if (value != this._currentDailyDefault)
				{
					this._currentDailyDefault = value;
					base.OnPropertyChangedWithValue<SettlementDailyProjectVM>(value, "CurrentDailyDefault");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SettlementBuildingProjectVM> AvailableProjects
		{
			get
			{
				return this._availableProjects;
			}
			set
			{
				if (value != this._availableProjects)
				{
					this._availableProjects = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementBuildingProjectVM>>(value, "AvailableProjects");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SettlementBuildingProjectVM> CurrentDevelopmentQueue
		{
			get
			{
				return this._currentDevelopmentQueue;
			}
			set
			{
				if (value != this._currentDevelopmentQueue)
				{
					this._currentDevelopmentQueue = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementBuildingProjectVM>>(value, "CurrentDevelopmentQueue");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SettlementDailyProjectVM> DailyDefaultList
		{
			get
			{
				return this._dailyDefaultList;
			}
			set
			{
				if (value != this._dailyDefaultList)
				{
					this._dailyDefaultList = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementDailyProjectVM>>(value, "DailyDefaultList");
				}
			}
		}

		private readonly Town _town;

		private readonly Settlement _settlement;

		private readonly Action _onAnyChangeInQueue;

		private SettlementDailyProjectVM _currentDailyDefault;

		private SettlementProjectVM _currentSelectedProject;

		private MBBindingList<SettlementDailyProjectVM> _dailyDefaultList;

		private MBBindingList<SettlementBuildingProjectVM> _currentDevelopmentQueue;

		private MBBindingList<SettlementBuildingProjectVM> _availableProjects;

		private string _projectsText;

		private string _queueText;

		private string _dailyDefaultsText;

		private string _dailyDefaultsExplanationText;
	}
}
