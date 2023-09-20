using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x02000091 RID: 145
	public abstract class SettlementProjectVM : ViewModel
	{
		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x000386F9 File Offset: 0x000368F9
		// (set) Token: 0x06000E0F RID: 3599 RVA: 0x00038701 File Offset: 0x00036901
		public bool IsDaily { get; protected set; }

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06000E10 RID: 3600 RVA: 0x0003870A File Offset: 0x0003690A
		// (set) Token: 0x06000E11 RID: 3601 RVA: 0x00038714 File Offset: 0x00036914
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

		// Token: 0x06000E12 RID: 3602 RVA: 0x000387E0 File Offset: 0x000369E0
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

		// Token: 0x06000E13 RID: 3603 RVA: 0x0003887C File Offset: 0x00036A7C
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

		// Token: 0x06000E14 RID: 3604 RVA: 0x00038910 File Offset: 0x00036B10
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

		// Token: 0x06000E15 RID: 3605 RVA: 0x00038962 File Offset: 0x00036B62
		private void ExecuteShowTooltip()
		{
			InformationManager.ShowTooltip(typeof(Building), new object[] { this._building });
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x00038982 File Offset: 0x00036B82
		private void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x00038989 File Offset: 0x00036B89
		private TextObject GetBonusExplanationOfLevel(int level)
		{
			if (level >= 0 && level <= 3)
			{
				return this.Building.BuildingType.GetExplanationAtLevel(level);
			}
			return TextObject.Empty;
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x000389AA File Offset: 0x00036BAA
		public virtual void RefreshProductionText()
		{
		}

		// Token: 0x06000E19 RID: 3609
		public abstract void ExecuteAddToQueue();

		// Token: 0x06000E1A RID: 3610
		public abstract void ExecuteSetAsActiveDevelopment();

		// Token: 0x06000E1B RID: 3611
		public abstract void ExecuteSetAsCurrent();

		// Token: 0x06000E1C RID: 3612
		public abstract void ExecuteResetCurrent();

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06000E1D RID: 3613 RVA: 0x000389AC File Offset: 0x00036BAC
		// (set) Token: 0x06000E1E RID: 3614 RVA: 0x000389B4 File Offset: 0x00036BB4
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

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06000E1F RID: 3615 RVA: 0x000389D7 File Offset: 0x00036BD7
		// (set) Token: 0x06000E20 RID: 3616 RVA: 0x000389DF File Offset: 0x00036BDF
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

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06000E21 RID: 3617 RVA: 0x00038A02 File Offset: 0x00036C02
		// (set) Token: 0x06000E22 RID: 3618 RVA: 0x00038A0A File Offset: 0x00036C0A
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

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06000E23 RID: 3619 RVA: 0x00038A2D File Offset: 0x00036C2D
		// (set) Token: 0x06000E24 RID: 3620 RVA: 0x00038A35 File Offset: 0x00036C35
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

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06000E25 RID: 3621 RVA: 0x00038A58 File Offset: 0x00036C58
		// (set) Token: 0x06000E26 RID: 3622 RVA: 0x00038A60 File Offset: 0x00036C60
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

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06000E27 RID: 3623 RVA: 0x00038A83 File Offset: 0x00036C83
		// (set) Token: 0x06000E28 RID: 3624 RVA: 0x00038A8B File Offset: 0x00036C8B
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

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x00038AA9 File Offset: 0x00036CA9
		// (set) Token: 0x06000E2A RID: 3626 RVA: 0x00038AB1 File Offset: 0x00036CB1
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

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x00038ACF File Offset: 0x00036CCF
		// (set) Token: 0x06000E2C RID: 3628 RVA: 0x00038AD7 File Offset: 0x00036CD7
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

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x00038AFA File Offset: 0x00036CFA
		// (set) Token: 0x06000E2E RID: 3630 RVA: 0x00038B02 File Offset: 0x00036D02
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

		// Token: 0x04000684 RID: 1668
		public int Index;

		// Token: 0x04000686 RID: 1670
		private Building _building;

		// Token: 0x04000687 RID: 1671
		protected Action<SettlementProjectVM, bool> _onSelection;

		// Token: 0x04000688 RID: 1672
		protected Action<SettlementProjectVM> _onSetAsCurrent;

		// Token: 0x04000689 RID: 1673
		protected Action _onResetCurrent;

		// Token: 0x0400068A RID: 1674
		private readonly TextObject L1BonusText = new TextObject("{=PJZ8QYgA}L-I : {BONUS}", null);

		// Token: 0x0400068B RID: 1675
		private readonly TextObject L2BonusText = new TextObject("{=9i0wnjJK}L-II : {BONUS}", null);

		// Token: 0x0400068C RID: 1676
		private readonly TextObject L3BonusText = new TextObject("{=pRP2sOWP}L-III : {BONUS}", null);

		// Token: 0x0400068D RID: 1677
		private string _name;

		// Token: 0x0400068E RID: 1678
		private string _visualCode;

		// Token: 0x0400068F RID: 1679
		private string _explanation;

		// Token: 0x04000690 RID: 1680
		private string _currentPositiveEffectText;

		// Token: 0x04000691 RID: 1681
		private string _nextPositiveEffectText;

		// Token: 0x04000692 RID: 1682
		private string _productionCostText;

		// Token: 0x04000693 RID: 1683
		private int _progress;

		// Token: 0x04000694 RID: 1684
		private bool _isCurrentActiveProject;

		// Token: 0x04000695 RID: 1685
		private string _productionText;
	}
}
