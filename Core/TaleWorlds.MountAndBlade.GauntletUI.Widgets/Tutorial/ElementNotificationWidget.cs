using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x0200003D RID: 61
	public class ElementNotificationWidget : Widget
	{
		// Token: 0x06000337 RID: 823 RVA: 0x0000A508 File Offset: 0x00008708
		public ElementNotificationWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000A51C File Offset: 0x0000871C
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

		// Token: 0x06000339 RID: 825 RVA: 0x0000A688 File Offset: 0x00008888
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

		// Token: 0x0600033A RID: 826 RVA: 0x0000A6DA File Offset: 0x000088DA
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

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600033B RID: 827 RVA: 0x0000A716 File Offset: 0x00008916
		// (set) Token: 0x0600033C RID: 828 RVA: 0x0000A720 File Offset: 0x00008920
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

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600033D RID: 829 RVA: 0x0000A773 File Offset: 0x00008973
		// (set) Token: 0x0600033E RID: 830 RVA: 0x0000A77B File Offset: 0x0000897B
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

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000A799 File Offset: 0x00008999
		// (set) Token: 0x06000340 RID: 832 RVA: 0x0000A7A1 File Offset: 0x000089A1
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

		// Token: 0x04000151 RID: 337
		private bool _doesNotHaveElement;

		// Token: 0x04000152 RID: 338
		private bool _shouldSyncSize;

		// Token: 0x04000153 RID: 339
		private string _elementID = string.Empty;

		// Token: 0x04000154 RID: 340
		private Widget _elementToHighlight;

		// Token: 0x04000155 RID: 341
		private TutorialHighlightItemBrushWidget _tutorialFrameWidget;
	}
}
