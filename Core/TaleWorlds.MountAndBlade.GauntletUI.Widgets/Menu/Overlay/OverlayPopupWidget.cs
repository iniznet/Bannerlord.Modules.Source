using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	// Token: 0x020000F4 RID: 244
	public class OverlayPopupWidget : Widget
	{
		// Token: 0x06000CAD RID: 3245 RVA: 0x0002379F File Offset: 0x0002199F
		public OverlayPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x000237A8 File Offset: 0x000219A8
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

		// Token: 0x06000CAF RID: 3247 RVA: 0x00023856 File Offset: 0x00021A56
		private void OpenPopup()
		{
			base.ParentWidget.IsVisible = true;
			base.EventFired("OnOpen", Array.Empty<object>());
			this._isOpen = true;
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x0002387B File Offset: 0x00021A7B
		private void ClosePopup()
		{
			base.ParentWidget.IsVisible = false;
			base.EventFired("OnClose", Array.Empty<object>());
			this._isOpen = false;
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x000238A0 File Offset: 0x00021AA0
		public void OnCloseButtonClick(Widget widget)
		{
			this.ClosePopup();
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000238A8 File Offset: 0x00021AA8
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

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06000CB3 RID: 3251 RVA: 0x0002395B File Offset: 0x00021B5B
		// (set) Token: 0x06000CB4 RID: 3252 RVA: 0x00023963 File Offset: 0x00021B63
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

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x00023981 File Offset: 0x00021B81
		// (set) Token: 0x06000CB6 RID: 3254 RVA: 0x00023989 File Offset: 0x00021B89
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

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x000239A7 File Offset: 0x00021BA7
		// (set) Token: 0x06000CB8 RID: 3256 RVA: 0x000239AF File Offset: 0x00021BAF
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

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x000239CD File Offset: 0x00021BCD
		// (set) Token: 0x06000CBA RID: 3258 RVA: 0x000239D5 File Offset: 0x00021BD5
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

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06000CBB RID: 3259 RVA: 0x000239F3 File Offset: 0x00021BF3
		// (set) Token: 0x06000CBC RID: 3260 RVA: 0x000239FB File Offset: 0x00021BFB
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

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06000CBD RID: 3261 RVA: 0x00023A19 File Offset: 0x00021C19
		// (set) Token: 0x06000CBE RID: 3262 RVA: 0x00023A21 File Offset: 0x00021C21
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

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06000CBF RID: 3263 RVA: 0x00023A3F File Offset: 0x00021C3F
		// (set) Token: 0x06000CC0 RID: 3264 RVA: 0x00023A47 File Offset: 0x00021C47
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

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06000CC1 RID: 3265 RVA: 0x00023A65 File Offset: 0x00021C65
		// (set) Token: 0x06000CC2 RID: 3266 RVA: 0x00023A70 File Offset: 0x00021C70
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

		// Token: 0x040005DA RID: 1498
		private bool _isOpen;

		// Token: 0x040005DB RID: 1499
		private ImageIdentifierWidget _currentCharacterImageWidget;

		// Token: 0x040005DC RID: 1500
		private TextWidget _locationTextWidget;

		// Token: 0x040005DD RID: 1501
		private TextWidget _descriptionTextWidget;

		// Token: 0x040005DE RID: 1502
		private TextWidget _powerTextWidget;

		// Token: 0x040005DF RID: 1503
		private TextWidget _nameTextWidget;

		// Token: 0x040005E0 RID: 1504
		private Widget _relationBackgroundWidget;

		// Token: 0x040005E1 RID: 1505
		private ButtonWidget _closeButton;

		// Token: 0x040005E2 RID: 1506
		private ListPanel _actionButtonsList;
	}
}
