using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x0200007B RID: 123
	public class MultiplayerEndOfRoundPanelBrushWidget : BrushWidget
	{
		// Token: 0x060006D4 RID: 1748 RVA: 0x00014577 File Offset: 0x00012777
		public MultiplayerEndOfRoundPanelBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00014580 File Offset: 0x00012780
		private void IsShownUpdated()
		{
			if (this.IsShown)
			{
				string text = (this.IsRoundWinner ? "Victory" : "Defeat");
				base.EventFired(text, Array.Empty<object>());
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x000145B6 File Offset: 0x000127B6
		// (set) Token: 0x060006D7 RID: 1751 RVA: 0x000145BE File Offset: 0x000127BE
		[DataSourceProperty]
		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (value != this._isShown)
				{
					this._isShown = value;
					base.OnPropertyChanged(value, "IsShown");
					this.IsShownUpdated();
				}
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x000145E2 File Offset: 0x000127E2
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x000145EA File Offset: 0x000127EA
		[DataSourceProperty]
		public bool IsRoundWinner
		{
			get
			{
				return this._isRoundWinner;
			}
			set
			{
				if (value != this._isRoundWinner)
				{
					this._isRoundWinner = value;
					base.OnPropertyChanged(value, "IsRoundWinner");
				}
			}
		}

		// Token: 0x04000307 RID: 775
		private bool _isShown;

		// Token: 0x04000308 RID: 776
		private bool _isRoundWinner;
	}
}
