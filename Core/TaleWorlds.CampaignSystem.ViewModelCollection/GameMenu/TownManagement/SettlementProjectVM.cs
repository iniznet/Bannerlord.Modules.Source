using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public abstract class SettlementProjectVM : ViewModel
	{
		public bool IsDaily { get; protected set; }

		public Building Building
		{
			get
			{
				return this._building;
			}
			set
			{
				this._building = value;
				this.Name = ((value != null) ? value.Name.ToString() : "");
				this.Explanation = ((value != null) ? value.Explanation.ToString() : "");
				this.VisualCode = ((value != null) ? value.BuildingType.StringId.ToLower() : "");
				int constructionCost = this.Building.GetConstructionCost();
				TextObject textObject;
				if (constructionCost > 0)
				{
					textObject = new TextObject("{=tAwRIPiy}Construction Cost: {COST}", null);
					textObject.SetTextVariable("COST", constructionCost);
				}
				else
				{
					textObject = TextObject.Empty;
				}
				this.ProductionCostText = ((value != null) ? textObject.ToString() : "");
				this.CurrentPositiveEffectText = ((value != null) ? value.GetBonusExplanation().ToString() : "");
			}
		}

		protected SettlementProjectVM(Action<SettlementProjectVM, bool> onSelection, Action<SettlementProjectVM> onSetAsCurrent, Action onResetCurrent, Building building)
		{
			this._onSelection = onSelection;
			this._onSetAsCurrent = onSetAsCurrent;
			this._onResetCurrent = onResetCurrent;
			this.Building = building;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.Town : null) != null)
			{
				this.Progress = (int)(BuildingHelper.GetProgressOfBuilding(building, Settlement.CurrentSettlement.Town) * 100f);
			}
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Building.BuildingType.IsDefaultProject)
			{
				this.CurrentPositiveEffectText = this.Building.BuildingType.GetExplanationAtLevel(this.Building.CurrentLevel).ToString();
				this.NextPositiveEffectText = "";
				return;
			}
			this.CurrentPositiveEffectText = this.GetBonusText(this.Building, this.Building.CurrentLevel);
			this.NextPositiveEffectText = this.GetBonusText(this.Building, this.Building.CurrentLevel + 1);
		}

		private string GetBonusText(Building building, int level)
		{
			if (level == 0 || level == 4)
			{
				return "";
			}
			object obj = ((level == 1) ? this.L1BonusText : ((level == 2) ? this.L2BonusText : this.L3BonusText));
			TextObject bonusExplanationOfLevel = this.GetBonusExplanationOfLevel(level);
			object obj2 = obj;
			obj2.SetTextVariable("BONUS", bonusExplanationOfLevel);
			return obj2.ToString();
		}

		private void ExecuteShowTooltip()
		{
			InformationManager.ShowTooltip(typeof(Building), new object[] { this._building });
		}

		private void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		private TextObject GetBonusExplanationOfLevel(int level)
		{
			if (level >= 0 && level <= 3)
			{
				return this.Building.BuildingType.GetExplanationAtLevel(level);
			}
			return TextObject.Empty;
		}

		public virtual void RefreshProductionText()
		{
		}

		public abstract void ExecuteAddToQueue();

		public abstract void ExecuteSetAsActiveDevelopment();

		public abstract void ExecuteSetAsCurrent();

		public abstract void ExecuteResetCurrent();

		[DataSourceProperty]
		public string VisualCode
		{
			get
			{
				return this._visualCode;
			}
			set
			{
				if (value != this._visualCode)
				{
					this._visualCode = value;
					base.OnPropertyChangedWithValue<string>(value, "VisualCode");
				}
			}
		}

		[DataSourceProperty]
		public string ProductionText
		{
			get
			{
				return this._productionText;
			}
			set
			{
				if (value != this._productionText)
				{
					this._productionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProductionText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentPositiveEffectText
		{
			get
			{
				return this._currentPositiveEffectText;
			}
			set
			{
				if (value != this._currentPositiveEffectText)
				{
					this._currentPositiveEffectText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentPositiveEffectText");
				}
			}
		}

		[DataSourceProperty]
		public string NextPositiveEffectText
		{
			get
			{
				return this._nextPositiveEffectText;
			}
			set
			{
				if (value != this._nextPositiveEffectText)
				{
					this._nextPositiveEffectText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextPositiveEffectText");
				}
			}
		}

		[DataSourceProperty]
		public string ProductionCostText
		{
			get
			{
				return this._productionCostText;
			}
			set
			{
				if (value != this._productionCostText)
				{
					this._productionCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProductionCostText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCurrentActiveProject
		{
			get
			{
				return this._isCurrentActiveProject;
			}
			set
			{
				if (value != this._isCurrentActiveProject)
				{
					this._isCurrentActiveProject = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentActiveProject");
				}
			}
		}

		[DataSourceProperty]
		public int Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				if (value != this._progress)
				{
					this._progress = value;
					base.OnPropertyChangedWithValue(value, "Progress");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string Explanation
		{
			get
			{
				return this._explanation;
			}
			set
			{
				if (value != this._explanation)
				{
					this._explanation = value;
					base.OnPropertyChangedWithValue<string>(value, "Explanation");
				}
			}
		}

		public int Index;

		private Building _building;

		protected Action<SettlementProjectVM, bool> _onSelection;

		protected Action<SettlementProjectVM> _onSetAsCurrent;

		protected Action _onResetCurrent;

		private readonly TextObject L1BonusText = new TextObject("{=PJZ8QYgA}L-I : {BONUS}", null);

		private readonly TextObject L2BonusText = new TextObject("{=9i0wnjJK}L-II : {BONUS}", null);

		private readonly TextObject L3BonusText = new TextObject("{=pRP2sOWP}L-III : {BONUS}", null);

		private string _name;

		private string _visualCode;

		private string _explanation;

		private string _currentPositiveEffectText;

		private string _nextPositiveEffectText;

		private string _productionCostText;

		private int _progress;

		private bool _isCurrentActiveProject;

		private string _productionText;
	}
}
