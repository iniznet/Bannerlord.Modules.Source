using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000B7 RID: 183
	public class ClassLoadoutAlternativeUsageItemTabButtonWidget : ButtonWidget
	{
		// Token: 0x0600097C RID: 2428 RVA: 0x0001B234 File Offset: 0x00019434
		public ClassLoadoutAlternativeUsageItemTabButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0001B240 File Offset: 0x00019440
		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.UsageType) || this._iconWidget == null)
			{
				return;
			}
			Sprite sprite = base.Context.SpriteData.GetSprite("MPClassLoadout\\UsageIcons\\" + this.UsageType);
			foreach (Style style in this.IconWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Sprite = sprite;
				}
			}
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0001B30C File Offset: 0x0001950C
		protected override void RefreshState()
		{
			base.RefreshState();
			if (base.IsSelected && base.ParentWidget is Container)
			{
				(base.ParentWidget as Container).OnChildSelected(this);
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x0001B33A File Offset: 0x0001953A
		// (set) Token: 0x06000980 RID: 2432 RVA: 0x0001B342 File Offset: 0x00019542
		public string UsageType
		{
			get
			{
				return this._usageType;
			}
			set
			{
				if (value != this._usageType)
				{
					this._usageType = value;
					base.OnPropertyChanged<string>(value, "UsageType");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x0001B36B File Offset: 0x0001956B
		// (set) Token: 0x06000982 RID: 2434 RVA: 0x0001B373 File Offset: 0x00019573
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

		// Token: 0x0400045E RID: 1118
		private string _usageType;

		// Token: 0x0400045F RID: 1119
		private BrushWidget _iconWidget;
	}
}
