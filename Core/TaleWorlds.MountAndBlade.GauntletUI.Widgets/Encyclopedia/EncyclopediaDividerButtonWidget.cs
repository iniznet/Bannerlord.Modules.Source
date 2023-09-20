using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x02000136 RID: 310
	public class EncyclopediaDividerButtonWidget : ButtonWidget
	{
		// Token: 0x0600106A RID: 4202 RVA: 0x0002E1C4 File Offset: 0x0002C3C4
		public EncyclopediaDividerButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x0002E1CD File Offset: 0x0002C3CD
		protected override void OnClick()
		{
			base.OnClick();
			this.UpdateItemListVisibility();
			this.UpdateCollapseIndicator();
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x0002E1E1 File Offset: 0x0002C3E1
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.IsVisible = this.ItemListWidget.ChildCount > 0;
			this.UpdateCollapseIndicator();
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0002E204 File Offset: 0x0002C404
		private void UpdateItemListVisibility()
		{
			if (this.ItemListWidget != null && this.ItemListWidget != null)
			{
				this.ItemListWidget.IsVisible = !this.ItemListWidget.IsVisible;
			}
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x0002E230 File Offset: 0x0002C430
		private void UpdateCollapseIndicator()
		{
			if (this.ItemListWidget != null && this.ItemListWidget != null && this.CollapseIndicator != null)
			{
				if (this.ItemListWidget.IsVisible)
				{
					this.CollapseIndicator.SetState("Expanded");
					return;
				}
				this.CollapseIndicator.SetState("Collapsed");
			}
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x0002E283 File Offset: 0x0002C483
		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06001070 RID: 4208 RVA: 0x0002E2AB File Offset: 0x0002C4AB
		// (set) Token: 0x06001071 RID: 4209 RVA: 0x0002E2B3 File Offset: 0x0002C4B3
		public Widget ItemListWidget
		{
			get
			{
				return this._itemListWidget;
			}
			set
			{
				if (value != this._itemListWidget)
				{
					this._itemListWidget = value;
					base.OnPropertyChanged<Widget>(value, "ItemListWidget");
				}
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x0002E2D1 File Offset: 0x0002C4D1
		// (set) Token: 0x06001073 RID: 4211 RVA: 0x0002E2D9 File Offset: 0x0002C4D9
		public Widget CollapseIndicator
		{
			get
			{
				return this._collapseIndicator;
			}
			set
			{
				if (value != this._collapseIndicator)
				{
					this._collapseIndicator = value;
					base.OnPropertyChanged<Widget>(value, "CollapseIndicator");
					this.CollapseIndicatorUpdated();
				}
			}
		}

		// Token: 0x04000790 RID: 1936
		private Widget _itemListWidget;

		// Token: 0x04000791 RID: 1937
		private Widget _collapseIndicator;
	}
}
