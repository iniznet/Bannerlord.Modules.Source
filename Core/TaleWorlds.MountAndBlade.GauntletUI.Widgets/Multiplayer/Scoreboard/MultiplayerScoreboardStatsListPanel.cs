using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x02000088 RID: 136
	public class MultiplayerScoreboardStatsListPanel : ListPanel
	{
		// Token: 0x06000742 RID: 1858 RVA: 0x00015834 File Offset: 0x00013A34
		public MultiplayerScoreboardStatsListPanel(UIContext context)
			: base(context)
		{
			this._nameColumnItemDescription = new ContainerItemDescription
			{
				WidgetId = "name"
			};
			this._scoreColumnItemDescription = new ContainerItemDescription
			{
				WidgetId = "score"
			};
			this._soldiersColumnItemDescription = new ContainerItemDescription
			{
				WidgetId = "soldiers"
			};
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x000158AB File Offset: 0x00013AAB
		private void NameColumnWidthRatioUpdated()
		{
			this._nameColumnItemDescription.WidthStretchRatio = this.NameColumnWidthRatio;
			base.AddItemDescription(this._nameColumnItemDescription);
			base.SetMeasureAndLayoutDirty();
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x000158D0 File Offset: 0x00013AD0
		private void ScoreColumnWidthRatioUpdated()
		{
			this._scoreColumnItemDescription.WidthStretchRatio = this.ScoreColumnWidthRatio;
			base.AddItemDescription(this._scoreColumnItemDescription);
			base.SetMeasureAndLayoutDirty();
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x000158F5 File Offset: 0x00013AF5
		private void SoldiersColumnWidthRatioUpdated()
		{
			this._soldiersColumnItemDescription.WidthStretchRatio = this.SoldiersColumnWidthRatio;
			base.AddItemDescription(this._soldiersColumnItemDescription);
			base.SetMeasureAndLayoutDirty();
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0001591A File Offset: 0x00013B1A
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x00015922 File Offset: 0x00013B22
		public float NameColumnWidthRatio
		{
			get
			{
				return this._nameColumnWidthRatio;
			}
			set
			{
				if (value != this._nameColumnWidthRatio)
				{
					this._nameColumnWidthRatio = value;
					base.OnPropertyChanged(value, "NameColumnWidthRatio");
					this.NameColumnWidthRatioUpdated();
				}
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x00015946 File Offset: 0x00013B46
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x0001594E File Offset: 0x00013B4E
		public float ScoreColumnWidthRatio
		{
			get
			{
				return this._scoreColumnWidthRatio;
			}
			set
			{
				if (value != this._scoreColumnWidthRatio)
				{
					this._scoreColumnWidthRatio = value;
					base.OnPropertyChanged(value, "ScoreColumnWidthRatio");
					this.ScoreColumnWidthRatioUpdated();
				}
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x00015972 File Offset: 0x00013B72
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x0001597A File Offset: 0x00013B7A
		public float SoldiersColumnWidthRatio
		{
			get
			{
				return this._soldiersColumnWidthRatio;
			}
			set
			{
				if (value != this._soldiersColumnWidthRatio)
				{
					this._soldiersColumnWidthRatio = value;
					base.OnPropertyChanged(value, "SoldiersColumnWidthRatio");
					this.SoldiersColumnWidthRatioUpdated();
				}
			}
		}

		// Token: 0x04000344 RID: 836
		private ContainerItemDescription _nameColumnItemDescription;

		// Token: 0x04000345 RID: 837
		private ContainerItemDescription _scoreColumnItemDescription;

		// Token: 0x04000346 RID: 838
		private ContainerItemDescription _soldiersColumnItemDescription;

		// Token: 0x04000347 RID: 839
		private const string _nameColumnWidgetID = "name";

		// Token: 0x04000348 RID: 840
		private const string _scoreColumnWidgetID = "score";

		// Token: 0x04000349 RID: 841
		private const string _soldiersColumnWidgetID = "soldiers";

		// Token: 0x0400034A RID: 842
		private float _nameColumnWidthRatio = 1f;

		// Token: 0x0400034B RID: 843
		private float _scoreColumnWidthRatio = 1f;

		// Token: 0x0400034C RID: 844
		private float _soldiersColumnWidthRatio = 1f;
	}
}
