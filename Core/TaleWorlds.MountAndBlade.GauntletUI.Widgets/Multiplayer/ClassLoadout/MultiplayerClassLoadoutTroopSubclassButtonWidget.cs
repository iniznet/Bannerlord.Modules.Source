using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000BD RID: 189
	public class MultiplayerClassLoadoutTroopSubclassButtonWidget : ButtonWidget
	{
		// Token: 0x060009AB RID: 2475 RVA: 0x0001BA79 File Offset: 0x00019C79
		public MultiplayerClassLoadoutTroopSubclassButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x0001BA84 File Offset: 0x00019C84
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

		// Token: 0x060009AD RID: 2477 RVA: 0x0001BB50 File Offset: 0x00019D50
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

		// Token: 0x060009AE RID: 2478 RVA: 0x0001BB6F File Offset: 0x00019D6F
		public override void SetState(string stateName)
		{
			base.SetState(stateName);
			if (this.PerksNavigationScopeTargeter != null)
			{
				this.PerksNavigationScopeTargeter.IsScopeEnabled = stateName == "Selected";
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060009AF RID: 2479 RVA: 0x0001BB96 File Offset: 0x00019D96
		// (set) Token: 0x060009B0 RID: 2480 RVA: 0x0001BB9E File Offset: 0x00019D9E
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

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060009B1 RID: 2481 RVA: 0x0001BBC7 File Offset: 0x00019DC7
		// (set) Token: 0x060009B2 RID: 2482 RVA: 0x0001BBCF File Offset: 0x00019DCF
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

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x0001BBF3 File Offset: 0x00019DF3
		// (set) Token: 0x060009B4 RID: 2484 RVA: 0x0001BBFB File Offset: 0x00019DFB
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

		// Token: 0x0400046E RID: 1134
		private string _troopType;

		// Token: 0x0400046F RID: 1135
		private BrushWidget _iconWidget;

		// Token: 0x04000470 RID: 1136
		private NavigationScopeTargeter _perksNavigationScopeTargeter;
	}
}
