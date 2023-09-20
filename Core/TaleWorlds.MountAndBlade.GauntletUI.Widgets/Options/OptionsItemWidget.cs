using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	public class OptionsItemWidget : Widget
	{
		public Widget BooleanOption { get; set; }

		public Widget NumericOption { get; set; }

		public Widget StringOption { get; set; }

		public Widget GameKeyOption { get; set; }

		public Widget ActionOption { get; set; }

		public Widget InputOption { get; set; }

		public AnimatedDropdownWidget DropdownWidget
		{
			get
			{
				return this._dropdownWidget;
			}
			set
			{
				if (value != this._dropdownWidget)
				{
					this._dropdownWidget = value;
				}
			}
		}

		public ButtonWidget BooleanToggleButtonWidget
		{
			get
			{
				return this._booleanToggleButtonWidget;
			}
			set
			{
				if (value != this._booleanToggleButtonWidget)
				{
					this._booleanToggleButtonWidget = value;
				}
			}
		}

		public OptionsItemWidget(UIContext context)
			: base(context)
		{
			this._optionTypeID = -1;
			this._graphicsSprites = new List<Sprite>();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.SetCurrentScreenWidget(this.FindScreenWidget(base.ParentWidget));
				if (this.ImageIDs != null)
				{
					for (int i = 0; i < this.ImageIDs.Length; i++)
					{
						if (this.ImageIDs[i] != string.Empty)
						{
							Sprite sprite = base.Context.SpriteData.GetSprite(this.ImageIDs[i]);
							this._graphicsSprites.Add(sprite);
						}
					}
				}
				this.RefreshVisibilityOfSubItems();
				this.ResetNavigationIndices();
				this._initialized = true;
			}
			if (!this._eventsRegistered)
			{
				this.RegisterHoverEvents();
				this._eventsRegistered = true;
			}
			if (this._isEnabledStateDirty)
			{
				Widget currentOptionWidget = this.GetCurrentOptionWidget();
				if (currentOptionWidget != null)
				{
					foreach (Widget widget in currentOptionWidget.AllChildrenAndThis)
					{
						widget.IsEnabled = this.IsOptionEnabled;
					}
				}
				this._isEnabledStateDirty = false;
			}
		}

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetCurrentOption(false, false, -1);
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.ResetCurrentOption();
		}

		private OptionsScreenWidget FindScreenWidget(Widget parent)
		{
			OptionsScreenWidget optionsScreenWidget;
			if ((optionsScreenWidget = parent as OptionsScreenWidget) != null)
			{
				return optionsScreenWidget;
			}
			if (parent == null)
			{
				return null;
			}
			return this.FindScreenWidget(parent.ParentWidget);
		}

		private void SetCurrentOption(bool fromHoverOverDropdown, bool fromBooleanSelection, int hoverDropdownItemIndex = -1)
		{
			if (this._optionTypeID == 3)
			{
				Sprite sprite;
				if (fromHoverOverDropdown)
				{
					sprite = ((this._graphicsSprites.Count > hoverDropdownItemIndex) ? this._graphicsSprites[hoverDropdownItemIndex] : null);
				}
				else
				{
					sprite = ((this._graphicsSprites.Count > this.DropdownWidget.CurrentSelectedIndex && this.DropdownWidget.CurrentSelectedIndex >= 0) ? this._graphicsSprites[this.DropdownWidget.CurrentSelectedIndex] : null);
				}
				OptionsScreenWidget screenWidget = this._screenWidget;
				if (screenWidget == null)
				{
					return;
				}
				screenWidget.SetCurrentOption(this, sprite);
				return;
			}
			else if (this._optionTypeID == 0)
			{
				int num = (this.BooleanToggleButtonWidget.IsSelected ? 0 : 1);
				Sprite sprite2 = ((this._graphicsSprites.Count > num) ? this._graphicsSprites[num] : null);
				OptionsScreenWidget screenWidget2 = this._screenWidget;
				if (screenWidget2 == null)
				{
					return;
				}
				screenWidget2.SetCurrentOption(this, sprite2);
				return;
			}
			else
			{
				OptionsScreenWidget screenWidget3 = this._screenWidget;
				if (screenWidget3 == null)
				{
					return;
				}
				screenWidget3.SetCurrentOption(this, null);
				return;
			}
		}

		public void SetCurrentScreenWidget(OptionsScreenWidget screenWidget)
		{
			this._screenWidget = screenWidget;
		}

		private void ResetCurrentOption()
		{
			OptionsScreenWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(null, null);
		}

		private void RegisterHoverEvents()
		{
			foreach (Widget widget in base.AllChildren)
			{
				widget.boolPropertyChanged += this.Child_PropertyChanged;
			}
			if (this.OptionTypeID == 0)
			{
				this.BooleanToggleButtonWidget.boolPropertyChanged += this.BooleanOption_PropertyChanged;
				return;
			}
			if (this.OptionTypeID == 3)
			{
				this._dropdownExtensionParentWidget = this.DropdownWidget.DropdownClipWidget;
				foreach (Widget widget2 in this._dropdownExtensionParentWidget.AllChildren)
				{
					widget2.boolPropertyChanged += this.DropdownItem_PropertyChanged1;
				}
			}
		}

		private void BooleanOption_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsSelected")
			{
				this.SetCurrentOption(false, true, -1);
			}
		}

		private void Child_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				if (propertyValue)
				{
					this.SetCurrentOption(false, false, -1);
					return;
				}
				this.ResetCurrentOption();
			}
		}

		private void DropdownItem_PropertyChanged1(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				if (propertyValue)
				{
					Widget widget = childWidget as Widget;
					this.SetCurrentOption(true, false, widget.ParentWidget.GetChildIndex(widget));
					return;
				}
				this.ResetCurrentOption();
			}
		}

		private void RefreshVisibilityOfSubItems()
		{
			this.BooleanOption.IsVisible = this.OptionTypeID == 0;
			this.NumericOption.IsVisible = this.OptionTypeID == 1;
			this.StringOption.IsVisible = this.OptionTypeID == 3;
			this.GameKeyOption.IsVisible = this.OptionTypeID == 2;
			this.InputOption.IsVisible = this.OptionTypeID == 4;
			if (this.ActionOption != null)
			{
				this.ActionOption.IsVisible = this.OptionTypeID == 5;
			}
		}

		private Widget GetCurrentOptionWidget()
		{
			switch (this.OptionTypeID)
			{
			case 0:
				return this.BooleanOption;
			case 1:
				return this.NumericOption;
			case 2:
				return this.StringOption;
			case 3:
				return this.GameKeyOption;
			case 4:
				return this.InputOption;
			case 5:
				return this.ActionOption;
			default:
				return null;
			}
		}

		private void ResetNavigationIndices()
		{
			if (base.GamepadNavigationIndex == -1)
			{
				return;
			}
			bool flag = false;
			Widget booleanOption = this.BooleanOption;
			if (booleanOption != null && booleanOption.IsVisible)
			{
				this.BooleanOption.GamepadNavigationIndex = base.GamepadNavigationIndex;
				flag = true;
			}
			else
			{
				Widget numericOption = this.NumericOption;
				if (numericOption != null && numericOption.IsVisible)
				{
					this.NumericOption.GamepadNavigationIndex = base.GamepadNavigationIndex;
					flag = true;
				}
				else
				{
					Widget stringOption = this.StringOption;
					if (stringOption != null && stringOption.IsVisible)
					{
						this.StringOption.GamepadNavigationIndex = base.GamepadNavigationIndex;
						flag = true;
					}
					else
					{
						Widget gameKeyOption = this.GameKeyOption;
						if (gameKeyOption != null && gameKeyOption.IsVisible)
						{
							this.GameKeyOption.GamepadNavigationIndex = base.GamepadNavigationIndex;
							flag = true;
						}
						else
						{
							Widget inputOption = this.InputOption;
							if (inputOption != null && inputOption.IsVisible)
							{
								this.InputOption.GamepadNavigationIndex = base.GamepadNavigationIndex;
								flag = true;
							}
							else
							{
								Widget actionOption = this.ActionOption;
								if (actionOption != null && actionOption.IsVisible)
								{
									this.ActionOption.GamepadNavigationIndex = base.GamepadNavigationIndex;
									flag = true;
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				base.GamepadNavigationIndex = -1;
				return;
			}
			Debug.FailedAssert("No option type is visible for: " + base.GetType().Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Options\\OptionsItemWidget.cs", "ResetNavigationIndices", 325);
		}

		protected override void OnGamepadNavigationIndexUpdated(int newIndex)
		{
			if (this._initialized)
			{
				this.ResetNavigationIndices();
			}
		}

		public int OptionTypeID
		{
			get
			{
				return this._optionTypeID;
			}
			set
			{
				if (this._optionTypeID != value)
				{
					this._optionTypeID = value;
					base.OnPropertyChanged(value, "OptionTypeID");
				}
			}
		}

		public bool IsOptionEnabled
		{
			get
			{
				return this._isOptionEnabled;
			}
			set
			{
				if (this._isOptionEnabled != value)
				{
					this._isOptionEnabled = value;
					base.OnPropertyChanged(value, "IsOptionEnabled");
					this._isEnabledStateDirty = true;
				}
			}
		}

		public string OptionTitle
		{
			get
			{
				return this._optionTitle;
			}
			set
			{
				if (this._optionTitle != value)
				{
					this._optionTitle = value;
				}
			}
		}

		public string[] ImageIDs
		{
			get
			{
				return this._imageIDs;
			}
			set
			{
				if (this._imageIDs != value)
				{
					this._imageIDs = value;
				}
			}
		}

		public string OptionDescription
		{
			get
			{
				return this._optionDescription;
			}
			set
			{
				if (this._optionDescription != value)
				{
					this._optionDescription = value;
				}
			}
		}

		private ButtonWidget _booleanToggleButtonWidget;

		private AnimatedDropdownWidget _dropdownWidget;

		private OptionsScreenWidget _screenWidget;

		private Widget _dropdownExtensionParentWidget;

		private bool _eventsRegistered;

		private bool _initialized;

		private List<Sprite> _graphicsSprites;

		private bool _isEnabledStateDirty = true;

		private int _optionTypeID;

		private string _optionDescription;

		private string _optionTitle;

		private string[] _imageIDs;

		private bool _isOptionEnabled;
	}
}
