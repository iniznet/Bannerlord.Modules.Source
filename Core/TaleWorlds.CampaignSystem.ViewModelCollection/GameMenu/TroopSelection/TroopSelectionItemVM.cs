using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection
{
	public class TroopSelectionItemVM : ViewModel
	{
		public TroopRosterElement Troop { get; private set; }

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

		public void ExecuteAdd()
		{
			Action<TroopSelectionItemVM> onAdd = this._onAdd;
			if (onAdd == null)
			{
				return;
			}
			onAdd.DynamicInvokeWithLog(new object[] { this });
		}

		public void ExecuteRemove()
		{
			Action<TroopSelectionItemVM> onRemove = this._onRemove;
			if (onRemove == null)
			{
				return;
			}
			onRemove.DynamicInvokeWithLog(new object[] { this });
		}

		private void UpdateAmountText()
		{
			GameTexts.SetVariable("LEFT", this.CurrentAmount);
			GameTexts.SetVariable("RIGHT", this.MaxAmount);
			this.AmountText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

		public void ExecuteLink()
		{
			if (this.Troop.Character != null)
			{
				EncyclopediaManager encyclopediaManager = Campaign.Current.EncyclopediaManager;
				Hero heroObject = this.Troop.Character.HeroObject;
				encyclopediaManager.GoToLink(((heroObject != null) ? heroObject.EncyclopediaLink : null) ?? this.Troop.Character.EncyclopediaLink);
			}
		}

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

		private readonly Action<TroopSelectionItemVM> _onAdd;

		private readonly Action<TroopSelectionItemVM> _onRemove;

		private int _currentAmount;

		private int _maxAmount;

		private int _heroHealthPercent;

		private ImageIdentifierVM _visual;

		private bool _isSelected;

		private bool _isRosterFull;

		private bool _isLocked;

		private bool _isTroopHero;

		private string _name;

		private string _amountText;

		private StringItemWithHintVM _tierIconData;

		private StringItemWithHintVM _typeIconData;
	}
}
