using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	public class ClanPartyRoleSelectionToggleWidget : ButtonWidget
	{
		public ClanPartyRoleSelectionToggleWidget(UIContext context)
			: base(context)
		{
			this.ClickEventHandlers.Add(new Action<Widget>(this.OnClick));
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			ClanPartyRoleSelectionPopupWidget popup = this.Popup;
			if (((popup != null) ? popup.ActiveToggleWidget : null) == this && MathF.Abs(this.Popup.Size.Y - this._lastPopupSizeY) > 1E-05f)
			{
				this.UpdatePopupPosition();
				this._lastPopupSizeY = this.Popup.Size.Y;
			}
		}

		protected virtual void OnClick(Widget widget)
		{
			if (this.Popup != null)
			{
				if (this.Popup.ActiveToggleWidget == this)
				{
					this.ClosePopup();
					return;
				}
				this.OpenPopup();
			}
		}

		private void OpenPopup()
		{
			this.Popup.ActiveToggleWidget = this;
			this.Popup.IsVisible = true;
			this.UpdatePopupPosition();
			this._lastPopupSizeY = this.Popup.Size.Y;
		}

		private void ClosePopup()
		{
			this.Popup.ActiveToggleWidget = null;
			this.Popup.IsVisible = false;
		}

		private void UpdatePopupPosition()
		{
			this.Popup.ScaledPositionYOffset += base.GlobalPosition.Y - this.Popup.GlobalPosition.Y - this.Popup.Size.Y + 47f * base._scaleToUse;
			this.Popup.ScaledPositionXOffset += base.GlobalPosition.X - this.Popup.GlobalPosition.X + 80f * base._scaleToUse;
		}

		[Editor(false)]
		public ClanPartyRoleSelectionPopupWidget Popup
		{
			get
			{
				return this._popup;
			}
			set
			{
				if (this._popup != value)
				{
					this._popup = value;
					base.OnPropertyChanged<ClanPartyRoleSelectionPopupWidget>(value, "Popup");
					this._popup.AddToggleWidget(this);
				}
			}
		}

		private float _lastPopupSizeY;

		private ClanPartyRoleSelectionPopupWidget _popup;
	}
}
