using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaDividerButtonWidget : ButtonWidget
	{
		public EncyclopediaDividerButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnClick()
		{
			base.OnClick();
			this.UpdateItemListVisibility();
			this.UpdateCollapseIndicator();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.IsVisible = this.ItemListWidget.ChildCount > 0;
			this.UpdateCollapseIndicator();
		}

		private void UpdateItemListVisibility()
		{
			if (this.ItemListWidget != null && this.ItemListWidget != null)
			{
				this.ItemListWidget.IsVisible = !this.ItemListWidget.IsVisible;
			}
		}

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

		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

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

		private Widget _itemListWidget;

		private Widget _collapseIndicator;
	}
}
