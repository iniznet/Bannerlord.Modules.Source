using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerFactionBannerWidget : Widget
	{
		public MultiplayerFactionBannerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._firstFrame)
			{
				this.UpdateBanner();
				this.UpdateIcon();
				this._firstFrame = false;
			}
		}

		private void UpdateBanner()
		{
			if (this._bannerWidget == null)
			{
				return;
			}
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
							styleLayer.Color = this.CultureColor1;
						}
					}
					return;
				}
			}
			this.BannerWidget.Color = this.CultureColor1;
		}

		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.FactionCode) || this._iconWidget == null)
			{
				return;
			}
			this.IconWidget.Sprite = base.Context.SpriteData.GetSprite("StdAssets\\FactionIcons\\LargeIcons\\" + this.FactionCode);
			this.IconWidget.Color = this.CultureColor2;
		}

		[DataSourceProperty]
		public Color CultureColor1
		{
			get
			{
				return this._cultureColor1;
			}
			set
			{
				if (value != this._cultureColor1)
				{
					this._cultureColor1 = value;
					base.OnPropertyChanged(value, "CultureColor1");
					this.UpdateBanner();
				}
			}
		}

		[DataSourceProperty]
		public Color CultureColor2
		{
			get
			{
				return this._cultureColor2;
			}
			set
			{
				if (value != this._cultureColor2)
				{
					this._cultureColor2 = value;
					base.OnPropertyChanged(value, "CultureColor2");
					this.UpdateIcon();
				}
			}
		}

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
				}
			}
		}

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

		private bool _firstFrame = true;

		private Color _cultureColor1;

		private Color _cultureColor2;

		private string _factionCode;

		private Widget _bannerWidget;

		private Widget _iconWidget;
	}
}
