using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x0200002B RID: 43
	public class OrderOfBattleFormationClassVM : ViewModel
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000DE08 File Offset: 0x0000C008
		// (set) Token: 0x06000308 RID: 776 RVA: 0x0000DE10 File Offset: 0x0000C010
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

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000DE7D File Offset: 0x0000C07D
		// (set) Token: 0x0600030A RID: 778 RVA: 0x0000DE85 File Offset: 0x0000C085
		public int PreviousWeight { get; private set; }

		// Token: 0x0600030B RID: 779 RVA: 0x0000DE90 File Offset: 0x0000C090
		public OrderOfBattleFormationClassVM(OrderOfBattleFormationItemVM formationItem, FormationClass formationClass = FormationClass.NumberOfAllFormations)
		{
			this.BelongedFormationItem = formationItem;
			this._isFormationClassPreset = formationClass != FormationClass.NumberOfAllFormations;
			this.Class = formationClass;
			this.PreviousWeight = 0;
			this.OnWeightAdjusted();
			this.RefreshValues();
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0000DEE3 File Offset: 0x0000C0E3
		public override void RefreshValues()
		{
			this.LockWeightHint = new HintViewModel(new TextObject("{=mPCrz4rs}Lock troop percentage from relative changes.", null), null);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000DEFC File Offset: 0x0000C0FC
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

		// Token: 0x0600030E RID: 782 RVA: 0x0000DF20 File Offset: 0x0000C120
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

		// Token: 0x0600030F RID: 783 RVA: 0x0000DFAA File Offset: 0x0000C1AA
		public void SetWeightAdjustmentLock(bool isLocked)
		{
			this._isLockedOfWeightAdjustments = isLocked;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000DFB3 File Offset: 0x0000C1B3
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

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000DFDA File Offset: 0x0000C1DA
		// (set) Token: 0x06000312 RID: 786 RVA: 0x0000DFE2 File Offset: 0x0000C1E2
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

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0000E019 File Offset: 0x0000C219
		// (set) Token: 0x06000314 RID: 788 RVA: 0x0000E021 File Offset: 0x0000C221
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

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0000E03F File Offset: 0x0000C23F
		// (set) Token: 0x06000316 RID: 790 RVA: 0x0000E047 File Offset: 0x0000C247
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

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0000E065 File Offset: 0x0000C265
		// (set) Token: 0x06000318 RID: 792 RVA: 0x0000E06D File Offset: 0x0000C26D
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

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0000E09D File Offset: 0x0000C29D
		// (set) Token: 0x0600031A RID: 794 RVA: 0x0000E0A5 File Offset: 0x0000C2A5
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

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000E0C3 File Offset: 0x0000C2C3
		// (set) Token: 0x0600031C RID: 796 RVA: 0x0000E0CB File Offset: 0x0000C2CB
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

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000E0EE File Offset: 0x0000C2EE
		// (set) Token: 0x0600031E RID: 798 RVA: 0x0000E0F6 File Offset: 0x0000C2F6
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

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000E114 File Offset: 0x0000C314
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000E11C File Offset: 0x0000C31C
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

		// Token: 0x0400017F RID: 383
		private FormationClass _class;

		// Token: 0x04000180 RID: 384
		private bool _isLockedOfWeightAdjustments;

		// Token: 0x04000182 RID: 386
		public readonly OrderOfBattleFormationItemVM BelongedFormationItem;

		// Token: 0x04000183 RID: 387
		public static Action<OrderOfBattleFormationClassVM> OnWeightAdjustedCallback;

		// Token: 0x04000184 RID: 388
		public static Action<OrderOfBattleFormationClassVM, FormationClass> OnClassChanged;

		// Token: 0x04000185 RID: 389
		public static Func<OrderOfBattleFormationClassVM, bool> CanAdjustWeight;

		// Token: 0x04000186 RID: 390
		public static Func<FormationClass, int> GetTotalCountOfTroopType;

		// Token: 0x04000187 RID: 391
		private readonly TextObject _weightWithTroopCountText = new TextObject("{=s6qslcQY}{PERCENTAGE} ({TROOP_COUNT}/{TOTAL_TROOP_COUNT})", null);

		// Token: 0x04000188 RID: 392
		private bool _isFormationClassPreset;

		// Token: 0x04000189 RID: 393
		private bool _isAdjustable;

		// Token: 0x0400018A RID: 394
		private bool _isLocked;

		// Token: 0x0400018B RID: 395
		private bool _isUnset;

		// Token: 0x0400018C RID: 396
		private int _weight;

		// Token: 0x0400018D RID: 397
		private int _shownFormationClass;

		// Token: 0x0400018E RID: 398
		private string _weightText;

		// Token: 0x0400018F RID: 399
		private HintViewModel _lockWeightHint;

		// Token: 0x04000190 RID: 400
		private bool _isWeightHighlightActive;
	}
}
