using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D3 RID: 211
	public class OrderOfBattleFormationItemListPanel : ListPanel
	{
		// Token: 0x06000ABC RID: 2748 RVA: 0x0001DF31 File Offset: 0x0001C131
		public OrderOfBattleFormationItemListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x0001DF3C File Offset: 0x0001C13C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			Widget latestMouseUpWidget = base.EventManager.LatestMouseUpWidget;
			if (this.IsFilterDropdownEnabled && !base.CheckIsMyChildRecursive(latestMouseUpWidget))
			{
				this.IsFilterDropdownEnabled = false;
			}
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0001DF74 File Offset: 0x0001C174
		private void OnStateChanged()
		{
			if (this.IsSelected)
			{
				Widget cardWidget = this.CardWidget;
				if (cardWidget == null)
				{
					return;
				}
				cardWidget.SetState("Selected");
				return;
			}
			else
			{
				Widget cardWidget2 = this.CardWidget;
				if (cardWidget2 == null)
				{
					return;
				}
				cardWidget2.SetState("Default");
				return;
			}
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0001DFA9 File Offset: 0x0001C1A9
		private void OnClassDropdownEnabledStateChanged(DropdownWidget widget)
		{
			this.IsClassDropdownEnabled = widget.IsOpen;
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x0001DFB7 File Offset: 0x0001C1B7
		// (set) Token: 0x06000AC1 RID: 2753 RVA: 0x0001DFBF File Offset: 0x0001C1BF
		[Editor(false)]
		public Widget CardWidget
		{
			get
			{
				return this._cardWidget;
			}
			set
			{
				if (value != this._cardWidget)
				{
					this._cardWidget = value;
					base.OnPropertyChanged<Widget>(value, "CardWidget");
				}
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x0001DFDD File Offset: 0x0001C1DD
		// (set) Token: 0x06000AC3 RID: 2755 RVA: 0x0001DFE8 File Offset: 0x0001C1E8
		[Editor(false)]
		public DropdownWidget FormationClassDropdown
		{
			get
			{
				return this._formationClassDropdown;
			}
			set
			{
				if (value != this._formationClassDropdown)
				{
					if (this._formationClassDropdown != null)
					{
						DropdownWidget formationClassDropdown = this._formationClassDropdown;
						formationClassDropdown.OnOpenStateChanged = (Action<DropdownWidget>)Delegate.Remove(formationClassDropdown.OnOpenStateChanged, new Action<DropdownWidget>(this.OnClassDropdownEnabledStateChanged));
					}
					this._formationClassDropdown = value;
					base.OnPropertyChanged<DropdownWidget>(value, "FormationClassDropdown");
					if (this._formationClassDropdown != null)
					{
						DropdownWidget formationClassDropdown2 = this._formationClassDropdown;
						formationClassDropdown2.OnOpenStateChanged = (Action<DropdownWidget>)Delegate.Combine(formationClassDropdown2.OnOpenStateChanged, new Action<DropdownWidget>(this.OnClassDropdownEnabledStateChanged));
						this.OnClassDropdownEnabledStateChanged(this._formationClassDropdown);
					}
				}
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x0001E07B File Offset: 0x0001C27B
		// (set) Token: 0x06000AC5 RID: 2757 RVA: 0x0001E084 File Offset: 0x0001C284
		[Editor(false)]
		public bool IsControlledByPlayer
		{
			get
			{
				return this._isControlledByPlayer;
			}
			set
			{
				if (value != this._isControlledByPlayer)
				{
					this._isControlledByPlayer = value;
					base.OnPropertyChanged(value, "IsControlledByPlayer");
					DropdownWidget formationClassDropdown = this.FormationClassDropdown;
					if (((formationClassDropdown != null) ? formationClassDropdown.Button : null) != null)
					{
						this.FormationClassDropdown.Button.IsEnabled = value;
					}
				}
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x0001E0D2 File Offset: 0x0001C2D2
		// (set) Token: 0x06000AC7 RID: 2759 RVA: 0x0001E0DA File Offset: 0x0001C2DA
		[Editor(false)]
		public bool IsFilterDropdownEnabled
		{
			get
			{
				return this._isFilterDropdownEnabled;
			}
			set
			{
				if (value != this._isFilterDropdownEnabled)
				{
					this._isFilterDropdownEnabled = value;
					base.OnPropertyChanged(value, "IsFilterDropdownEnabled");
				}
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x0001E0F8 File Offset: 0x0001C2F8
		// (set) Token: 0x06000AC9 RID: 2761 RVA: 0x0001E100 File Offset: 0x0001C300
		[Editor(false)]
		public bool IsClassDropdownEnabled
		{
			get
			{
				return this._isClassDropdownEnabled;
			}
			set
			{
				if (value != this._isClassDropdownEnabled)
				{
					this._isClassDropdownEnabled = value;
					base.OnPropertyChanged(value, "IsClassDropdownEnabled");
					if (this.FormationClassDropdown != null)
					{
						this.FormationClassDropdown.IsOpen = value;
					}
				}
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000ACA RID: 2762 RVA: 0x0001E132 File Offset: 0x0001C332
		// (set) Token: 0x06000ACB RID: 2763 RVA: 0x0001E13A File Offset: 0x0001C33A
		[Editor(false)]
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
					base.OnPropertyChanged(value, "IsSelected");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000ACC RID: 2764 RVA: 0x0001E15E File Offset: 0x0001C35E
		// (set) Token: 0x06000ACD RID: 2765 RVA: 0x0001E166 File Offset: 0x0001C366
		[Editor(false)]
		public bool HasFormation
		{
			get
			{
				return this._hasFormation;
			}
			set
			{
				if (value != this._hasFormation)
				{
					this._hasFormation = value;
					base.OnPropertyChanged(value, "HasFormation");
				}
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0001E184 File Offset: 0x0001C384
		// (set) Token: 0x06000ACF RID: 2767 RVA: 0x0001E18C File Offset: 0x0001C38C
		[Editor(false)]
		public float DefaultFocusYOffsetFromCenter
		{
			get
			{
				return this._defaultFocusYOffsetFromCenter;
			}
			set
			{
				if (value != this._defaultFocusYOffsetFromCenter)
				{
					this._defaultFocusYOffsetFromCenter = value;
					base.OnPropertyChanged(value, "DefaultFocusYOffsetFromCenter");
				}
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x0001E1AA File Offset: 0x0001C3AA
		// (set) Token: 0x06000AD1 RID: 2769 RVA: 0x0001E1B2 File Offset: 0x0001C3B2
		[Editor(false)]
		public float NoFormationFocusYOffsetFromCenter
		{
			get
			{
				return this._noFormationFocusYOffsetFromCenter;
			}
			set
			{
				if (value != this._noFormationFocusYOffsetFromCenter)
				{
					this._noFormationFocusYOffsetFromCenter = value;
					base.OnPropertyChanged(value, "NoFormationFocusYOffsetFromCenter");
				}
			}
		}

		// Token: 0x040004E6 RID: 1254
		private Widget _cardWidget;

		// Token: 0x040004E7 RID: 1255
		private DropdownWidget _formationClassDropdown;

		// Token: 0x040004E8 RID: 1256
		private bool _isControlledByPlayer;

		// Token: 0x040004E9 RID: 1257
		private bool _isFilterDropdownEnabled;

		// Token: 0x040004EA RID: 1258
		private bool _isClassDropdownEnabled;

		// Token: 0x040004EB RID: 1259
		private bool _isSelected;

		// Token: 0x040004EC RID: 1260
		private bool _hasFormation;

		// Token: 0x040004ED RID: 1261
		private float _defaultFocusYOffsetFromCenter;

		// Token: 0x040004EE RID: 1262
		private float _noFormationFocusYOffsetFromCenter;
	}
}
