using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerBattleResultColorizedWidget : Widget
	{
		public MultiplayerBattleResultColorizedWidget(UIContext context)
			: base(context)
		{
		}

		private void BattleResultUpdated()
		{
			if (this.BattleResult == 2)
			{
				base.Color = this.DrawColor;
				return;
			}
			if (this.BattleResult == 1)
			{
				base.Color = this.VictoryColor;
				return;
			}
			if (this.BattleResult == 0)
			{
				base.Color = this.DefeatColor;
			}
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
		public Color DrawColor
		{
			get
			{
				return this._drawColor;
			}
			set
			{
				if (this._drawColor != value)
				{
					this._drawColor = value;
					base.OnPropertyChanged(value, "DrawColor");
				}
			}
		}

		[Editor(false)]
		public Color VictoryColor
		{
			get
			{
				return this._victoryColor;
			}
			set
			{
				if (this._victoryColor != value)
				{
					this._victoryColor = value;
					base.OnPropertyChanged(value, "VictoryColor");
				}
			}
		}

		[Editor(false)]
		public Color DefeatColor
		{
			get
			{
				return this._defeatColor;
			}
			set
			{
				if (this._defeatColor != value)
				{
					this._defeatColor = value;
					base.OnPropertyChanged(value, "DefeatColor");
				}
			}
		}

		private int _battleResult = -1;

		private Color _drawColor;

		private Color _victoryColor;

		private Color _defeatColor;
	}
}
