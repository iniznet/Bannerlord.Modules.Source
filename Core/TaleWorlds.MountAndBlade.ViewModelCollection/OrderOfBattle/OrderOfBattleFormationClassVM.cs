using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	public class OrderOfBattleFormationClassVM : ViewModel
	{
		public FormationClass Class
		{
			get
			{
				return this._class;
			}
			set
			{
				if (value != this._class)
				{
					if (!this._isFormationClassPreset)
					{
						Action<OrderOfBattleFormationClassVM, FormationClass> onClassChanged = OrderOfBattleFormationClassVM.OnClassChanged;
						if (onClassChanged != null)
						{
							onClassChanged(this, value);
						}
					}
					this._class = value;
					this.IsUnset = this._class == FormationClass.NumberOfAllFormations;
					this.ShownFormationClass = (int)(this.IsUnset ? FormationClass.Infantry : (this._class + 1));
					this.UpdateWeightText();
					this._isFormationClassPreset = false;
				}
			}
		}

		public int PreviousWeight { get; private set; }

		public OrderOfBattleFormationClassVM(OrderOfBattleFormationItemVM formationItem, FormationClass formationClass = FormationClass.NumberOfAllFormations)
		{
			this.BelongedFormationItem = formationItem;
			this._isFormationClassPreset = formationClass != FormationClass.NumberOfAllFormations;
			this.Class = formationClass;
			this.PreviousWeight = 0;
			this.OnWeightAdjusted();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			this.LockWeightHint = new HintViewModel(new TextObject("{=mPCrz4rs}Lock troop percentage from relative changes.", null), null);
		}

		private void OnWeightAdjusted()
		{
			if (!this._isLockedOfWeightAdjustments)
			{
				Action<OrderOfBattleFormationClassVM> onWeightAdjustedCallback = OrderOfBattleFormationClassVM.OnWeightAdjustedCallback;
				if (onWeightAdjustedCallback != null)
				{
					onWeightAdjustedCallback(this);
				}
			}
			this.UpdateWeightText();
		}

		public void UpdateWeightText()
		{
			if (this.Class != FormationClass.NumberOfAllFormations && OrderOfBattleFormationClassVM.GetTotalCountOfTroopType != null)
			{
				GameTexts.SetVariable("NUMBER", this.Weight);
				GameTexts.SetVariable("PERCENTAGE", GameTexts.FindText("str_NUMBER_percent", null));
				GameTexts.SetVariable("TROOP_COUNT", OrderOfBattleUIHelper.GetVisibleCountOfUnitsInClass(this));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", OrderOfBattleFormationClassVM.GetTotalCountOfTroopType(this.Class));
				this.WeightText = this._weightWithTroopCountText.ToString();
				return;
			}
			this.WeightText = string.Empty;
		}

		public void SetWeightAdjustmentLock(bool isLocked)
		{
			this._isLockedOfWeightAdjustments = isLocked;
		}

		public void UpdateWeightAdjustable()
		{
			bool flag;
			if (this.Class != FormationClass.NumberOfAllFormations)
			{
				Func<OrderOfBattleFormationClassVM, bool> canAdjustWeight = OrderOfBattleFormationClassVM.CanAdjustWeight;
				flag = canAdjustWeight != null && canAdjustWeight(this);
			}
			else
			{
				flag = false;
			}
			this.IsAdjustable = flag;
		}

		[DataSourceProperty]
		public bool IsAdjustable
		{
			get
			{
				return this._isAdjustable;
			}
			set
			{
				if (value != this._isAdjustable)
				{
					this._isAdjustable = value && Mission.Current.PlayerTeam.IsPlayerGeneral;
					base.OnPropertyChangedWithValue(this._isAdjustable, "IsAdjustable");
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
		public bool IsUnset
		{
			get
			{
				return this._isUnset;
			}
			set
			{
				if (value != this._isUnset)
				{
					this._isUnset = value;
					base.OnPropertyChangedWithValue(value, "IsUnset");
				}
			}
		}

		[DataSourceProperty]
		public int Weight
		{
			get
			{
				return this._weight;
			}
			set
			{
				if (value != this._weight)
				{
					this.PreviousWeight = this._weight;
					this._weight = value;
					base.OnPropertyChangedWithValue(value, "Weight");
					this.OnWeightAdjusted();
				}
			}
		}

		[DataSourceProperty]
		public int ShownFormationClass
		{
			get
			{
				return this._shownFormationClass;
			}
			set
			{
				if (value != this._shownFormationClass)
				{
					this._shownFormationClass = value;
					base.OnPropertyChangedWithValue(value, "ShownFormationClass");
				}
			}
		}

		[DataSourceProperty]
		public string WeightText
		{
			get
			{
				return this._weightText;
			}
			set
			{
				if (value != this._weightText)
				{
					this._weightText = value;
					base.OnPropertyChangedWithValue<string>(value, "WeightText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel LockWeightHint
		{
			get
			{
				return this._lockWeightHint;
			}
			set
			{
				if (value != this._lockWeightHint)
				{
					this._lockWeightHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LockWeightHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWeightHighlightActive
		{
			get
			{
				return this._isWeightHighlightActive;
			}
			set
			{
				if (value != this._isWeightHighlightActive)
				{
					this._isWeightHighlightActive = value;
					base.OnPropertyChangedWithValue(value, "IsWeightHighlightActive");
				}
			}
		}

		private FormationClass _class;

		private bool _isLockedOfWeightAdjustments;

		public readonly OrderOfBattleFormationItemVM BelongedFormationItem;

		public static Action<OrderOfBattleFormationClassVM> OnWeightAdjustedCallback;

		public static Action<OrderOfBattleFormationClassVM, FormationClass> OnClassChanged;

		public static Func<OrderOfBattleFormationClassVM, bool> CanAdjustWeight;

		public static Func<FormationClass, int> GetTotalCountOfTroopType;

		private readonly TextObject _weightWithTroopCountText = new TextObject("{=s6qslcQY}{PERCENTAGE} ({TROOP_COUNT}/{TOTAL_TROOP_COUNT})", null);

		private bool _isFormationClassPreset;

		private bool _isAdjustable;

		private bool _isLocked;

		private bool _isUnset;

		private int _weight;

		private int _shownFormationClass;

		private string _weightText;

		private HintViewModel _lockWeightHint;

		private bool _isWeightHighlightActive;
	}
}
