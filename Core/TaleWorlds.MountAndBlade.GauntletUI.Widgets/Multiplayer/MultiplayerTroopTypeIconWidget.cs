using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x02000082 RID: 130
	public class MultiplayerTroopTypeIconWidget : Widget
	{
		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000706 RID: 1798 RVA: 0x00014C34 File Offset: 0x00012E34
		// (set) Token: 0x06000707 RID: 1799 RVA: 0x00014C3C File Offset: 0x00012E3C
		public float ScaleFactor { get; set; } = 1f;

		// Token: 0x06000708 RID: 1800 RVA: 0x00014C45 File Offset: 0x00012E45
		public MultiplayerTroopTypeIconWidget(UIContext context)
			: base(context)
		{
			this.BackgroundWidget = this;
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00014C60 File Offset: 0x00012E60
		private void UpdateIcon()
		{
			if (this.BackgroundWidget == null || this.ForegroundWidget == null || string.IsNullOrEmpty(this.IconSpriteType))
			{
				return;
			}
			string text = "MPHud\\TroopIcons\\" + this.IconSpriteType;
			string text2 = text + "_Outline";
			this.ForegroundWidget.Sprite = base.Context.SpriteData.GetSprite(text);
			this.BackgroundWidget.Sprite = base.Context.SpriteData.GetSprite(text2);
			if (this.BackgroundWidget.Sprite != null)
			{
				float num = (float)this.BackgroundWidget.Sprite.Width;
				this.BackgroundWidget.SuggestedWidth = num * this.ScaleFactor;
				this.ForegroundWidget.SuggestedWidth = num * this.ScaleFactor;
				float num2 = (float)this.BackgroundWidget.Sprite.Height;
				this.BackgroundWidget.SuggestedHeight = num2 * this.ScaleFactor;
				this.ForegroundWidget.SuggestedHeight = num2 * this.ScaleFactor;
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x0600070A RID: 1802 RVA: 0x00014D5D File Offset: 0x00012F5D
		// (set) Token: 0x0600070B RID: 1803 RVA: 0x00014D65 File Offset: 0x00012F65
		[DataSourceProperty]
		public Widget BackgroundWidget
		{
			get
			{
				return this._backgroundWidget;
			}
			set
			{
				if (this._backgroundWidget != value)
				{
					this._backgroundWidget = value;
					base.OnPropertyChanged<Widget>(value, "BackgroundWidget");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600070C RID: 1804 RVA: 0x00014D89 File Offset: 0x00012F89
		// (set) Token: 0x0600070D RID: 1805 RVA: 0x00014D91 File Offset: 0x00012F91
		[DataSourceProperty]
		public Widget ForegroundWidget
		{
			get
			{
				return this._foregroundWidget;
			}
			set
			{
				if (this._foregroundWidget != value)
				{
					this._foregroundWidget = value;
					base.OnPropertyChanged<Widget>(value, "ForegroundWidget");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x0600070E RID: 1806 RVA: 0x00014DB5 File Offset: 0x00012FB5
		// (set) Token: 0x0600070F RID: 1807 RVA: 0x00014DBD File Offset: 0x00012FBD
		[DataSourceProperty]
		public string IconSpriteType
		{
			get
			{
				return this._iconSpriteType;
			}
			set
			{
				if (this._iconSpriteType != value)
				{
					this._iconSpriteType = value;
					base.OnPropertyChanged<string>(value, "IconSpriteType");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x04000320 RID: 800
		private Widget _backgroundWidget;

		// Token: 0x04000321 RID: 801
		private Widget _foregroundWidget;

		// Token: 0x04000322 RID: 802
		private string _iconSpriteType;
	}
}
