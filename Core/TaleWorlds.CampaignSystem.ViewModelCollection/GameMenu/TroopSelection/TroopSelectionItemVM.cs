using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection
{
	// Token: 0x0200008A RID: 138
	public class TroopSelectionItemVM : ViewModel
	{
		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06000D9D RID: 3485 RVA: 0x000372FE File Offset: 0x000354FE
		// (set) Token: 0x06000D9E RID: 3486 RVA: 0x00037306 File Offset: 0x00035506
		public TroopRosterElement Troop { get; private set; }

		// Token: 0x06000D9F RID: 3487 RVA: 0x00037310 File Offset: 0x00035510
		public TroopSelectionItemVM(TroopRosterElement troop, Action<TroopSelectionItemVM> onAdd, Action<TroopSelectionItemVM> onRemove)
		{
			this._onAdd = onAdd;
			this._onRemove = onRemove;
			this.Troop = troop;
			this.MaxAmount = this.Troop.Number - this.Troop.WoundedNumber;
			this.Visual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(troop.Character, false));
			this.Name = troop.Character.Name.ToString();
			this.TierIconData = CampaignUIHelper.GetCharacterTierData(this.Troop.Character, false);
			this.TypeIconData = CampaignUIHelper.GetCharacterTypeData(this.Troop.Character, false);
			this.IsTroopHero = this.Troop.Character.IsHero;
			this.HeroHealthPercent = (this.Troop.Character.IsHero ? MathF.Ceiling((float)this.Troop.Character.HeroObject.HitPoints / (float)this.Troop.Character.MaxHitPoints() * 100f) : 0);
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x0003741A File Offset: 0x0003561A
		public void ExecuteAdd()
		{
			Action<TroopSelectionItemVM> onAdd = this._onAdd;
			if (onAdd == null)
			{
				return;
			}
			onAdd.DynamicInvokeWithLog(new object[] { this });
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x00037437 File Offset: 0x00035637
		public void ExecuteRemove()
		{
			Action<TroopSelectionItemVM> onRemove = this._onRemove;
			if (onRemove == null)
			{
				return;
			}
			onRemove.DynamicInvokeWithLog(new object[] { this });
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x00037454 File Offset: 0x00035654
		private void UpdateAmountText()
		{
			GameTexts.SetVariable("LEFT", this.CurrentAmount);
			GameTexts.SetVariable("RIGHT", this.MaxAmount);
			this.AmountText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x0003748C File Offset: 0x0003568C
		public void ExecuteLink()
		{
			if (this.Troop.Character != null)
			{
				EncyclopediaManager encyclopediaManager = Campaign.Current.EncyclopediaManager;
				Hero heroObject = this.Troop.Character.HeroObject;
				encyclopediaManager.GoToLink(((heroObject != null) ? heroObject.EncyclopediaLink : null) ?? this.Troop.Character.EncyclopediaLink);
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x000374E5 File Offset: 0x000356E5
		// (set) Token: 0x06000DA5 RID: 3493 RVA: 0x000374ED File Offset: 0x000356ED
		[DataSourceProperty]
		public int MaxAmount
		{
			get
			{
				return this._maxAmount;
			}
			set
			{
				if (value != this._maxAmount)
				{
					this._maxAmount = value;
					base.OnPropertyChangedWithValue(value, "MaxAmount");
					this.UpdateAmountText();
				}
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x00037511 File Offset: 0x00035711
		// (set) Token: 0x06000DA7 RID: 3495 RVA: 0x00037519 File Offset: 0x00035719
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

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x00037537 File Offset: 0x00035737
		// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x0003753F File Offset: 0x0003573F
		[DataSourceProperty]
		public bool IsRosterFull
		{
			get
			{
				return this._isRosterFull;
			}
			set
			{
				if (value != this._isRosterFull)
				{
					this._isRosterFull = value;
					base.OnPropertyChangedWithValue(value, "IsRosterFull");
				}
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06000DAA RID: 3498 RVA: 0x0003755D File Offset: 0x0003575D
		// (set) Token: 0x06000DAB RID: 3499 RVA: 0x00037565 File Offset: 0x00035765
		[DataSourceProperty]
		public bool IsTroopHero
		{
			get
			{
				return this._isTroopHero;
			}
			set
			{
				if (value != this._isTroopHero)
				{
					this._isTroopHero = value;
					base.OnPropertyChangedWithValue(value, "IsTroopHero");
				}
			}
		}

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x00037583 File Offset: 0x00035783
		// (set) Token: 0x06000DAD RID: 3501 RVA: 0x0003758B File Offset: 0x0003578B
		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
				}
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x000375A9 File Offset: 0x000357A9
		// (set) Token: 0x06000DAF RID: 3503 RVA: 0x000375B1 File Offset: 0x000357B1
		[DataSourceProperty]
		public int CurrentAmount
		{
			get
			{
				return this._currentAmount;
			}
			set
			{
				if (value != this._currentAmount)
				{
					this._currentAmount = value;
					base.OnPropertyChangedWithValue(value, "CurrentAmount");
					this.IsSelected = value > 0;
					this.UpdateAmountText();
				}
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x000375DF File Offset: 0x000357DF
		// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x000375E7 File Offset: 0x000357E7
		[DataSourceProperty]
		public int HeroHealthPercent
		{
			get
			{
				return this._heroHealthPercent;
			}
			set
			{
				if (value != this._heroHealthPercent)
				{
					this._heroHealthPercent = value;
					base.OnPropertyChangedWithValue(value, "HeroHealthPercent");
				}
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x00037605 File Offset: 0x00035805
		// (set) Token: 0x06000DB3 RID: 3507 RVA: 0x0003760D File Offset: 0x0003580D
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

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06000DB4 RID: 3508 RVA: 0x00037630 File Offset: 0x00035830
		// (set) Token: 0x06000DB5 RID: 3509 RVA: 0x00037638 File Offset: 0x00035838
		[DataSourceProperty]
		public string AmountText
		{
			get
			{
				return this._amountText;
			}
			set
			{
				if (value != this._amountText)
				{
					this._amountText = value;
					base.OnPropertyChangedWithValue<string>(value, "AmountText");
				}
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06000DB6 RID: 3510 RVA: 0x0003765B File Offset: 0x0003585B
		// (set) Token: 0x06000DB7 RID: 3511 RVA: 0x00037663 File Offset: 0x00035863
		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06000DB8 RID: 3512 RVA: 0x00037681 File Offset: 0x00035881
		// (set) Token: 0x06000DB9 RID: 3513 RVA: 0x00037689 File Offset: 0x00035889
		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
				}
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06000DBA RID: 3514 RVA: 0x000376A7 File Offset: 0x000358A7
		// (set) Token: 0x06000DBB RID: 3515 RVA: 0x000376AF File Offset: 0x000358AF
		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
				}
			}
		}

		// Token: 0x04000657 RID: 1623
		private readonly Action<TroopSelectionItemVM> _onAdd;

		// Token: 0x04000658 RID: 1624
		private readonly Action<TroopSelectionItemVM> _onRemove;

		// Token: 0x04000659 RID: 1625
		private int _currentAmount;

		// Token: 0x0400065A RID: 1626
		private int _maxAmount;

		// Token: 0x0400065B RID: 1627
		private int _heroHealthPercent;

		// Token: 0x0400065C RID: 1628
		private ImageIdentifierVM _visual;

		// Token: 0x0400065D RID: 1629
		private bool _isSelected;

		// Token: 0x0400065E RID: 1630
		private bool _isRosterFull;

		// Token: 0x0400065F RID: 1631
		private bool _isLocked;

		// Token: 0x04000660 RID: 1632
		private bool _isTroopHero;

		// Token: 0x04000661 RID: 1633
		private string _name;

		// Token: 0x04000662 RID: 1634
		private string _amountText;

		// Token: 0x04000663 RID: 1635
		private StringItemWithHintVM _tierIconData;

		// Token: 0x04000664 RID: 1636
		private StringItemWithHintVM _typeIconData;
	}
}
