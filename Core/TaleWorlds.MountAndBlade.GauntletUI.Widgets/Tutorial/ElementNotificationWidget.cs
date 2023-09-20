using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class ElementNotificationWidget : Widget
	{
		public ElementNotificationWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			string elementID = this.ElementID;
			if (elementID != null && elementID.Any<char>() && this.ElementToHighlight == null && !this._doesNotHaveElement)
			{
				this.ElementToHighlight = this.FindElementWithID(base.EventManager.Root, this.ElementID);
				this._doesNotHaveElement = true;
				if (this.ElementToHighlight != null)
				{
					this.TutorialFrameWidget.IsVisible = true;
					this.TutorialFrameWidget.IsHighlightEnabled = true;
					this.TutorialFrameWidget.ParentWidget = this.ElementToHighlight;
					if (this.ElementToHighlight.HeightSizePolicy == SizePolicy.CoverChildren || this.ElementToHighlight.WidthSizePolicy == SizePolicy.CoverChildren)
					{
						this.TutorialFrameWidget.WidthSizePolicy = SizePolicy.Fixed;
						this.TutorialFrameWidget.HeightSizePolicy = SizePolicy.Fixed;
						this._shouldSyncSize = true;
					}
					else
					{
						this.TutorialFrameWidget.WidthSizePolicy = SizePolicy.StretchToParent;
						this.TutorialFrameWidget.HeightSizePolicy = SizePolicy.StretchToParent;
						this._shouldSyncSize = false;
					}
				}
			}
			if (this._shouldSyncSize && this.ElementToHighlight != null && this.ElementToHighlight.Size.X > 1f && this.ElementToHighlight.Size.Y > 1f)
			{
				base.ScaledSuggestedWidth = this.ElementToHighlight.Size.X - 1f;
				base.ScaledSuggestedHeight = this.ElementToHighlight.Size.Y - 1f;
			}
		}

		private Widget FindElementWithID(Widget current, string ID)
		{
			if (current != null)
			{
				for (int i = 0; i < current.ChildCount; i++)
				{
					if (current.GetChild(i).Id == ID)
					{
						return current.GetChild(i);
					}
					Widget widget = this.FindElementWithID(current.GetChild(i), ID);
					if (widget != null)
					{
						return widget;
					}
				}
			}
			return null;
		}

		private void ResetHighlight()
		{
			if (this.TutorialFrameWidget != null)
			{
				this.TutorialFrameWidget.ParentWidget = this;
				this._doesNotHaveElement = false;
				this.TutorialFrameWidget.IsVisible = false;
				this.TutorialFrameWidget.IsHighlightEnabled = false;
				this.ElementToHighlight = null;
			}
		}

		[Editor(false)]
		public string ElementID
		{
			get
			{
				return this._elementID;
			}
			set
			{
				if (this._elementID != value)
				{
					if (this._elementID != string.Empty && value == string.Empty)
					{
						this.ResetHighlight();
					}
					this._elementID = value;
					base.OnPropertyChanged<string>(value, "ElementID");
				}
			}
		}

		[Editor(false)]
		public Widget ElementToHighlight
		{
			get
			{
				return this._elementToHighlight;
			}
			set
			{
				if (this._elementToHighlight != value)
				{
					this._elementToHighlight = value;
					base.OnPropertyChanged<Widget>(value, "ElementToHighlight");
				}
			}
		}

		[Editor(false)]
		public TutorialHighlightItemBrushWidget TutorialFrameWidget
		{
			get
			{
				return this._tutorialFrameWidget;
			}
			set
			{
				if (this._tutorialFrameWidget != value)
				{
					this._tutorialFrameWidget = value;
					base.OnPropertyChanged<TutorialHighlightItemBrushWidget>(value, "TutorialFrameWidget");
					if (this._tutorialFrameWidget != null)
					{
						this._tutorialFrameWidget.IsVisible = false;
					}
				}
			}
		}

		private bool _doesNotHaveElement;

		private bool _shouldSyncSize;

		private string _elementID = string.Empty;

		private Widget _elementToHighlight;

		private TutorialHighlightItemBrushWidget _tutorialFrameWidget;
	}
}
