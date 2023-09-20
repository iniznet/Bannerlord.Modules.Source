using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyArmoryCosmeticObtainPopupWidget : Widget
	{
		public MultiplayerLobbyArmoryCosmeticObtainPopupWidget(UIContext context)
			: base(context)
		{
		}

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

		private int _obtainState = -1;

		private ButtonWidget _cancelButtonWidget;

		private ListPanel _itemPreviewListPanel;

		private ButtonWidget _actionButtonWidget;

		private Widget _resultSuccessfulIconWidget;

		private Widget _resultFailedIconWidget;

		private TextWidget _resultTextWidget;

		private Widget _loadingAnimationWidget;
	}
}
