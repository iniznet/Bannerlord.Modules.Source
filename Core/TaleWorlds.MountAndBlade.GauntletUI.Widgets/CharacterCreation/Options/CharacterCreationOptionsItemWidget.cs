using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Options
{
	public class CharacterCreationOptionsItemWidget : Widget
	{
		public CharacterCreationOptionsItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isDirty)
			{
				if (this.Type == 0)
				{
					this.ActionOptionWidget.IsVisible = false;
					this.BooleanOptionWidget.IsVisible = true;
					this.SelectionOptionWidget.IsVisible = false;
					this.NumericOptionWidget.IsVisible = false;
				}
				else if (this.Type == 1)
				{
					this.ActionOptionWidget.IsVisible = false;
					this.BooleanOptionWidget.IsVisible = false;
					this.SelectionOptionWidget.IsVisible = false;
					this.NumericOptionWidget.IsVisible = true;
				}
				else if (this.Type == 2)
				{
					this.ActionOptionWidget.IsVisible = false;
					this.BooleanOptionWidget.IsVisible = false;
					this.SelectionOptionWidget.IsVisible = true;
					this.NumericOptionWidget.IsVisible = false;
				}
				else if (this.Type == 3)
				{
					this.ActionOptionWidget.IsVisible = true;
					this.BooleanOptionWidget.IsVisible = false;
					this.SelectionOptionWidget.IsVisible = false;
					this.NumericOptionWidget.IsVisible = false;
				}
				this.ResetNavigationIndices();
				this._isDirty = false;
			}
		}

		private void ResetNavigationIndices()
		{
			if (base.GamepadNavigationIndex == -1)
			{
				return;
			}
			bool flag = false;
			Widget booleanOptionWidget = this.BooleanOptionWidget;
			if (booleanOptionWidget != null && booleanOptionWidget.IsVisible)
			{
				this.BooleanOptionWidget.GamepadNavigationIndex = base.GamepadNavigationIndex;
				flag = true;
			}
			else
			{
				Widget numericOptionWidget = this.NumericOptionWidget;
				if (numericOptionWidget != null && numericOptionWidget.IsVisible)
				{
					this.NumericOptionWidget.GamepadNavigationIndex = base.GamepadNavigationIndex;
					flag = true;
				}
				else
				{
					Widget selectionOptionWidget = this.SelectionOptionWidget;
					if (selectionOptionWidget != null && selectionOptionWidget.IsVisible)
					{
						this.SelectionOptionWidget.GamepadNavigationIndex = base.GamepadNavigationIndex;
						flag = true;
					}
					else
					{
						Widget actionOptionWidget = this.ActionOptionWidget;
						if (actionOptionWidget != null && actionOptionWidget.IsVisible)
						{
							this.ActionOptionWidget.GamepadNavigationIndex = base.GamepadNavigationIndex;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				base.GamepadNavigationIndex = -1;
			}
		}

		protected override void OnGamepadNavigationIndexUpdated(int newIndex)
		{
			base.OnGamepadNavigationIndexUpdated(newIndex);
			this.ResetNavigationIndices();
		}

		[Editor(false)]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (this._type != value)
				{
					this._type = value;
					base.OnPropertyChanged(value, "Type");
					this._isDirty = true;
				}
			}
		}

		[Editor(false)]
		public Widget ActionOptionWidget
		{
			get
			{
				return this._actionOptionWidget;
			}
			set
			{
				if (this._actionOptionWidget != value)
				{
					this._actionOptionWidget = value;
					base.OnPropertyChanged<Widget>(value, "ActionOptionWidget");
				}
			}
		}

		[Editor(false)]
		public Widget NumericOptionWidget
		{
			get
			{
				return this._numericOptionWidget;
			}
			set
			{
				if (this._numericOptionWidget != value)
				{
					this._numericOptionWidget = value;
					base.OnPropertyChanged<Widget>(value, "NumericOptionWidget");
				}
			}
		}

		[Editor(false)]
		public Widget SelectionOptionWidget
		{
			get
			{
				return this._selectionOptionWidget;
			}
			set
			{
				if (this._selectionOptionWidget != value)
				{
					this._selectionOptionWidget = value;
					base.OnPropertyChanged<Widget>(value, "SelectionOptionWidget");
				}
			}
		}

		[Editor(false)]
		public Widget BooleanOptionWidget
		{
			get
			{
				return this._booleanOptionWidget;
			}
			set
			{
				if (this._booleanOptionWidget != value)
				{
					this._booleanOptionWidget = value;
					base.OnPropertyChanged<Widget>(value, "BooleanOptionWidget");
				}
			}
		}

		private bool _isDirty = true;

		private int _type;

		private Widget _actionOptionWidget;

		private Widget _numericOptionWidget;

		private Widget _selectionOptionWidget;

		private Widget _booleanOptionWidget;
	}
}
