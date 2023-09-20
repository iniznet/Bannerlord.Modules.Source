using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	public class OverlayPopupWidget : Widget
	{
		public OverlayPopupWidget(UIContext context)
			: base(context)
		{
		}

		public void SetCurrentCharacter(GameMenuPartyItemButtonWidget item)
		{
			this.NameTextWidget.Text = item.Name;
			this.DescriptionTextWidget.Text = item.Description;
			this.LocationTextWidget.Text = item.Location;
			this.PowerTextWidget.Text = item.Power;
			if (item.CurrentCharacterImageWidget != null)
			{
				this.CurrentCharacterImageWidget.ImageId = item.CurrentCharacterImageWidget.ImageId;
				this.CurrentCharacterImageWidget.ImageTypeCode = item.CurrentCharacterImageWidget.ImageTypeCode;
				this.CurrentCharacterImageWidget.AdditionalArgs = item.CurrentCharacterImageWidget.AdditionalArgs;
			}
			if (!base.ParentWidget.IsVisible)
			{
				this.OpenPopup();
			}
		}

		private void OpenPopup()
		{
			base.ParentWidget.IsVisible = true;
			base.EventFired("OnOpen", Array.Empty<object>());
			this._isOpen = true;
		}

		private void ClosePopup()
		{
			base.ParentWidget.IsVisible = false;
			base.EventFired("OnClose", Array.Empty<object>());
			this._isOpen = false;
		}

		public void OnCloseButtonClick(Widget widget)
		{
			this.ClosePopup();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isOpen && !base.IsRecursivelyVisible())
			{
				this.ClosePopup();
			}
			else if (!this._isOpen && base.IsRecursivelyVisible())
			{
				this.OpenPopup();
			}
			if (!(base.EventManager.LatestMouseDownWidget is GameMenuPartyItemButtonWidget) && base.EventManager.LatestMouseDownWidget != this && base.EventManager.LatestMouseDownWidget != this._closeButton && base.ParentWidget.IsVisible && (!base.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget) || this.ActionButtonsList.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget)))
			{
				this.ClosePopup();
			}
		}

		[Editor(false)]
		public ImageIdentifierWidget CurrentCharacterImageWidget
		{
			get
			{
				return this._currentCharacterImageWidget;
			}
			set
			{
				if (this._currentCharacterImageWidget != value)
				{
					this._currentCharacterImageWidget = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "CurrentCharacterImageWidget");
				}
			}
		}

		[Editor(false)]
		public TextWidget LocationTextWidget
		{
			get
			{
				return this._locationTextWidget;
			}
			set
			{
				if (this._locationTextWidget != value)
				{
					this._locationTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "LocationTextWidget");
				}
			}
		}

		[Editor(false)]
		public TextWidget NameTextWidget
		{
			get
			{
				return this._nameTextWidget;
			}
			set
			{
				if (this._nameTextWidget != value)
				{
					this._nameTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameTextWidget");
				}
			}
		}

		[Editor(false)]
		public TextWidget PowerTextWidget
		{
			get
			{
				return this._powerTextWidget;
			}
			set
			{
				if (this._powerTextWidget != value)
				{
					this._powerTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "PowerTextWidget");
				}
			}
		}

		[Editor(false)]
		public TextWidget DescriptionTextWidget
		{
			get
			{
				return this._descriptionTextWidget;
			}
			set
			{
				if (this._descriptionTextWidget != value)
				{
					this._descriptionTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "DescriptionTextWidget");
				}
			}
		}

		[Editor(false)]
		public Widget RelationBackgroundWidget
		{
			get
			{
				return this._relationBackgroundWidget;
			}
			set
			{
				if (this._relationBackgroundWidget != value)
				{
					this._relationBackgroundWidget = value;
					base.OnPropertyChanged<Widget>(value, "RelationBackgroundWidget");
				}
			}
		}

		[Editor(false)]
		public ListPanel ActionButtonsList
		{
			get
			{
				return this._actionButtonsList;
			}
			set
			{
				if (this._actionButtonsList != value)
				{
					this._actionButtonsList = value;
					base.OnPropertyChanged<ListPanel>(value, "ActionButtonsList");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget CloseButton
		{
			get
			{
				return this._closeButton;
			}
			set
			{
				if (this._closeButton != value)
				{
					ButtonWidget closeButton = this._closeButton;
					if (closeButton != null)
					{
						closeButton.ClickEventHandlers.Remove(new Action<Widget>(this.OnCloseButtonClick));
					}
					this._closeButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "CloseButton");
					ButtonWidget closeButton2 = this._closeButton;
					if (closeButton2 == null)
					{
						return;
					}
					closeButton2.ClickEventHandlers.Add(new Action<Widget>(this.OnCloseButtonClick));
				}
			}
		}

		private bool _isOpen;

		private ImageIdentifierWidget _currentCharacterImageWidget;

		private TextWidget _locationTextWidget;

		private TextWidget _descriptionTextWidget;

		private TextWidget _powerTextWidget;

		private TextWidget _nameTextWidget;

		private Widget _relationBackgroundWidget;

		private ButtonWidget _closeButton;

		private ListPanel _actionButtonsList;
	}
}
