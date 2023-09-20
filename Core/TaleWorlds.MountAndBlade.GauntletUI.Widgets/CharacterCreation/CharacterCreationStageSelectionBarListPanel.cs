using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation
{
	// Token: 0x02000167 RID: 359
	public class CharacterCreationStageSelectionBarListPanel : ListPanel
	{
		// Token: 0x06001265 RID: 4709 RVA: 0x00032C7F File Offset: 0x00030E7F
		public CharacterCreationStageSelectionBarListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x00032CA8 File Offset: 0x00030EA8
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.RefreshButtonList();
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x00032CB8 File Offset: 0x00030EB8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.BarFillWidget != null && this.TotalStagesCount != 0 && this._buttonsInitialized && this.CurrentStageIndex != -1)
			{
				this.BarFillWidget.ScaledSuggestedWidth = this.BarCanvasWidget.Size.X - this._stageButtonsList[this._stageButtonsList.Count - 1 - this.CurrentStageIndex].LocalPosition.X;
			}
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x00032D34 File Offset: 0x00030F34
		private void RefreshButtonList()
		{
			if (!this._buttonsInitialized)
			{
				this._stageButtonsList = new List<ButtonWidget>();
				if (base.HasChild(this.StageButtonTemplate))
				{
					base.RemoveChild(this.StageButtonTemplate);
				}
				base.RemoveAllChildren();
				if (this.StageButtonTemplate != null && this.EmptyButtonBrush != null && this.FullButtonBrush != null && this.FullBrightButtonBrush != null)
				{
					if (this.TotalStagesCount == 0)
					{
						this.BarCanvasWidget.IsVisible = false;
						base.IsVisible = false;
					}
					else
					{
						for (int i = 0; i < this.TotalStagesCount; i++)
						{
							ButtonWidget buttonWidget = new ButtonWidget(base.Context);
							base.AddChild(buttonWidget);
							buttonWidget.Brush = this.StageButtonTemplate.ReadOnlyBrush;
							bool flag = false;
							if (i == this.CurrentStageIndex)
							{
								buttonWidget.Brush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == this.FullBrightButtonBrush);
							}
							else if (i <= this.OpenedStageIndex || (this.OpenedStageIndex == -1 && i < this.CurrentStageIndex))
							{
								buttonWidget.Brush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == this.FullButtonBrush);
								flag = true;
							}
							else
							{
								buttonWidget.Brush = base.EventManager.Context.Brushes.First((Brush b) => b.Name == this.EmptyButtonBrush);
							}
							buttonWidget.DoNotAcceptEvents = !flag;
							buttonWidget.SuggestedHeight = this.StageButtonTemplate.SuggestedHeight;
							buttonWidget.SuggestedWidth = this.StageButtonTemplate.SuggestedWidth;
							buttonWidget.DoNotPassEventsToChildren = this.StageButtonTemplate.DoNotPassEventsToChildren;
							buttonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnStageSelection));
							this._stageButtonsList.Add(buttonWidget);
						}
					}
					this._buttonsInitialized = true;
				}
			}
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x00032F08 File Offset: 0x00031108
		private void OnStageSelection(Widget stageButton)
		{
			int num = this._stageButtonsList.IndexOf(stageButton as ButtonWidget);
			base.EventFired("OnStageSelection", new object[] { num });
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x0600126A RID: 4714 RVA: 0x00032F41 File Offset: 0x00031141
		// (set) Token: 0x0600126B RID: 4715 RVA: 0x00032F49 File Offset: 0x00031149
		[Editor(false)]
		public ButtonWidget StageButtonTemplate
		{
			get
			{
				return this._stageButtonTemplate;
			}
			set
			{
				if (this._stageButtonTemplate != value)
				{
					this._stageButtonTemplate = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StageButtonTemplate");
					if (value != null)
					{
						base.RemoveChild(value);
					}
				}
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x0600126C RID: 4716 RVA: 0x00032F71 File Offset: 0x00031171
		// (set) Token: 0x0600126D RID: 4717 RVA: 0x00032F79 File Offset: 0x00031179
		[Editor(false)]
		public Widget BarFillWidget
		{
			get
			{
				return this._barFillWidget;
			}
			set
			{
				if (this._barFillWidget != value)
				{
					this._barFillWidget = value;
					base.OnPropertyChanged<Widget>(value, "BarFillWidget");
				}
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x0600126E RID: 4718 RVA: 0x00032F97 File Offset: 0x00031197
		// (set) Token: 0x0600126F RID: 4719 RVA: 0x00032F9F File Offset: 0x0003119F
		[Editor(false)]
		public Widget BarCanvasWidget
		{
			get
			{
				return this._barCanvasWidget;
			}
			set
			{
				if (this._barCanvasWidget != value)
				{
					this._barCanvasWidget = value;
					base.OnPropertyChanged<Widget>(value, "BarCanvasWidget");
				}
			}
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06001270 RID: 4720 RVA: 0x00032FBD File Offset: 0x000311BD
		// (set) Token: 0x06001271 RID: 4721 RVA: 0x00032FC5 File Offset: 0x000311C5
		[Editor(false)]
		public int CurrentStageIndex
		{
			get
			{
				return this._currentStageIndex;
			}
			set
			{
				if (this._currentStageIndex != value)
				{
					this._currentStageIndex = value;
					base.OnPropertyChanged(value, "CurrentStageIndex");
					this._buttonsInitialized = false;
				}
			}
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06001272 RID: 4722 RVA: 0x00032FEA File Offset: 0x000311EA
		// (set) Token: 0x06001273 RID: 4723 RVA: 0x00032FF2 File Offset: 0x000311F2
		[Editor(false)]
		public int TotalStagesCount
		{
			get
			{
				return this._totalStagesCount;
			}
			set
			{
				if (this._totalStagesCount != value)
				{
					this._totalStagesCount = value;
					base.OnPropertyChanged(value, "TotalStagesCount");
				}
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06001274 RID: 4724 RVA: 0x00033010 File Offset: 0x00031210
		// (set) Token: 0x06001275 RID: 4725 RVA: 0x00033018 File Offset: 0x00031218
		[Editor(false)]
		public int OpenedStageIndex
		{
			get
			{
				return this._openedStageIndex;
			}
			set
			{
				if (this._openedStageIndex != value)
				{
					this._openedStageIndex = value;
					base.OnPropertyChanged(value, "OpenedStageIndex");
				}
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06001276 RID: 4726 RVA: 0x00033036 File Offset: 0x00031236
		// (set) Token: 0x06001277 RID: 4727 RVA: 0x0003303E File Offset: 0x0003123E
		[Editor(false)]
		public string FullButtonBrush
		{
			get
			{
				return this._fullButtonBrush;
			}
			set
			{
				if (this._fullButtonBrush != value)
				{
					this._fullButtonBrush = value;
					base.OnPropertyChanged<string>(value, "FullButtonBrush");
				}
			}
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06001278 RID: 4728 RVA: 0x00033061 File Offset: 0x00031261
		// (set) Token: 0x06001279 RID: 4729 RVA: 0x00033069 File Offset: 0x00031269
		[Editor(false)]
		public string EmptyButtonBrush
		{
			get
			{
				return this._emptyButtonBrush;
			}
			set
			{
				if (this._emptyButtonBrush != value)
				{
					this._emptyButtonBrush = value;
					base.OnPropertyChanged<string>(value, "EmptyButtonBrush");
				}
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x0600127A RID: 4730 RVA: 0x0003308C File Offset: 0x0003128C
		// (set) Token: 0x0600127B RID: 4731 RVA: 0x00033094 File Offset: 0x00031294
		[Editor(false)]
		public string FullBrightButtonBrush
		{
			get
			{
				return this._fullBrightButtonBrush;
			}
			set
			{
				if (this._fullBrightButtonBrush != value)
				{
					this._fullBrightButtonBrush = value;
					base.OnPropertyChanged<string>(value, "FullBrightButtonBrush");
				}
			}
		}

		// Token: 0x0400086D RID: 2157
		private List<ButtonWidget> _stageButtonsList = new List<ButtonWidget>();

		// Token: 0x0400086E RID: 2158
		private bool _buttonsInitialized;

		// Token: 0x0400086F RID: 2159
		private ButtonWidget _stageButtonTemplate;

		// Token: 0x04000870 RID: 2160
		private int _currentStageIndex = -1;

		// Token: 0x04000871 RID: 2161
		private int _totalStagesCount = -1;

		// Token: 0x04000872 RID: 2162
		private int _openedStageIndex = -1;

		// Token: 0x04000873 RID: 2163
		private string _fullButtonBrush;

		// Token: 0x04000874 RID: 2164
		private string _emptyButtonBrush;

		// Token: 0x04000875 RID: 2165
		private string _fullBrightButtonBrush;

		// Token: 0x04000876 RID: 2166
		private Widget _barFillWidget;

		// Token: 0x04000877 RID: 2167
		private Widget _barCanvasWidget;
	}
}
