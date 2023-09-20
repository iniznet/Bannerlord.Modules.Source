using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class SettlementBuildingProjectVM : SettlementProjectVM
	{
		public SettlementBuildingProjectVM(Action<SettlementProjectVM, bool> onSelection, Action<SettlementProjectVM> onSetAsCurrent, Action onResetCurrent, Building building)
			: base(onSelection, onSetAsCurrent, onResetCurrent, building)
		{
			this.Level = building.CurrentLevel;
			this.MaxLevel = 3;
			this.DevelopmentLevelText = building.CurrentLevel.ToString();
			this.CanBuild = this.Level < 3;
			base.IsDaily = false;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AlreadyAtMaxText = new TextObject("{=ybLA7ZXp}Already at Max", null).ToString();
		}

		public override void RefreshProductionText()
		{
			base.RefreshProductionText();
			if (this.DevelopmentQueueIndex == 0)
			{
				GameTexts.SetVariable("LEFT", GameTexts.FindText("str_completion", null));
				TextObject textObject = TextObject.Empty;
				int daysToComplete = BuildingHelper.GetDaysToComplete(base.Building, Settlement.CurrentSettlement.Town);
				if (daysToComplete != -1)
				{
					textObject = new TextObject("{=c5eYzHaM}{DAYS} {?DAY_IS_PLURAL}Days{?}Day{\\?} ({PERCENTAGE}%)", null);
					textObject.SetTextVariable("DAYS", daysToComplete);
					GameTexts.SetVariable("DAY_IS_PLURAL", (daysToComplete > 1) ? 1 : 0);
				}
				else
				{
					textObject = new TextObject("{=0TauthlH}Never ({PERCENTAGE}%)", null);
				}
				textObject.SetTextVariable("PERCENTAGE", (int)(BuildingHelper.GetProgressOfBuilding(base.Building, Settlement.CurrentSettlement.Town) * 100f));
				GameTexts.SetVariable("RIGHT", textObject);
				base.ProductionText = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
				return;
			}
			if (this.DevelopmentQueueIndex > 0)
			{
				GameTexts.SetVariable("NUMBER", this.DevelopmentQueueIndex);
				base.ProductionText = GameTexts.FindText("str_in_queue_with_number", null).ToString();
				return;
			}
			base.ProductionText = " ";
		}

		public override void ExecuteAddToQueue()
		{
			if (this._onSelection != null && this.CanBuild)
			{
				this._onSelection(this, false);
			}
		}

		public override void ExecuteSetAsActiveDevelopment()
		{
			if (this._onSelection != null && this.CanBuild)
			{
				this._onSelection(this, true);
			}
		}

		public override void ExecuteSetAsCurrent()
		{
			Action<SettlementProjectVM> onSetAsCurrent = this._onSetAsCurrent;
			if (onSetAsCurrent == null)
			{
				return;
			}
			onSetAsCurrent(this);
		}

		public override void ExecuteResetCurrent()
		{
			Action onResetCurrent = this._onResetCurrent;
			if (onResetCurrent == null)
			{
				return;
			}
			onResetCurrent();
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string DevelopmentLevelText
		{
			get
			{
				return this._developmentLevelText;
			}
			set
			{
				if (value != this._developmentLevelText)
				{
					this._developmentLevelText = value;
					base.OnPropertyChangedWithValue<string>(value, "DevelopmentLevelText");
				}
			}
		}

		[DataSourceProperty]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue(value, "Level");
				}
			}
		}

		[DataSourceProperty]
		public int MaxLevel
		{
			get
			{
				return this._maxLevel;
			}
			set
			{
				if (value != this._maxLevel)
				{
					this._maxLevel = value;
					base.OnPropertyChangedWithValue(value, "MaxLevel");
				}
			}
		}

		[DataSourceProperty]
		public int DevelopmentQueueIndex
		{
			get
			{
				return this._developmentQueueIndex;
			}
			set
			{
				if (value != this._developmentQueueIndex)
				{
					this._developmentQueueIndex = value;
					base.OnPropertyChangedWithValue(value, "DevelopmentQueueIndex");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInQueue
		{
			get
			{
				return this._isInQueue;
			}
			set
			{
				if (value != this._isInQueue)
				{
					this._isInQueue = value;
					base.OnPropertyChangedWithValue(value, "IsInQueue");
				}
			}
		}

		[DataSourceProperty]
		public string AlreadyAtMaxText
		{
			get
			{
				return this._alreadyAtMaxText;
			}
			set
			{
				if (value != this._alreadyAtMaxText)
				{
					this._alreadyAtMaxText = value;
					base.OnPropertyChangedWithValue<string>(value, "AlreadyAtMaxText");
				}
			}
		}

		[DataSourceProperty]
		public bool CanBuild
		{
			get
			{
				return this._canBuild;
			}
			set
			{
				if (value != this._canBuild)
				{
					this._canBuild = value;
					base.OnPropertyChangedWithValue(value, "CanBuild");
				}
			}
		}

		private bool _isSelected;

		private string _alreadyAtMaxText;

		private string _developmentLevelText;

		private int _level;

		private int _maxLevel;

		private int _developmentQueueIndex = -1;

		private bool _canBuild;

		private bool _isInQueue;
	}
}
