using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Popup
{
	public class TextQueryParentWidget : Widget
	{
		public TextQueryParentWidget(UIContext context)
			: base(context)
		{
		}

		private void FocusOnTextQuery()
		{
			base.EventManager.SetWidgetFocused(this.TextInputWidget, true);
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			if (this.TextInputWidget != null)
			{
				this.FocusOnTextQuery();
			}
		}

		[Editor(false)]
		public EditableTextWidget TextInputWidget
		{
			get
			{
				return this._editableTextWidget;
			}
			set
			{
				if (value != this._editableTextWidget)
				{
					this._editableTextWidget = value;
					this.FocusOnTextQuery();
				}
			}
		}

		private EditableTextWidget _editableTextWidget;
	}
}
