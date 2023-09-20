using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class TutorialScreenWidget : Widget
	{
		public TutorialPanelImageWidget LeftItem { get; set; }

		public TutorialPanelImageWidget RightItem { get; set; }

		public TutorialPanelImageWidget BottomItem { get; set; }

		public TutorialPanelImageWidget TopItem { get; set; }

		public TutorialPanelImageWidget LeftTopItem { get; set; }

		public TutorialPanelImageWidget RightTopItem { get; set; }

		public TutorialPanelImageWidget LeftBottomItem { get; set; }

		public TutorialPanelImageWidget RightBottomItem { get; set; }

		public TutorialPanelImageWidget CenterItem { get; set; }

		public TutorialArrowWidget ArrowWidget { get; set; }

		public TutorialScreenWidget(UIContext context)
			: base(context)
		{
			EventManager.UIEventManager.RegisterEvent<TutorialHighlightItemBrushWidget.HighlightElementToggledEvent>(new Action<TutorialHighlightItemBrushWidget.HighlightElementToggledEvent>(this.OnHighlightElementToggleEvent));
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initalized)
			{
				this.LeftItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.RightItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.BottomItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.TopItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.LeftTopItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.RightTopItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.LeftBottomItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.RightBottomItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this.CenterItem.boolPropertyChanged += this.OnTutorialItemPropertyChanged;
				this._initalized = true;
			}
			if (this._currentActiveHighligtFrame != null && this._currentActivePanelItem != null)
			{
				Tuple<Widget, Widget> leftAndRightElements = this.GetLeftAndRightElements();
				Tuple<Widget, Widget> topAndBottomElements = this.GetTopAndBottomElements();
				float num = leftAndRightElements.Item1.GlobalPosition.X + leftAndRightElements.Item1.Size.X;
				float x = leftAndRightElements.Item2.GlobalPosition.X;
				float y = topAndBottomElements.Item1.GlobalPosition.Y;
				float y2 = topAndBottomElements.Item2.GlobalPosition.Y;
				float num2 = MathF.Abs(num - x);
				float num3 = MathF.Abs(y - y2);
				this.ArrowWidget.ScaledPositionXOffset = num;
				this.ArrowWidget.ScaledPositionYOffset = y;
				this.ArrowWidget.SetArrowProperties(num2, num3, this.GetIsArrowDirectionIsDownwards(), this.GetIsArrowDirectionIsTowardsRight());
				this.ArrowWidget.IsVisible = true;
				return;
			}
			this.ArrowWidget.IsVisible = false;
		}

		private bool GetIsArrowDirectionIsDownwards()
		{
			if (this._currentActiveHighligtFrame.GlobalPosition.Y < this._currentActivePanelItem.GlobalPosition.Y)
			{
				return this._currentActiveHighligtFrame.GlobalPosition.X < this._currentActivePanelItem.GlobalPosition.X;
			}
			return this._currentActivePanelItem.GlobalPosition.X < this._currentActiveHighligtFrame.GlobalPosition.X;
		}

		private bool GetIsArrowDirectionIsTowardsRight()
		{
			return this._currentActiveHighligtFrame.GlobalPosition.X > this._currentActivePanelItem.GlobalPosition.X;
		}

		private Tuple<Widget, Widget> GetLeftAndRightElements()
		{
			if (this._currentActiveHighligtFrame.GlobalPosition.X < this._currentActivePanelItem.GlobalPosition.X)
			{
				return new Tuple<Widget, Widget>(this._currentActiveHighligtFrame, this._currentActivePanelItem);
			}
			return new Tuple<Widget, Widget>(this._currentActivePanelItem, this._currentActiveHighligtFrame);
		}

		private Tuple<Widget, Widget> GetTopAndBottomElements()
		{
			if (this._currentActiveHighligtFrame.GlobalPosition.Y < this._currentActivePanelItem.GlobalPosition.Y)
			{
				return new Tuple<Widget, Widget>(this._currentActiveHighligtFrame, this._currentActivePanelItem);
			}
			return new Tuple<Widget, Widget>(this._currentActivePanelItem, this._currentActiveHighligtFrame);
		}

		private void OnTutorialItemPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsDisabled")
			{
				if (propertyValue)
				{
					this._currentActivePanelItem = null;
					this.ArrowWidget.ResetFade();
					return;
				}
				this._currentActivePanelItem = widget as TutorialPanelImageWidget;
				this.ArrowWidget.DisableFade();
			}
		}

		private void OnHighlightElementToggleEvent(TutorialHighlightItemBrushWidget.HighlightElementToggledEvent obj)
		{
			if (obj.IsEnabled)
			{
				this._currentActiveHighligtFrame = obj.HighlightFrameWidget;
				this.ArrowWidget.ResetFade();
				return;
			}
			this.ArrowWidget.DisableFade();
			this._currentActiveHighligtFrame = null;
		}

		private bool _initalized;

		private TutorialHighlightItemBrushWidget _currentActiveHighligtFrame;

		private TutorialPanelImageWidget _currentActivePanelItem;
	}
}
