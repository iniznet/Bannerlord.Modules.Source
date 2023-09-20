using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	// Token: 0x02000049 RID: 73
	public class ScoreboardBattleResultTitleBackgroundWidget : Widget
	{
		// Token: 0x060003DD RID: 989 RVA: 0x0000CB17 File Offset: 0x0000AD17
		public ScoreboardBattleResultTitleBackgroundWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0000CB20 File Offset: 0x0000AD20
		private void BattleResultUpdated()
		{
			this.DefeatWidget.IsVisible = this.BattleResult == 0;
			this.VictoryWidget.IsVisible = this.BattleResult == 1;
			this.RetreatWidget.IsVisible = this.BattleResult == 2;
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060003DF RID: 991 RVA: 0x0000CB5E File Offset: 0x0000AD5E
		// (set) Token: 0x060003E0 RID: 992 RVA: 0x0000CB66 File Offset: 0x0000AD66
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

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x0000CB8A File Offset: 0x0000AD8A
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x0000CB92 File Offset: 0x0000AD92
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

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x0000CBB0 File Offset: 0x0000ADB0
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x0000CBB8 File Offset: 0x0000ADB8
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

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x0000CBD6 File Offset: 0x0000ADD6
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x0000CBDE File Offset: 0x0000ADDE
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

		// Token: 0x040001AD RID: 429
		private int _battleResult;

		// Token: 0x040001AE RID: 430
		private Widget _victoryWidget;

		// Token: 0x040001AF RID: 431
		private Widget _defeatWidget;

		// Token: 0x040001B0 RID: 432
		private Widget _retreatWidget;
	}
}
