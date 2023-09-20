using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	public class MultiSelectionElementsWidget : Widget
	{
		public MultiSelectionElementsWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._updateRequired)
			{
				this.UpdateElementsList();
				this._updateRequired = false;
			}
			if (base.IsRecursivelyVisible())
			{
				if (this.MaxSelectableOptionCount <= 0)
				{
					this.DoneButtonWidget.IsEnabled = true;
				}
				else
				{
					int numOfChildrenSelected = this.GetNumOfChildrenSelected();
					this.DoneButtonWidget.IsEnabled = (numOfChildrenSelected <= this.MaxSelectableOptionCount && numOfChildrenSelected > 0) || this._elementsList.Count == 0;
				}
				if (this._latestClickedWidget != null && this.MaxSelectableOptionCount == 1)
				{
					this._elementsList.ForEach(delegate(ButtonWidget e)
					{
						if (e != this._latestClickedWidget)
						{
							e.IsSelected = false;
							return;
						}
						e.IsSelected = true;
					});
				}
			}
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			if (child is ListPanel)
			{
				this._elementContainer = child as ListPanel;
				this._elementContainer.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnElementAdded));
			}
		}

		private void OnElementAdded(Widget parentWidget, Widget addedWidget)
		{
			this._updateRequired = true;
		}

		private int GetNumOfChildrenSelected()
		{
			int num = 0;
			for (int i = 0; i < this._elementsList.Count; i++)
			{
				if (this._elementsList[i].IsSelected)
				{
					num++;
				}
			}
			return num;
		}

		private void UpdateElementsList()
		{
			this._elementsList.Clear();
			this._latestClickedWidget = null;
			for (int i = 0; i < this._elementContainer.ChildCount; i++)
			{
				ButtonWidget buttonWidget = this._elementContainer.GetChild(i).GetChild(0) as ButtonWidget;
				buttonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnElementClick));
				this._elementsList.Add(buttonWidget);
			}
		}

		private void OnElementClick(Widget widget)
		{
			ButtonWidget buttonWidget;
			if ((buttonWidget = widget as ButtonWidget) != null)
			{
				this._latestClickedWidget = buttonWidget;
			}
		}

		[Editor(false)]
		public int MaxSelectableOptionCount
		{
			get
			{
				return this._maxSelectableOptionCount;
			}
			set
			{
				if (this._maxSelectableOptionCount != value)
				{
					this._maxSelectableOptionCount = value;
					base.OnPropertyChanged(value, "MaxSelectableOptionCount");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget DoneButtonWidget
		{
			get
			{
				return this._doneButtonWidget;
			}
			set
			{
				if (this._doneButtonWidget != value)
				{
					this._doneButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "DoneButtonWidget");
				}
			}
		}

		private bool _updateRequired;

		private List<ButtonWidget> _elementsList = new List<ButtonWidget>();

		private ButtonWidget _latestClickedWidget;

		private ButtonWidget _doneButtonWidget;

		private ListPanel _elementContainer;

		private int _maxSelectableOptionCount = -100;
	}
}
