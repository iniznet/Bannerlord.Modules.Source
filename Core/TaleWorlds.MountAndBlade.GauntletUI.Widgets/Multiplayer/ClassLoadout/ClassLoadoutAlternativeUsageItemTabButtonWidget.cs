using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class ClassLoadoutAlternativeUsageItemTabButtonWidget : ButtonWidget
	{
		public ClassLoadoutAlternativeUsageItemTabButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		protected override void RefreshState()
		{
			base.RefreshState();
			if (base.IsSelected && base.ParentWidget is Container)
			{
				(base.ParentWidget as Container).OnChildSelected(this);
			}
		}

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

		private string _usageType;

		private BrushWidget _iconWidget;
	}
}
