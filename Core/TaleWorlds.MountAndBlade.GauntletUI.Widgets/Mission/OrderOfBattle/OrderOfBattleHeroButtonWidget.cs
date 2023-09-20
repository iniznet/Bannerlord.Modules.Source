using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D5 RID: 213
	public class OrderOfBattleHeroButtonWidget : ButtonWidget
	{
		// Token: 0x06000ADC RID: 2780 RVA: 0x0001E2EE File Offset: 0x0001C4EE
		public OrderOfBattleHeroButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x0001E2F8 File Offset: 0x0001C4F8
		private void OnHeroTypeChanged()
		{
			foreach (BrushLayer brushLayer in base.Brush.Layers)
			{
				brushLayer.HueFactor = (float)(this.IsMainHero ? this.MainHeroHueFactor : 0);
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x0001E360 File Offset: 0x0001C560
		// (set) Token: 0x06000ADF RID: 2783 RVA: 0x0001E368 File Offset: 0x0001C568
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChanged(value, "IsMainHero");
					this.OnHeroTypeChanged();
				}
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x0001E38C File Offset: 0x0001C58C
		// (set) Token: 0x06000AE1 RID: 2785 RVA: 0x0001E394 File Offset: 0x0001C594
		public int MainHeroHueFactor
		{
			get
			{
				return this._mainHeroHueFactor;
			}
			set
			{
				if (value != this._mainHeroHueFactor)
				{
					this._mainHeroHueFactor = value;
					base.OnPropertyChanged(value, "MainHeroHueFactor");
				}
			}
		}

		// Token: 0x040004F3 RID: 1267
		private bool _isMainHero;

		// Token: 0x040004F4 RID: 1268
		private int _mainHeroHueFactor;
	}
}
