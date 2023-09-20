using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	public class QuestItemButtonWidget : ButtonWidget
	{
		public Brush MainStoryLineItemBrush { get; set; }

		public Brush NormalItemBrush { get; set; }

		public QuestItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				base.Brush = (this.IsMainStoryLineQuest ? this.MainStoryLineItemBrush : this.NormalItemBrush);
				this._initialized = true;
			}
			if (this.QuestNameText != null && this.QuestDateText != null)
			{
				if (base.CurrentState == "Pressed")
				{
					this.QuestNameText.PositionYOffset = (float)this.QuestNameYOffset;
					this.QuestNameText.PositionXOffset = (float)this.QuestNameXOffset;
					this.QuestDateText.PositionYOffset = (float)this.QuestDateYOffset;
					this.QuestDateText.PositionXOffset = (float)this.QuestDateXOffset;
				}
				else
				{
					this.QuestNameText.PositionYOffset = 0f;
					this.QuestNameText.PositionXOffset = 0f;
					this.QuestDateText.PositionYOffset = 0f;
					this.QuestDateText.PositionXOffset = 0f;
				}
			}
			if (this.QuestDateText != null)
			{
				if (this.IsCompleted)
				{
					this.QuestDateText.IsVisible = false;
					return;
				}
				this.QuestDateText.IsHidden = this.IsRemainingDaysHidden;
			}
		}

		[Editor(false)]
		public bool IsCompleted
		{
			get
			{
				return this._isCompleted;
			}
			set
			{
				if (this._isCompleted != value)
				{
					this._isCompleted = value;
					base.OnPropertyChanged(value, "IsCompleted");
				}
			}
		}

		[Editor(false)]
		public bool IsMainStoryLineQuest
		{
			get
			{
				return this._isMainStoryLineQuest;
			}
			set
			{
				if (this._isMainStoryLineQuest != value)
				{
					this._isMainStoryLineQuest = value;
					base.OnPropertyChanged(value, "IsMainStoryLineQuest");
				}
			}
		}

		[Editor(false)]
		public bool IsRemainingDaysHidden
		{
			get
			{
				return this._isRemainingDaysHidden;
			}
			set
			{
				if (this._isRemainingDaysHidden != value)
				{
					this._isRemainingDaysHidden = value;
					base.OnPropertyChanged(value, "IsRemainingDaysHidden");
				}
			}
		}

		[Editor(false)]
		public TextWidget QuestNameText
		{
			get
			{
				return this._questNameText;
			}
			set
			{
				if (this._questNameText != value)
				{
					this._questNameText = value;
					base.OnPropertyChanged<TextWidget>(value, "QuestNameText");
				}
			}
		}

		[Editor(false)]
		public TextWidget QuestDateText
		{
			get
			{
				return this._questDateText;
			}
			set
			{
				if (this._questDateText != value)
				{
					this._questDateText = value;
					base.OnPropertyChanged<TextWidget>(value, "QuestDateText");
				}
			}
		}

		[Editor(false)]
		public int QuestNameYOffset
		{
			get
			{
				return this._questNameYOffset;
			}
			set
			{
				if (this._questNameYOffset != value)
				{
					this._questNameYOffset = value;
					base.OnPropertyChanged(value, "QuestNameYOffset");
				}
			}
		}

		[Editor(false)]
		public int QuestNameXOffset
		{
			get
			{
				return this._questNameXOffset;
			}
			set
			{
				if (this._questNameXOffset != value)
				{
					this._questNameXOffset = value;
					base.OnPropertyChanged(value, "QuestNameXOffset");
				}
			}
		}

		[Editor(false)]
		public int QuestDateYOffset
		{
			get
			{
				return this._questDateYOffset;
			}
			set
			{
				if (this._questDateYOffset != value)
				{
					this._questDateYOffset = value;
					base.OnPropertyChanged(value, "QuestDateYOffset");
				}
			}
		}

		[Editor(false)]
		public int QuestDateXOffset
		{
			get
			{
				return this._questDateXOffset;
			}
			set
			{
				if (this._questDateXOffset != value)
				{
					this._questDateXOffset = value;
					base.OnPropertyChanged(value, "QuestDateXOffset");
				}
			}
		}

		private bool _initialized;

		private TextWidget _questNameText;

		private TextWidget _questDateText;

		private int _questNameYOffset;

		private int _questNameXOffset;

		private int _questDateYOffset;

		private int _questDateXOffset;

		private bool _isCompleted;

		private bool _isRemainingDaysHidden;

		private bool _isMainStoryLineQuest;
	}
}
