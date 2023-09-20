using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x02000079 RID: 121
	public class MultiplayerBattleResultColorizedWidget : Widget
	{
		// Token: 0x060006C4 RID: 1732 RVA: 0x00014367 File Offset: 0x00012567
		public MultiplayerBattleResultColorizedWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00014378 File Offset: 0x00012578
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

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x000143C5 File Offset: 0x000125C5
		// (set) Token: 0x060006C7 RID: 1735 RVA: 0x000143CD File Offset: 0x000125CD
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

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x000143F1 File Offset: 0x000125F1
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x000143F9 File Offset: 0x000125F9
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

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x0001441C File Offset: 0x0001261C
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x00014424 File Offset: 0x00012624
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

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x00014447 File Offset: 0x00012647
		// (set) Token: 0x060006CD RID: 1741 RVA: 0x0001444F File Offset: 0x0001264F
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

		// Token: 0x040002FD RID: 765
		private int _battleResult = -1;

		// Token: 0x040002FE RID: 766
		private Color _drawColor;

		// Token: 0x040002FF RID: 767
		private Color _victoryColor;

		// Token: 0x04000300 RID: 768
		private Color _defeatColor;
	}
}
