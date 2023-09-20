using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	// Token: 0x020000AC RID: 172
	public class MultiplayerDuelKillFeedItemWidget : MultiplayerGeneralKillFeedItemWidget
	{
		// Token: 0x060008D6 RID: 2262 RVA: 0x000194E2 File Offset: 0x000176E2
		public MultiplayerDuelKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x000194EB File Offset: 0x000176EB
		// (set) Token: 0x060008D8 RID: 2264 RVA: 0x000194F4 File Offset: 0x000176F4
		[Editor(false)]
		public bool IsEndOfDuel
		{
			get
			{
				return this._isEndOfDuel;
			}
			set
			{
				if (value != this._isEndOfDuel)
				{
					this._isEndOfDuel = value;
					base.OnPropertyChanged(value, "IsEndOfDuel");
					if (value)
					{
						BrushWidget background = this.Background;
						if (background != null)
						{
							background.SetState("EndOfDuel");
						}
						BrushWidget victimCompassBackground = this.VictimCompassBackground;
						if (victimCompassBackground != null)
						{
							victimCompassBackground.SetState("EndOfDuel");
						}
						BrushWidget murdererCompassBackground = this.MurdererCompassBackground;
						if (murdererCompassBackground != null)
						{
							murdererCompassBackground.SetState("EndOfDuel");
						}
						ScrollingRichTextWidget victimNameText = this.VictimNameText;
						if (victimNameText != null)
						{
							victimNameText.SetState("EndOfDuel");
						}
						ScrollingRichTextWidget murdererNameText = this.MurdererNameText;
						if (murdererNameText != null)
						{
							murdererNameText.SetState("EndOfDuel");
						}
						TextWidget victimScoreText = this.VictimScoreText;
						if (victimScoreText != null)
						{
							victimScoreText.SetState("EndOfDuel");
						}
						TextWidget murdererScoreText = this.MurdererScoreText;
						if (murdererScoreText == null)
						{
							return;
						}
						murdererScoreText.SetState("EndOfDuel");
					}
				}
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x000195BF File Offset: 0x000177BF
		// (set) Token: 0x060008DA RID: 2266 RVA: 0x000195C7 File Offset: 0x000177C7
		[Editor(false)]
		public BrushWidget Background
		{
			get
			{
				return this._background;
			}
			set
			{
				if (value != this._background)
				{
					this._background = value;
					base.OnPropertyChanged<BrushWidget>(value, "Background");
				}
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x000195E5 File Offset: 0x000177E5
		// (set) Token: 0x060008DC RID: 2268 RVA: 0x000195ED File Offset: 0x000177ED
		[Editor(false)]
		public BrushWidget VictimCompassBackground
		{
			get
			{
				return this._victimCompassBackground;
			}
			set
			{
				if (value != this._victimCompassBackground)
				{
					this._victimCompassBackground = value;
					base.OnPropertyChanged<BrushWidget>(value, "VictimCompassBackground");
				}
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x0001960B File Offset: 0x0001780B
		// (set) Token: 0x060008DE RID: 2270 RVA: 0x00019613 File Offset: 0x00017813
		[Editor(false)]
		public BrushWidget MurdererCompassBackground
		{
			get
			{
				return this._murdererCompassBackground;
			}
			set
			{
				if (value != this._murdererCompassBackground)
				{
					this._murdererCompassBackground = value;
					base.OnPropertyChanged<BrushWidget>(value, "MurdererCompassBackground");
				}
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x00019631 File Offset: 0x00017831
		// (set) Token: 0x060008E0 RID: 2272 RVA: 0x00019639 File Offset: 0x00017839
		[Editor(false)]
		public ScrollingRichTextWidget VictimNameText
		{
			get
			{
				return this._victimNameText;
			}
			set
			{
				if (value != this._victimNameText)
				{
					this._victimNameText = value;
					base.OnPropertyChanged<ScrollingRichTextWidget>(value, "VictimNameText");
				}
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060008E1 RID: 2273 RVA: 0x00019657 File Offset: 0x00017857
		// (set) Token: 0x060008E2 RID: 2274 RVA: 0x0001965F File Offset: 0x0001785F
		[Editor(false)]
		public ScrollingRichTextWidget MurdererNameText
		{
			get
			{
				return this._murdererNameText;
			}
			set
			{
				if (value != this._murdererNameText)
				{
					this._murdererNameText = value;
					base.OnPropertyChanged<ScrollingRichTextWidget>(value, "MurdererNameText");
				}
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060008E3 RID: 2275 RVA: 0x0001967D File Offset: 0x0001787D
		// (set) Token: 0x060008E4 RID: 2276 RVA: 0x00019685 File Offset: 0x00017885
		[Editor(false)]
		public TextWidget VictimScoreText
		{
			get
			{
				return this._victimScoreText;
			}
			set
			{
				if (value != this._victimScoreText)
				{
					this._victimScoreText = value;
					base.OnPropertyChanged<TextWidget>(value, "VictimScoreText");
				}
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060008E5 RID: 2277 RVA: 0x000196A3 File Offset: 0x000178A3
		// (set) Token: 0x060008E6 RID: 2278 RVA: 0x000196AB File Offset: 0x000178AB
		[Editor(false)]
		public TextWidget MurdererScoreText
		{
			get
			{
				return this._murdererScoreText;
			}
			set
			{
				if (value != this._murdererScoreText)
				{
					this._murdererScoreText = value;
					base.OnPropertyChanged<TextWidget>(value, "MurdererScoreText");
				}
			}
		}

		// Token: 0x04000404 RID: 1028
		private const string EndOfDuelState = "EndOfDuel";

		// Token: 0x04000405 RID: 1029
		private bool _isEndOfDuel;

		// Token: 0x04000406 RID: 1030
		private BrushWidget _background;

		// Token: 0x04000407 RID: 1031
		private BrushWidget _victimCompassBackground;

		// Token: 0x04000408 RID: 1032
		private BrushWidget _murdererCompassBackground;

		// Token: 0x04000409 RID: 1033
		private ScrollingRichTextWidget _victimNameText;

		// Token: 0x0400040A RID: 1034
		private ScrollingRichTextWidget _murdererNameText;

		// Token: 0x0400040B RID: 1035
		private TextWidget _victimScoreText;

		// Token: 0x0400040C RID: 1036
		private TextWidget _murdererScoreText;
	}
}
