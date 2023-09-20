using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GatherArmy
{
	public class BoostCohesionPopupWidget : Widget
	{
		public BoostCohesionPopupWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.ClosePopupButton != null && !this.ClosePopupButton.ClickEventHandlers.Contains(new Action<Widget>(this.ClosePopup)))
			{
				this.ClosePopupButton.ClickEventHandlers.Add(new Action<Widget>(this.ClosePopup));
			}
		}

		public void ClosePopup(Widget widget)
		{
			base.ParentWidget.IsVisible = false;
		}

		[Editor(false)]
		public ButtonWidget ClosePopupButton
		{
			get
			{
				return this._closePopupButton;
			}
			set
			{
				if (this._closePopupButton != value)
				{
					this._closePopupButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ClosePopupButton");
				}
			}
		}

		private ButtonWidget _closePopupButton;
	}
}
