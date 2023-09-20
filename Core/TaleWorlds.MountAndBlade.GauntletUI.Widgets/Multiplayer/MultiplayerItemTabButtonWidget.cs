using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x0200007E RID: 126
	public class MultiplayerItemTabButtonWidget : ButtonWidget
	{
		// Token: 0x060006EA RID: 1770 RVA: 0x000148B4 File Offset: 0x00012AB4
		public MultiplayerItemTabButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x000148C0 File Offset: 0x00012AC0
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

		// Token: 0x060006EC RID: 1772 RVA: 0x0001495B File Offset: 0x00012B5B
		protected override void RefreshState()
		{
			base.RefreshState();
			if (base.IsSelected && base.ParentWidget is Container)
			{
				(base.ParentWidget as Container).OnChildSelected(this);
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060006ED RID: 1773 RVA: 0x00014989 File Offset: 0x00012B89
		// (set) Token: 0x060006EE RID: 1774 RVA: 0x00014991 File Offset: 0x00012B91
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

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x000149BA File Offset: 0x00012BBA
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x000149C2 File Offset: 0x00012BC2
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

		// Token: 0x0400030F RID: 783
		private const string BaseSpritePath = "StdAssets\\ItemIcons\\";

		// Token: 0x04000310 RID: 784
		private string _itemType;

		// Token: 0x04000311 RID: 785
		private BrushWidget _iconWidget;
	}
}
