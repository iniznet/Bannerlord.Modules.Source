using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x0200008C RID: 140
	public class SettlementBuildingProjectVM : SettlementProjectVM
	{
		// Token: 0x06000DBE RID: 3518 RVA: 0x000377B0 File Offset: 0x000359B0
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

		// Token: 0x06000DBF RID: 3519 RVA: 0x00037814 File Offset: 0x00035A14
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AlreadyAtMaxText = new TextObject("{=ybLA7ZXp}Already at Max", null).ToString();
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x00037834 File Offset: 0x00035A34
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

		// Token: 0x06000DC1 RID: 3521 RVA: 0x00037945 File Offset: 0x00035B45
		public override void ExecuteAddToQueue()
		{
			if (this._onSelection != null && this.CanBuild)
			{
				this._onSelection(this, false);
			}
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x00037964 File Offset: 0x00035B64
		public override void ExecuteSetAsActiveDevelopment()
		{
			if (this._onSelection != null && this.CanBuild)
			{
				this._onSelection(this, true);
			}
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x00037983 File Offset: 0x00035B83
		public override void ExecuteSetAsCurrent()
		{
			Action<SettlementProjectVM> onSetAsCurrent = this._onSetAsCurrent;
			if (onSetAsCurrent == null)
			{
				return;
			}
			onSetAsCurrent(this);
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x00037996 File Offset: 0x00035B96
		public override void ExecuteResetCurrent()
		{
			Action onResetCurrent = this._onResetCurrent;
			if (onResetCurrent == null)
			{
				return;
			}
			onResetCurrent();
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06000DC5 RID: 3525 RVA: 0x000379A8 File Offset: 0x00035BA8
		// (set) Token: 0x06000DC6 RID: 3526 RVA: 0x000379B0 File Offset: 0x00035BB0
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

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x000379CE File Offset: 0x00035BCE
		// (set) Token: 0x06000DC8 RID: 3528 RVA: 0x000379D6 File Offset: 0x00035BD6
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

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x000379F9 File Offset: 0x00035BF9
		// (set) Token: 0x06000DCA RID: 3530 RVA: 0x00037A01 File Offset: 0x00035C01
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

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06000DCB RID: 3531 RVA: 0x00037A1F File Offset: 0x00035C1F
		// (set) Token: 0x06000DCC RID: 3532 RVA: 0x00037A27 File Offset: 0x00035C27
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

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x00037A45 File Offset: 0x00035C45
		// (set) Token: 0x06000DCE RID: 3534 RVA: 0x00037A4D File Offset: 0x00035C4D
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

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00037A6B File Offset: 0x00035C6B
		// (set) Token: 0x06000DD0 RID: 3536 RVA: 0x00037A73 File Offset: 0x00035C73
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

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x00037A91 File Offset: 0x00035C91
		// (set) Token: 0x06000DD2 RID: 3538 RVA: 0x00037A99 File Offset: 0x00035C99
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

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x00037ABC File Offset: 0x00035CBC
		// (set) Token: 0x06000DD4 RID: 3540 RVA: 0x00037AC4 File Offset: 0x00035CC4
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

		// Token: 0x04000665 RID: 1637
		private bool _isSelected;

		// Token: 0x04000666 RID: 1638
		private string _alreadyAtMaxText;

		// Token: 0x04000667 RID: 1639
		private string _developmentLevelText;

		// Token: 0x04000668 RID: 1640
		private int _level;

		// Token: 0x04000669 RID: 1641
		private int _maxLevel;

		// Token: 0x0400066A RID: 1642
		private int _developmentQueueIndex = -1;

		// Token: 0x0400066B RID: 1643
		private bool _canBuild;

		// Token: 0x0400066C RID: 1644
		private bool _isInQueue;
	}
}
