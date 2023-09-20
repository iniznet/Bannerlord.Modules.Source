using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerItemTabButtonWidget : ButtonWidget
	{
		public MultiplayerItemTabButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.ItemType) || this._iconWidget == null)
			{
				return;
			}
			Sprite sprite = base.Context.SpriteData.GetSprite("StdAssets\\ItemIcons\\" + this.ItemType);
			this.IconWidget.Brush.DefaultLayer.Sprite = sprite;
			Sprite sprite2 = base.Context.SpriteData.GetSprite("StdAssets\\ItemIcons\\" + this.ItemType + "_selected");
			this.IconWidget.Brush.GetLayer("Selected").Sprite = sprite2;
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			if (base.IsSelected && base.ParentWidget is Container)
			{
				(base.ParentWidget as Container).OnChildSelected(this);
			}
		}

		[Editor(false)]
		public string ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (value != this._itemType)
				{
					this._itemType = value;
					base.OnPropertyChanged<string>(value, "ItemType");
					this.UpdateIcon();
				}
			}
		}

		[Editor(false)]
		public BrushWidget IconWidget
		{
			get
			{
				return this._iconWidget;
			}
			set
			{
				if (value != this._iconWidget)
				{
					this._iconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "IconWidget");
					this.UpdateIcon();
				}
			}
		}

		private const string BaseSpritePath = "StdAssets\\ItemIcons\\";

		private string _itemType;

		private BrushWidget _iconWidget;
	}
}
