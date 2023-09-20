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
	// Token: 0x02000090 RID: 144
	public class SettlementProjectSelectionVM : ViewModel
	{
		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x00038017 File Offset: 0x00036217
		// (set) Token: 0x06000DF5 RID: 3573 RVA: 0x0003801F File Offset: 0x0003621F
		public List<Building> LocalDevelopmentList { get; private set; }

		// Token: 0x06000DF6 RID: 3574 RVA: 0x00038028 File Offset: 0x00036228
		public SettlementProjectSelectionVM(Settlement settlement, Action onAnyChangeInQueue)
		{
			this._settlement = settlement;
			this._town = ((settlement != null) ? settlement.Town : null);
			this._onAnyChangeInQueue = onAnyChangeInQueue;
			this.RefreshValues();
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x00038058 File Offset: 0x00036258
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ProjectsText = new TextObject("{=LpsoPtOo}Projects", null).ToString();
			this.DailyDefaultsText = GameTexts.FindText("str_town_management_daily_defaults", null).ToString();
			this.QueueText = GameTexts.FindText("str_town_management_queue", null).ToString();
			this.Refresh();
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x000380B4 File Offset: 0x000362B4
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

		// Token: 0x06000DF9 RID: 3577 RVA: 0x000382F8 File Offset: 0x000364F8
		private void OnCurrentProjectSet(SettlementProjectVM selectedItem)
		{
			if (selectedItem != this.CurrentSelectedProject)
			{
				this.CurrentSelectedProject = selectedItem;
				this.CurrentSelectedProject.RefreshProductionText();
			}
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00038318 File Offset: 0x00036518
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

		// Token: 0x06000DFB RID: 3579 RVA: 0x00038428 File Offset: 0x00036628
		private void OnResetCurrentProject()
		{
			this.CurrentSelectedProject = ((this.LocalDevelopmentList.Count > 0) ? this.AvailableProjects.First((SettlementBuildingProjectVM p) => p.Building == this.LocalDevelopmentList[0]) : this.CurrentDailyDefault);
			this.CurrentSelectedProject.RefreshProductionText();
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00038468 File Offset: 0x00036668
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

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06000DFD RID: 3581 RVA: 0x000385A4 File Offset: 0x000367A4
		// (set) Token: 0x06000DFE RID: 3582 RVA: 0x000385AC File Offset: 0x000367AC
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

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06000DFF RID: 3583 RVA: 0x000385CF File Offset: 0x000367CF
		// (set) Token: 0x06000E00 RID: 3584 RVA: 0x000385D7 File Offset: 0x000367D7
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

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06000E01 RID: 3585 RVA: 0x000385FA File Offset: 0x000367FA
		// (set) Token: 0x06000E02 RID: 3586 RVA: 0x00038602 File Offset: 0x00036802
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

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06000E03 RID: 3587 RVA: 0x00038625 File Offset: 0x00036825
		// (set) Token: 0x06000E04 RID: 3588 RVA: 0x0003862D File Offset: 0x0003682D
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

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0003864B File Offset: 0x0003684B
		// (set) Token: 0x06000E06 RID: 3590 RVA: 0x00038653 File Offset: 0x00036853
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

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06000E07 RID: 3591 RVA: 0x00038671 File Offset: 0x00036871
		// (set) Token: 0x06000E08 RID: 3592 RVA: 0x00038679 File Offset: 0x00036879
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

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x00038697 File Offset: 0x00036897
		// (set) Token: 0x06000E0A RID: 3594 RVA: 0x0003869F File Offset: 0x0003689F
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

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000386BD File Offset: 0x000368BD
		// (set) Token: 0x06000E0C RID: 3596 RVA: 0x000386C5 File Offset: 0x000368C5
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

		// Token: 0x04000678 RID: 1656
		private readonly Town _town;

		// Token: 0x04000679 RID: 1657
		private readonly Settlement _settlement;

		// Token: 0x0400067A RID: 1658
		private readonly Action _onAnyChangeInQueue;

		// Token: 0x0400067C RID: 1660
		private SettlementDailyProjectVM _currentDailyDefault;

		// Token: 0x0400067D RID: 1661
		private SettlementProjectVM _currentSelectedProject;

		// Token: 0x0400067E RID: 1662
		private MBBindingList<SettlementDailyProjectVM> _dailyDefaultList;

		// Token: 0x0400067F RID: 1663
		private MBBindingList<SettlementBuildingProjectVM> _currentDevelopmentQueue;

		// Token: 0x04000680 RID: 1664
		private MBBindingList<SettlementBuildingProjectVM> _availableProjects;

		// Token: 0x04000681 RID: 1665
		private string _projectsText;

		// Token: 0x04000682 RID: 1666
		private string _queueText;

		// Token: 0x04000683 RID: 1667
		private string _dailyDefaultsText;
	}
}
