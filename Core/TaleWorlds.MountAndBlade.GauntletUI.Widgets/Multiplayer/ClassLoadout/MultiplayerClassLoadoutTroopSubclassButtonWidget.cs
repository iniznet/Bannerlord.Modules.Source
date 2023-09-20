using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutTroopSubclassButtonWidget : ButtonWidget
	{
		public MultiplayerClassLoadoutTroopSubclassButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.TroopType) || this._iconWidget == null)
			{
				return;
			}
			Sprite sprite = base.Context.SpriteData.GetSprite("General\\compass\\" + this.TroopType);
			foreach (Style style in this.IconWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Sprite = sprite;
				}
			}
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			Widget parentWidget = base.ParentWidget;
			if (parentWidget == null)
			{
				return;
			}
			parentWidget.SetState(base.CurrentState);
		}

		public override void SetState(string stateName)
		{
			base.SetState(stateName);
			if (this.PerksNavigationScopeTargeter != null)
			{
				this.PerksNavigationScopeTargeter.IsScopeEnabled = stateName == "Selected";
			}
		}

		[DataSourceProperty]
		public string TroopType
		{
			get
			{
				return this._troopType;
			}
			set
			{
				if (value != this._troopType)
				{
					this._troopType = value;
					base.OnPropertyChanged<string>(value, "TroopType");
					this.UpdateIcon();
				}
			}
		}

		[DataSourceProperty]
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

		public NavigationScopeTargeter PerksNavigationScopeTargeter
		{
			get
			{
				return this._perksNavigationScopeTargeter;
			}
			set
			{
				if (value != this._perksNavigationScopeTargeter)
				{
					this._perksNavigationScopeTargeter = value;
					base.OnPropertyChanged<NavigationScopeTargeter>(value, "PerksNavigationScopeTargeter");
					if (this._perksNavigationScopeTargeter != null)
					{
						this._perksNavigationScopeTargeter.IsScopeEnabled = false;
					}
				}
			}
		}

		private string _troopType;

		private BrushWidget _iconWidget;

		private NavigationScopeTargeter _perksNavigationScopeTargeter;
	}
}
