using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	public class ScoreboardBattleResultTitleBackgroundWidget : Widget
	{
		public ScoreboardBattleResultTitleBackgroundWidget(UIContext context)
			: base(context)
		{
		}

		private void BattleResultUpdated()
		{
			this.DefeatWidget.IsVisible = this.BattleResult == 0;
			this.VictoryWidget.IsVisible = this.BattleResult == 1;
			this.RetreatWidget.IsVisible = this.BattleResult == 2;
		}

		[Editor(false)]
		public int BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (this._battleResult != value)
				{
					this._battleResult = value;
					base.OnPropertyChanged(value, "BattleResult");
					this.BattleResultUpdated();
				}
			}
		}

		[Editor(false)]
		public Widget VictoryWidget
		{
			get
			{
				return this._victoryWidget;
			}
			set
			{
				if (this._victoryWidget != value)
				{
					this._victoryWidget = value;
					base.OnPropertyChanged<Widget>(value, "VictoryWidget");
				}
			}
		}

		[Editor(false)]
		public Widget DefeatWidget
		{
			get
			{
				return this._defeatWidget;
			}
			set
			{
				if (this._defeatWidget != value)
				{
					this._defeatWidget = value;
					base.OnPropertyChanged<Widget>(value, "DefeatWidget");
				}
			}
		}

		[Editor(false)]
		public Widget RetreatWidget
		{
			get
			{
				return this._retreatWidget;
			}
			set
			{
				if (this._retreatWidget != value)
				{
					this._retreatWidget = value;
					base.OnPropertyChanged<Widget>(value, "RetreatWidget");
				}
			}
		}

		private int _battleResult;

		private Widget _victoryWidget;

		private Widget _defeatWidget;

		private Widget _retreatWidget;
	}
}
