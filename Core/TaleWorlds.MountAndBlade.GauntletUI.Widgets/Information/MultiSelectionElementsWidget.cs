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

		private void UpdateElementsList()
		{
			this._elementsList.Clear();
			for (int i = 0; i < this._elementContainer.ChildCount; i++)
			{
				ButtonWidget buttonWidget = this._elementContainer.GetChild(i).GetChild(0) as ButtonWidget;
				this._elementsList.Add(buttonWidget);
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

		private ButtonWidget _doneButtonWidget;

		private ListPanel _elementContainer;
	}
}
