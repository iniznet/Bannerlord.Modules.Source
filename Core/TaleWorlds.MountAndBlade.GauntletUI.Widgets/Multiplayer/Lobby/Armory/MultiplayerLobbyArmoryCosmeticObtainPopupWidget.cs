using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A8 RID: 168
	public class MultiplayerLobbyArmoryCosmeticObtainPopupWidget : Widget
	{
		// Token: 0x060008AF RID: 2223 RVA: 0x00018FB5 File Offset: 0x000171B5
		public MultiplayerLobbyArmoryCosmeticObtainPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x00018FC8 File Offset: 0x000171C8
		private void OnObtainStateChanged(int newState)
		{
			if (newState == 0)
			{
				this.ItemPreviewListPanel.IsVisible = true;
				this.ActionButtonWidget.IsEnabled = true;
				this.CancelButtonWidget.IsEnabled = true;
				this.ResultSuccessfulIconWidget.IsVisible = false;
				this.ResultFailedIconWidget.IsVisible = false;
				this.ResultTextWidget.IsVisible = false;
				this.LoadingAnimationWidget.IsVisible = false;
				return;
			}
			if (newState == 1)
			{
				this.LoadingAnimationWidget.IsVisible = true;
				this.CancelButtonWidget.IsEnabled = false;
				this.ActionButtonWidget.IsEnabled = false;
				this.ItemPreviewListPanel.IsVisible = false;
				this.ResultSuccessfulIconWidget.IsVisible = false;
				this.ResultFailedIconWidget.IsVisible = false;
				this.ResultTextWidget.IsVisible = false;
				return;
			}
			if (newState == 2 || newState == 3)
			{
				this.CancelButtonWidget.IsEnabled = true;
				this.ActionButtonWidget.IsEnabled = true;
				this.ResultTextWidget.IsVisible = true;
				if (newState == 2)
				{
					this.ResultSuccessfulIconWidget.IsVisible = true;
				}
				else
				{
					this.ResultFailedIconWidget.IsVisible = true;
				}
				this.ItemPreviewListPanel.IsVisible = false;
				this.LoadingAnimationWidget.IsVisible = false;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x000190E8 File Offset: 0x000172E8
		// (set) Token: 0x060008B2 RID: 2226 RVA: 0x000190F0 File Offset: 0x000172F0
		[Editor(false)]
		public int ObtainState
		{
			get
			{
				return this._obtainState;
			}
			set
			{
				if (value != this._obtainState)
				{
					this._obtainState = value;
					base.OnPropertyChanged(value, "ObtainState");
					this.OnObtainStateChanged(value);
				}
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x00019115 File Offset: 0x00017315
		// (set) Token: 0x060008B4 RID: 2228 RVA: 0x0001911D File Offset: 0x0001731D
		[Editor(false)]
		public ButtonWidget CancelButtonWidget
		{
			get
			{
				return this._cancelButtonWidget;
			}
			set
			{
				if (value != this._cancelButtonWidget)
				{
					this._cancelButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "CancelButtonWidget");
				}
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x0001913B File Offset: 0x0001733B
		// (set) Token: 0x060008B6 RID: 2230 RVA: 0x00019143 File Offset: 0x00017343
		[Editor(false)]
		public ListPanel ItemPreviewListPanel
		{
			get
			{
				return this._itemPreviewListPanel;
			}
			set
			{
				if (value != this._itemPreviewListPanel)
				{
					this._itemPreviewListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "ItemPreviewListPanel");
				}
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x00019161 File Offset: 0x00017361
		// (set) Token: 0x060008B8 RID: 2232 RVA: 0x00019169 File Offset: 0x00017369
		[Editor(false)]
		public ButtonWidget ActionButtonWidget
		{
			get
			{
				return this._actionButtonWidget;
			}
			set
			{
				if (value != this._actionButtonWidget)
				{
					this._actionButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ActionButtonWidget");
				}
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x00019187 File Offset: 0x00017387
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x0001918F File Offset: 0x0001738F
		[Editor(false)]
		public Widget ResultSuccessfulIconWidget
		{
			get
			{
				return this._resultSuccessfulIconWidget;
			}
			set
			{
				if (value != this._resultSuccessfulIconWidget)
				{
					this._resultSuccessfulIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "ResultSuccessfulIconWidget");
				}
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x000191AD File Offset: 0x000173AD
		// (set) Token: 0x060008BC RID: 2236 RVA: 0x000191B5 File Offset: 0x000173B5
		[Editor(false)]
		public Widget ResultFailedIconWidget
		{
			get
			{
				return this._resultFailedIconWidget;
			}
			set
			{
				if (value != this._resultFailedIconWidget)
				{
					this._resultFailedIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "ResultFailedIconWidget");
				}
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x000191D3 File Offset: 0x000173D3
		// (set) Token: 0x060008BE RID: 2238 RVA: 0x000191DB File Offset: 0x000173DB
		[Editor(false)]
		public TextWidget ResultTextWidget
		{
			get
			{
				return this._resultTextWidget;
			}
			set
			{
				if (value != this._resultTextWidget)
				{
					this._resultTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "ResultTextWidget");
				}
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x000191F9 File Offset: 0x000173F9
		// (set) Token: 0x060008C0 RID: 2240 RVA: 0x00019201 File Offset: 0x00017401
		[Editor(false)]
		public Widget LoadingAnimationWidget
		{
			get
			{
				return this._loadingAnimationWidget;
			}
			set
			{
				if (value != this._loadingAnimationWidget)
				{
					this._loadingAnimationWidget = value;
					base.OnPropertyChanged<Widget>(value, "LoadingAnimationWidget");
				}
			}
		}

		// Token: 0x040003F5 RID: 1013
		private int _obtainState = -1;

		// Token: 0x040003F6 RID: 1014
		private ButtonWidget _cancelButtonWidget;

		// Token: 0x040003F7 RID: 1015
		private ListPanel _itemPreviewListPanel;

		// Token: 0x040003F8 RID: 1016
		private ButtonWidget _actionButtonWidget;

		// Token: 0x040003F9 RID: 1017
		private Widget _resultSuccessfulIconWidget;

		// Token: 0x040003FA RID: 1018
		private Widget _resultFailedIconWidget;

		// Token: 0x040003FB RID: 1019
		private TextWidget _resultTextWidget;

		// Token: 0x040003FC RID: 1020
		private Widget _loadingAnimationWidget;
	}
}
