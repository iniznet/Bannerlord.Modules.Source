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
				this._firstFrame = false;
			}
		}

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

		private bool _firstFrame = true;

		private string _factionCode;

		private bool _useSecondary;

		private Widget _bannerWidget;

		private Widget _iconWidget;
	}
}
