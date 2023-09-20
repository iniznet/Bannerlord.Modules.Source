using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation
{
	public class CharacterCreationStageSelectionBarListPanel : ListPanel
	{
		public CharacterCreationStageSelectionBarListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.RefreshButtonList();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.BarFillWidget != null && this.TotalStagesCount != 0 && this._buttonsInitialized && this.CurrentStageIndex != -1)
			{
				this.BarFillWidget.ScaledSuggestedWidth = this.BarCanvasWidget.Size.X - this._stageButtonsList[this._stageButtonsList.Count - 1 - this.CurrentStageIndex].LocalPosition.X;
			}
		}

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

		private void OnStageSelection(Widget stageButton)
		{
			int num = this._stageButtonsList.IndexOf(stageButton as ButtonWidget);
			base.EventFired("OnStageSelection", new object[] { num });
		}

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

		private List<ButtonWidget> _stageButtonsList = new List<ButtonWidget>();

		private bool _buttonsInitialized;

		private ButtonWidget _stageButtonTemplate;

		private int _currentStageIndex = -1;

		private int _totalStagesCount = -1;

		private int _openedStageIndex = -1;

		private string _fullButtonBrush;

		private string _emptyButtonBrush;

		private string _fullBrightButtonBrush;

		private Widget _barFillWidget;

		private Widget _barCanvasWidget;
	}
}
