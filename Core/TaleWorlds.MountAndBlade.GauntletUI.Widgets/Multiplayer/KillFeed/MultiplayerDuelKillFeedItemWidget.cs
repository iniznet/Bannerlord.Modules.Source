using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	public class MultiplayerDuelKillFeedItemWidget : MultiplayerGeneralKillFeedItemWidget
	{
		public MultiplayerDuelKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

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

		private const string EndOfDuelState = "EndOfDuel";

		private bool _isEndOfDuel;

		private BrushWidget _background;

		private BrushWidget _victimCompassBackground;

		private BrushWidget _murdererCompassBackground;

		private ScrollingRichTextWidget _victimNameText;

		private ScrollingRichTextWidget _murdererNameText;

		private TextWidget _victimScoreText;

		private TextWidget _murdererScoreText;
	}
}
