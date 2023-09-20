using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Credits
{
	// Token: 0x0200013F RID: 319
	public class CreditsWidget : Widget
	{
		// Token: 0x060010C4 RID: 4292 RVA: 0x0002EEC2 File Offset: 0x0002D0C2
		public CreditsWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x0002EED8 File Offset: 0x0002D0D8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.RootItemWidget != null)
			{
				this.RootItemWidget.PositionYOffset = this._currentOffset;
				this._currentOffset -= dt * 75f;
				if (this._currentOffset < -this.RootItemWidget.Size.Y * base._inverseScaleToUse)
				{
					this._currentOffset = 1080f;
				}
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x060010C6 RID: 4294 RVA: 0x0002EF44 File Offset: 0x0002D144
		// (set) Token: 0x060010C7 RID: 4295 RVA: 0x0002EF4C File Offset: 0x0002D14C
		[Editor(false)]
		public Widget RootItemWidget
		{
			get
			{
				return this._rootItemWidget;
			}
			set
			{
				if (this._rootItemWidget != value)
				{
					this._rootItemWidget = value;
					base.OnPropertyChanged<Widget>(value, "RootItemWidget");
				}
			}
		}

		// Token: 0x040007B3 RID: 1971
		private float _currentOffset = 1080f;

		// Token: 0x040007B4 RID: 1972
		private Widget _rootItemWidget;
	}
}
