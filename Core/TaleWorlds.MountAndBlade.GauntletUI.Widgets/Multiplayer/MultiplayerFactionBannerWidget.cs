using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x0200007C RID: 124
	public class MultiplayerFactionBannerWidget : Widget
	{
		// Token: 0x060006DA RID: 1754 RVA: 0x00014608 File Offset: 0x00012808
		public MultiplayerFactionBannerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x00014618 File Offset: 0x00012818
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._firstFrame)
			{
				this.UpdateBanner();
				this._firstFrame = false;
			}
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x00014638 File Offset: 0x00012838
		private void UpdateBanner()
		{
			if (string.IsNullOrEmpty(this.FactionCode) || this._bannerWidget == null)
			{
				return;
			}
			Color color = Color.ConvertStringToColor(WidgetsMultiplayerHelper.GetFactionColorCode(this.FactionCode.ToLower(), this.UseSecondary));
			BrushWidget brushWidget;
			if ((brushWidget = this.BannerWidget as BrushWidget) != null)
			{
				using (Dictionary<string, Style>.ValueCollection.Enumerator enumerator = brushWidget.Brush.Styles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Style style = enumerator.Current;
						foreach (StyleLayer styleLayer in style.Layers)
						{
							styleLayer.Color = color;
						}
					}
					return;
				}
			}
			this.BannerWidget.Color = color;
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x00014714 File Offset: 0x00012914
		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.FactionCode) || this._iconWidget == null)
			{
				return;
			}
			this.IconWidget.Sprite = base.Context.SpriteData.GetSprite("StdAssets\\FactionIcons\\LargeIcons\\" + this.FactionCode);
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(this.FactionCode.ToLower(), !this.UseSecondary);
			this.IconWidget.Color = Color.ConvertStringToColor(factionColorCode);
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x0001478D File Offset: 0x0001298D
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x00014795 File Offset: 0x00012995
		[DataSourceProperty]
		public string FactionCode
		{
			get
			{
				return this._factionCode;
			}
			set
			{
				if (value != this._factionCode)
				{
					this._factionCode = value;
					base.OnPropertyChanged<string>(value, "FactionCode");
					this.UpdateIcon();
					this.UpdateBanner();
				}
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x000147C4 File Offset: 0x000129C4
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x000147CC File Offset: 0x000129CC
		[DataSourceProperty]
		public Widget BannerWidget
		{
			get
			{
				return this._bannerWidget;
			}
			set
			{
				if (value != this._bannerWidget)
				{
					this._bannerWidget = value;
					base.OnPropertyChanged<Widget>(value, "BannerWidget");
					this.UpdateBanner();
				}
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x000147F0 File Offset: 0x000129F0
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x000147F8 File Offset: 0x000129F8
		[DataSourceProperty]
		public Widget IconWidget
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
					base.OnPropertyChanged<Widget>(value, "IconWidget");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0001481C File Offset: 0x00012A1C
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x00014824 File Offset: 0x00012A24
		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
					this.UpdateBanner();
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x04000309 RID: 777
		private bool _firstFrame = true;

		// Token: 0x0400030A RID: 778
		private string _factionCode;

		// Token: 0x0400030B RID: 779
		private bool _useSecondary;

		// Token: 0x0400030C RID: 780
		private Widget _bannerWidget;

		// Token: 0x0400030D RID: 781
		private Widget _iconWidget;
	}
}
