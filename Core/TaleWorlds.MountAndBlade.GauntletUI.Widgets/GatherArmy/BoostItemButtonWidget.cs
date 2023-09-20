using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GatherArmy
{
	// Token: 0x0200012D RID: 301
	public class BoostItemButtonWidget : ButtonWidget
	{
		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06000FE7 RID: 4071 RVA: 0x0002D15E File Offset: 0x0002B35E
		// (set) Token: 0x06000FE8 RID: 4072 RVA: 0x0002D166 File Offset: 0x0002B366
		public BoostCohesionPopupWidget ParentPopupWidget { get; private set; }

		// Token: 0x06000FE9 RID: 4073 RVA: 0x0002D16F File Offset: 0x0002B36F
		public BoostItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x0002D180 File Offset: 0x0002B380
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.BoostCurrencyIconWidget != null)
			{
				int boostCurrencyType = this.BoostCurrencyType;
				if (boostCurrencyType != 0)
				{
					if (boostCurrencyType == 1)
					{
						this.BoostCurrencyIconWidget.SetState("Influence");
					}
				}
				else
				{
					this.BoostCurrencyIconWidget.SetState("Gold");
				}
			}
			if (this.ParentPopupWidget == null)
			{
				this.ParentPopupWidget = this.FindParentPopupWidget();
				if (this.ParentPopupWidget != null)
				{
					this.ClickEventHandlers.Add(new Action<Widget>(this.ParentPopupWidget.ClosePopup));
				}
			}
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x0002D208 File Offset: 0x0002B408
		private BoostCohesionPopupWidget FindParentPopupWidget()
		{
			Widget widget = this;
			while (widget != base.EventManager.Root && this.ParentPopupWidget == null)
			{
				if (widget is BoostCohesionPopupWidget)
				{
					return widget as BoostCohesionPopupWidget;
				}
				widget = widget.ParentWidget;
			}
			return null;
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x0002D246 File Offset: 0x0002B446
		// (set) Token: 0x06000FED RID: 4077 RVA: 0x0002D24E File Offset: 0x0002B44E
		[Editor(false)]
		public int BoostCurrencyType
		{
			get
			{
				return this._boostCurrencyType;
			}
			set
			{
				if (this._boostCurrencyType != value)
				{
					this._boostCurrencyType = value;
					base.OnPropertyChanged(value, "BoostCurrencyType");
				}
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0002D26C File Offset: 0x0002B46C
		// (set) Token: 0x06000FEF RID: 4079 RVA: 0x0002D274 File Offset: 0x0002B474
		[Editor(false)]
		public Widget BoostCurrencyIconWidget
		{
			get
			{
				return this._boostCurrencyIconWidget;
			}
			set
			{
				if (this._boostCurrencyIconWidget != value)
				{
					this._boostCurrencyIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "BoostCurrencyIconWidget");
				}
			}
		}

		// Token: 0x0400075B RID: 1883
		private int _boostCurrencyType = -1;

		// Token: 0x0400075C RID: 1884
		private Widget _boostCurrencyIconWidget;
	}
}
