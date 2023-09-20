using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	// Token: 0x0200006A RID: 106
	public class OptionsItemWidget : Widget
	{
		// Token: 0x170001FB RID: 507
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00010D96 File Offset: 0x0000EF96
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x00010D9E File Offset: 0x0000EF9E
		public Widget BooleanOption { get; set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x00010DA7 File Offset: 0x0000EFA7
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x00010DAF File Offset: 0x0000EFAF
		public Widget NumericOption { get; set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00010DB8 File Offset: 0x0000EFB8
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x00010DC0 File Offset: 0x0000EFC0
		public Widget StringOption { get; set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00010DC9 File Offset: 0x0000EFC9
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x00010DD1 File Offset: 0x0000EFD1
		public Widget GameKeyOption { get; set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00010DDA File Offset: 0x0000EFDA
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x00010DE2 File Offset: 0x0000EFE2
		public Widget ActionOption { get; set; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00010DEB File Offset: 0x0000EFEB
		// (set) Token: 0x060005AA RID: 1450 RVA: 0x00010DF3 File Offset: 0x0000EFF3
		public Widget InputOption { get; set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x00010DFC File Offset: 0x0000EFFC
		// (set) Token: 0x060005AC RID: 1452 RVA: 0x00010E04 File Offset: 0x0000F004
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

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060005AD RID: 1453 RVA: 0x00010E16 File Offset: 0x0000F016
		// (set) Token: 0x060005AE RID: 1454 RVA: 0x00010E1E File Offset: 0x0000F01E
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

		// Token: 0x060005AF RID: 1455 RVA: 0x00010E30 File Offset: 0x0000F030
		public OptionsItemWidget(UIContext context)
			: base(context)
		{
			this._optionTypeID = -1;
			this._graphicsSprites = new List<Sprite>();
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00010E54 File Offset: 0x0000F054
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

		// Token: 0x060005B1 RID: 1457 RVA: 0x00010F5C File Offset: 0x0000F15C
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetCurrentOption(false, false, -1);
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00010F6D File Offset: 0x0000F16D
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.ResetCurrentOption();
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00010F7C File Offset: 0x0000F17C
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

		// Token: 0x060005B4 RID: 1460 RVA: 0x00010FA8 File Offset: 0x0000F1A8
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

		// Token: 0x060005B5 RID: 1461 RVA: 0x00011093 File Offset: 0x0000F293
		public void SetCurrentScreenWidget(OptionsScreenWidget screenWidget)
		{
			this._screenWidget = screenWidget;
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x0001109C File Offset: 0x0000F29C
		private void ResetCurrentOption()
		{
			OptionsScreenWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(null, null);
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x000110B0 File Offset: 0x0000F2B0
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

		// Token: 0x060005B8 RID: 1464 RVA: 0x0001118C File Offset: 0x0000F38C
		private void BooleanOption_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsSelected")
			{
				this.SetCurrentOption(false, true, -1);
			}
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x000111A4 File Offset: 0x0000F3A4
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

		// Token: 0x060005BA RID: 1466 RVA: 0x000111C8 File Offset: 0x0000F3C8
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

		// Token: 0x060005BB RID: 1467 RVA: 0x00011208 File Offset: 0x0000F408
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

		// Token: 0x060005BC RID: 1468 RVA: 0x00011298 File Offset: 0x0000F498
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

		// Token: 0x060005BD RID: 1469 RVA: 0x000112F8 File Offset: 0x0000F4F8
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

		// Token: 0x060005BE RID: 1470 RVA: 0x0001143F File Offset: 0x0000F63F
		protected override void OnGamepadNavigationIndexUpdated(int newIndex)
		{
			if (this._initialized)
			{
				this.ResetNavigationIndices();
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0001144F File Offset: 0x0000F64F
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x00011457 File Offset: 0x0000F657
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

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x00011475 File Offset: 0x0000F675
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x0001147D File Offset: 0x0000F67D
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

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x000114A2 File Offset: 0x0000F6A2
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x000114AA File Offset: 0x0000F6AA
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

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x000114C1 File Offset: 0x0000F6C1
		// (set) Token: 0x060005C6 RID: 1478 RVA: 0x000114C9 File Offset: 0x0000F6C9
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

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x000114DB File Offset: 0x0000F6DB
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x000114E3 File Offset: 0x0000F6E3
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

		// Token: 0x04000272 RID: 626
		private ButtonWidget _booleanToggleButtonWidget;

		// Token: 0x04000273 RID: 627
		private AnimatedDropdownWidget _dropdownWidget;

		// Token: 0x04000274 RID: 628
		private OptionsScreenWidget _screenWidget;

		// Token: 0x04000275 RID: 629
		private Widget _dropdownExtensionParentWidget;

		// Token: 0x04000276 RID: 630
		private bool _eventsRegistered;

		// Token: 0x04000277 RID: 631
		private bool _initialized;

		// Token: 0x04000278 RID: 632
		private List<Sprite> _graphicsSprites;

		// Token: 0x04000279 RID: 633
		private bool _isEnabledStateDirty = true;

		// Token: 0x0400027A RID: 634
		private int _optionTypeID;

		// Token: 0x0400027B RID: 635
		private string _optionDescription;

		// Token: 0x0400027C RID: 636
		private string _optionTitle;

		// Token: 0x0400027D RID: 637
		private string[] _imageIDs;

		// Token: 0x0400027E RID: 638
		private bool _isOptionEnabled;
	}
}
