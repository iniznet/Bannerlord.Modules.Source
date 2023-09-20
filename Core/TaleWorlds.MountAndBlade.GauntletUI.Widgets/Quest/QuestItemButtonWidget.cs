using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	// Token: 0x02000052 RID: 82
	public class QuestItemButtonWidget : ButtonWidget
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x0000D8F1 File Offset: 0x0000BAF1
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x0000D8F9 File Offset: 0x0000BAF9
		public Brush MainStoryLineItemBrush { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x0000D902 File Offset: 0x0000BB02
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x0000D90A File Offset: 0x0000BB0A
		public Brush NormalItemBrush { get; set; }

		// Token: 0x0600044D RID: 1101 RVA: 0x0000D913 File Offset: 0x0000BB13
		public QuestItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0000D91C File Offset: 0x0000BB1C
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

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x0000DA3B File Offset: 0x0000BC3B
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x0000DA43 File Offset: 0x0000BC43
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

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x0000DA61 File Offset: 0x0000BC61
		// (set) Token: 0x06000452 RID: 1106 RVA: 0x0000DA69 File Offset: 0x0000BC69
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

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000453 RID: 1107 RVA: 0x0000DA87 File Offset: 0x0000BC87
		// (set) Token: 0x06000454 RID: 1108 RVA: 0x0000DA8F File Offset: 0x0000BC8F
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

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000455 RID: 1109 RVA: 0x0000DAAD File Offset: 0x0000BCAD
		// (set) Token: 0x06000456 RID: 1110 RVA: 0x0000DAB5 File Offset: 0x0000BCB5
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

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000457 RID: 1111 RVA: 0x0000DAD3 File Offset: 0x0000BCD3
		// (set) Token: 0x06000458 RID: 1112 RVA: 0x0000DADB File Offset: 0x0000BCDB
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

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000459 RID: 1113 RVA: 0x0000DAF9 File Offset: 0x0000BCF9
		// (set) Token: 0x0600045A RID: 1114 RVA: 0x0000DB01 File Offset: 0x0000BD01
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

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600045B RID: 1115 RVA: 0x0000DB1F File Offset: 0x0000BD1F
		// (set) Token: 0x0600045C RID: 1116 RVA: 0x0000DB27 File Offset: 0x0000BD27
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

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x0600045D RID: 1117 RVA: 0x0000DB45 File Offset: 0x0000BD45
		// (set) Token: 0x0600045E RID: 1118 RVA: 0x0000DB4D File Offset: 0x0000BD4D
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

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x0000DB6B File Offset: 0x0000BD6B
		// (set) Token: 0x06000460 RID: 1120 RVA: 0x0000DB73 File Offset: 0x0000BD73
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

		// Token: 0x040001DC RID: 476
		private bool _initialized;

		// Token: 0x040001DF RID: 479
		private TextWidget _questNameText;

		// Token: 0x040001E0 RID: 480
		private TextWidget _questDateText;

		// Token: 0x040001E1 RID: 481
		private int _questNameYOffset;

		// Token: 0x040001E2 RID: 482
		private int _questNameXOffset;

		// Token: 0x040001E3 RID: 483
		private int _questDateYOffset;

		// Token: 0x040001E4 RID: 484
		private int _questDateXOffset;

		// Token: 0x040001E5 RID: 485
		private bool _isCompleted;

		// Token: 0x040001E6 RID: 486
		private bool _isRemainingDaysHidden;

		// Token: 0x040001E7 RID: 487
		private bool _isMainStoryLineQuest;
	}
}
