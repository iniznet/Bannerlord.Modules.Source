using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerTroopTypeIconWidget : Widget
	{
		public float ScaleFactor { get; set; } = 1f;

		public MultiplayerTroopTypeIconWidget(UIContext context)
			: base(context)
		{
			this.BackgroundWidget = this;
		}

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

		private Widget _backgroundWidget;

		private Widget _foregroundWidget;

		private string _iconSpriteType;
	}
}
