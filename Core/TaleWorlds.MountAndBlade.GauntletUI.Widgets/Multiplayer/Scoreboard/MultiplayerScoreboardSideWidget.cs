using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x02000087 RID: 135
	public class MultiplayerScoreboardSideWidget : Widget
	{
		// Token: 0x06000737 RID: 1847 RVA: 0x000156E8 File Offset: 0x000138E8
		public MultiplayerScoreboardSideWidget(UIContext context)
			: base(context)
		{
			this._nameColumnItemDescription = new ContainerItemDescription();
			this._nameColumnItemDescription.WidgetIndex = 3;
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00015713 File Offset: 0x00013913
		private void AvatarColumnWidthRatioUpdated()
		{
			if (this.TitlesListPanel == null)
			{
				return;
			}
			this._nameColumnItemDescription.WidthStretchRatio = this.NameColumnWidthRatio;
			this.TitlesListPanel.AddItemDescription(this._nameColumnItemDescription);
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00015740 File Offset: 0x00013940
		private void UpdateBackgroundColors()
		{
			if (string.IsNullOrEmpty(this.CultureId))
			{
				return;
			}
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(this.CultureId.ToLower(), this.UseSecondary);
			base.Color = Color.ConvertStringToColor(factionColorCode);
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0001577E File Offset: 0x0001397E
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x00015786 File Offset: 0x00013986
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChanged<string>(value, "CultureId");
					this.UpdateBackgroundColors();
				}
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x000157AF File Offset: 0x000139AF
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x000157B7 File Offset: 0x000139B7
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
					this.UpdateBackgroundColors();
				}
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x0600073E RID: 1854 RVA: 0x000157DB File Offset: 0x000139DB
		// (set) Token: 0x0600073F RID: 1855 RVA: 0x000157E3 File Offset: 0x000139E3
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
					this.AvatarColumnWidthRatioUpdated();
				}
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x00015807 File Offset: 0x00013A07
		// (set) Token: 0x06000741 RID: 1857 RVA: 0x0001580F File Offset: 0x00013A0F
		public ListPanel TitlesListPanel
		{
			get
			{
				return this._titlesListPanel;
			}
			set
			{
				if (value != this._titlesListPanel)
				{
					this._titlesListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "TitlesListPanel");
					this.AvatarColumnWidthRatioUpdated();
				}
			}
		}

		// Token: 0x0400033F RID: 831
		private ContainerItemDescription _nameColumnItemDescription;

		// Token: 0x04000340 RID: 832
		private float _nameColumnWidthRatio = 1f;

		// Token: 0x04000341 RID: 833
		private ListPanel _titlesListPanel;

		// Token: 0x04000342 RID: 834
		private string _cultureId;

		// Token: 0x04000343 RID: 835
		private bool _useSecondary;
	}
}
