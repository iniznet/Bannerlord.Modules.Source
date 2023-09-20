using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000BC RID: 188
	public class MultiplayerClassLoadoutTroopInfoBrushWidget : BrushWidget
	{
		// Token: 0x060009A5 RID: 2469 RVA: 0x0001B9F8 File Offset: 0x00019BF8
		public MultiplayerClassLoadoutTroopInfoBrushWidget(UIContext context)
			: base(context)
		{
			this.SetAlpha(this.DefaultAlpha);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x0001BA18 File Offset: 0x00019C18
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetAlpha(1f);
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0001BA2B File Offset: 0x00019C2B
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.SetAlpha(this.DefaultAlpha);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x0001BA3F File Offset: 0x00019C3F
		public override void OnBrushChanged()
		{
			base.OnBrushChanged();
			this.SetAlpha(this.DefaultAlpha);
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060009A9 RID: 2473 RVA: 0x0001BA53 File Offset: 0x00019C53
		// (set) Token: 0x060009AA RID: 2474 RVA: 0x0001BA5B File Offset: 0x00019C5B
		[Editor(false)]
		public float DefaultAlpha
		{
			get
			{
				return this._defaultAlpha;
			}
			set
			{
				if (value != this._defaultAlpha)
				{
					this._defaultAlpha = value;
					base.OnPropertyChanged(value, "DefaultAlpha");
				}
			}
		}

		// Token: 0x0400046D RID: 1133
		private float _defaultAlpha = 0.7f;
	}
}
