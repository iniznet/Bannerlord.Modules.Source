using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	// Token: 0x02000156 RID: 342
	public class ClanPartyRoleSelectionToggleWidget : ButtonWidget
	{
		// Token: 0x060011A7 RID: 4519 RVA: 0x00030B34 File Offset: 0x0002ED34
		public ClanPartyRoleSelectionToggleWidget(UIContext context)
			: base(context)
		{
			this.ClickEventHandlers.Add(new Action<Widget>(this.OnClick));
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00030B58 File Offset: 0x0002ED58
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

		// Token: 0x060011A9 RID: 4521 RVA: 0x00030BC0 File Offset: 0x0002EDC0
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

		// Token: 0x060011AA RID: 4522 RVA: 0x00030BE5 File Offset: 0x0002EDE5
		private void OpenPopup()
		{
			this.Popup.ActiveToggleWidget = this;
			this.Popup.IsVisible = true;
			this.UpdatePopupPosition();
			this._lastPopupSizeY = this.Popup.Size.Y;
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x00030C1B File Offset: 0x0002EE1B
		private void ClosePopup()
		{
			this.Popup.ActiveToggleWidget = null;
			this.Popup.IsVisible = false;
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00030C38 File Offset: 0x0002EE38
		private void UpdatePopupPosition()
		{
			this.Popup.ScaledPositionYOffset += base.GlobalPosition.Y - this.Popup.GlobalPosition.Y - this.Popup.Size.Y + 47f * base._scaleToUse;
			this.Popup.ScaledPositionXOffset += base.GlobalPosition.X - this.Popup.GlobalPosition.X + 80f * base._scaleToUse;
		}

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x060011AD RID: 4525 RVA: 0x00030CCC File Offset: 0x0002EECC
		// (set) Token: 0x060011AE RID: 4526 RVA: 0x00030CD4 File Offset: 0x0002EED4
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

		// Token: 0x04000813 RID: 2067
		private float _lastPopupSizeY;

		// Token: 0x04000814 RID: 2068
		private ClanPartyRoleSelectionPopupWidget _popup;
	}
}
