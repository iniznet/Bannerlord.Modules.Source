using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleFormationItemListPanel : ListPanel
	{
		public OrderOfBattleFormationItemListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			Widget latestMouseUpWidget = base.EventManager.LatestMouseUpWidget;
			if (this.IsFilterDropdownEnabled && !base.CheckIsMyChildRecursive(latestMouseUpWidget))
			{
				this.IsFilterDropdownEnabled = false;
			}
		}

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

		private void OnClassDropdownEnabledStateChanged(DropdownWidget widget)
		{
			this.IsClassDropdownEnabled = widget.IsOpen;
		}

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

		private Widget _cardWidget;

		private DropdownWidget _formationClassDropdown;

		private bool _isControlledByPlayer;

		private bool _isFilterDropdownEnabled;

		private bool _isClassDropdownEnabled;

		private bool _isSelected;

		private bool _hasFormation;

		private float _defaultFocusYOffsetFromCenter;

		private float _noFormationFocusYOffsetFromCenter;
	}
}
