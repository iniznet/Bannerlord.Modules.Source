using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleHeroButtonWidget : ButtonWidget
	{
		public OrderOfBattleHeroButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void OnHeroTypeChanged()
		{
			foreach (BrushLayer brushLayer in base.Brush.Layers)
			{
				brushLayer.HueFactor = (float)(this.IsMainHero ? this.MainHeroHueFactor : 0);
			}
		}

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

		private bool _isMainHero;

		private int _mainHeroHueFactor;
	}
}
