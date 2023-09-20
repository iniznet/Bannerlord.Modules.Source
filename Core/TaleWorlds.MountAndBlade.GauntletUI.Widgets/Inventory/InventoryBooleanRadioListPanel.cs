using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x0200011A RID: 282
	public class InventoryBooleanRadioListPanel : ListPanel
	{
		// Token: 0x06000E59 RID: 3673 RVA: 0x00027D76 File Offset: 0x00025F76
		public InventoryBooleanRadioListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x00027D80 File Offset: 0x00025F80
		private void UpdateChildSelectedState()
		{
			if (base.ChildCount < 2)
			{
				return;
			}
			ButtonWidget buttonWidget = base.GetChild(1) as ButtonWidget;
			ButtonWidget buttonWidget2 = base.GetChild(0) as ButtonWidget;
			if (buttonWidget == null || buttonWidget2 == null)
			{
				return;
			}
			buttonWidget.IsSelected = this.IsFirstSelected;
			buttonWidget2.IsSelected = !this.IsFirstSelected;
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x00027DD4 File Offset: 0x00025FD4
		public override void OnChildSelected(Widget widget)
		{
			base.OnChildSelected(widget);
			int childIndex = base.GetChildIndex(widget);
			this.IsFirstSelected = childIndex == 1;
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00027DFA File Offset: 0x00025FFA
		// (set) Token: 0x06000E5D RID: 3677 RVA: 0x00027E02 File Offset: 0x00026002
		[Editor(false)]
		public bool IsFirstSelected
		{
			get
			{
				return this._isFirstSelected;
			}
			set
			{
				if (this._isFirstSelected != value || !this._isSelectedStateSet)
				{
					this._isFirstSelected = value;
					base.OnPropertyChanged(value, "IsFirstSelected");
					this._isSelectedStateSet = true;
					this.UpdateChildSelectedState();
				}
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x00027E35 File Offset: 0x00026035
		// (set) Token: 0x06000E5F RID: 3679 RVA: 0x00027E40 File Offset: 0x00026040
		[Editor(false)]
		public bool IsSecondSelected
		{
			get
			{
				return !this._isFirstSelected;
			}
			set
			{
				if (this._isFirstSelected != !value)
				{
					this.IsFirstSelected = !value;
					base.OnPropertyChanged(!value, "IsSecondSelected");
				}
			}
		}

		// Token: 0x04000696 RID: 1686
		private bool _isSelectedStateSet;

		// Token: 0x04000697 RID: 1687
		private bool _isFirstSelected;
	}
}
