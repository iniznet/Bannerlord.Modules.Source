using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Options
{
	// Token: 0x02000168 RID: 360
	public class CharacterCreationOptionsItemWidget : Widget
	{
		// Token: 0x0600127F RID: 4735 RVA: 0x000330F0 File Offset: 0x000312F0
		public CharacterCreationOptionsItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x00033100 File Offset: 0x00031300
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

		// Token: 0x06001281 RID: 4737 RVA: 0x00033218 File Offset: 0x00031418
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

		// Token: 0x06001282 RID: 4738 RVA: 0x000332DD File Offset: 0x000314DD
		protected override void OnGamepadNavigationIndexUpdated(int newIndex)
		{
			base.OnGamepadNavigationIndexUpdated(newIndex);
			this.ResetNavigationIndices();
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06001283 RID: 4739 RVA: 0x000332EC File Offset: 0x000314EC
		// (set) Token: 0x06001284 RID: 4740 RVA: 0x000332F4 File Offset: 0x000314F4
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

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06001285 RID: 4741 RVA: 0x00033319 File Offset: 0x00031519
		// (set) Token: 0x06001286 RID: 4742 RVA: 0x00033321 File Offset: 0x00031521
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

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06001287 RID: 4743 RVA: 0x0003333F File Offset: 0x0003153F
		// (set) Token: 0x06001288 RID: 4744 RVA: 0x00033347 File Offset: 0x00031547
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

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06001289 RID: 4745 RVA: 0x00033365 File Offset: 0x00031565
		// (set) Token: 0x0600128A RID: 4746 RVA: 0x0003336D File Offset: 0x0003156D
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

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x0600128B RID: 4747 RVA: 0x0003338B File Offset: 0x0003158B
		// (set) Token: 0x0600128C RID: 4748 RVA: 0x00033393 File Offset: 0x00031593
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

		// Token: 0x04000878 RID: 2168
		private bool _isDirty = true;

		// Token: 0x04000879 RID: 2169
		private int _type;

		// Token: 0x0400087A RID: 2170
		private Widget _actionOptionWidget;

		// Token: 0x0400087B RID: 2171
		private Widget _numericOptionWidget;

		// Token: 0x0400087C RID: 2172
		private Widget _selectionOptionWidget;

		// Token: 0x0400087D RID: 2173
		private Widget _booleanOptionWidget;
	}
}
