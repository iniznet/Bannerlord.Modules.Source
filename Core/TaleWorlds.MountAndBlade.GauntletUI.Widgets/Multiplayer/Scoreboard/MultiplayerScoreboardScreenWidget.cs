using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x02000086 RID: 134
	public class MultiplayerScoreboardScreenWidget : Widget
	{
		// Token: 0x0600072D RID: 1837 RVA: 0x00015613 File Offset: 0x00013813
		public MultiplayerScoreboardScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0001561C File Offset: 0x0001381C
		private void UpdateSidesList()
		{
			if (this.SidesList == null)
			{
				return;
			}
			base.SuggestedWidth = (float)(this.IsSingleSide ? this.SingleColumnedWidth : this.DoubleColumnedWidth);
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x00015644 File Offset: 0x00013844
		// (set) Token: 0x06000730 RID: 1840 RVA: 0x0001564C File Offset: 0x0001384C
		[DataSourceProperty]
		public bool IsSingleSide
		{
			get
			{
				return this._isSingleSide;
			}
			set
			{
				if (value != this._isSingleSide)
				{
					this._isSingleSide = value;
					base.OnPropertyChanged(value, "IsSingleSide");
					this.UpdateSidesList();
				}
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000731 RID: 1841 RVA: 0x00015670 File Offset: 0x00013870
		// (set) Token: 0x06000732 RID: 1842 RVA: 0x00015678 File Offset: 0x00013878
		[DataSourceProperty]
		public int SingleColumnedWidth
		{
			get
			{
				return this._singleColumnedWidth;
			}
			set
			{
				if (value != this._singleColumnedWidth)
				{
					this._singleColumnedWidth = value;
					base.OnPropertyChanged(value, "SingleColumnedWidth");
				}
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x00015696 File Offset: 0x00013896
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x0001569E File Offset: 0x0001389E
		[DataSourceProperty]
		public int DoubleColumnedWidth
		{
			get
			{
				return this._doubleColumnedWidth;
			}
			set
			{
				if (value != this._doubleColumnedWidth)
				{
					this._doubleColumnedWidth = value;
					base.OnPropertyChanged(value, "DoubleColumnedWidth");
				}
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x000156BC File Offset: 0x000138BC
		// (set) Token: 0x06000736 RID: 1846 RVA: 0x000156C4 File Offset: 0x000138C4
		[DataSourceProperty]
		public ListPanel SidesList
		{
			get
			{
				return this._sidesList;
			}
			set
			{
				if (value != this._sidesList)
				{
					this._sidesList = value;
					base.OnPropertyChanged<ListPanel>(value, "SidesList");
					this.UpdateSidesList();
				}
			}
		}

		// Token: 0x0400033B RID: 827
		private bool _isSingleSide;

		// Token: 0x0400033C RID: 828
		private int _singleColumnedWidth;

		// Token: 0x0400033D RID: 829
		private int _doubleColumnedWidth;

		// Token: 0x0400033E RID: 830
		private ListPanel _sidesList;
	}
}
