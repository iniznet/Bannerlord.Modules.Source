using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	// Token: 0x020000F2 RID: 242
	public class GameMenuPartyItemButtonWidget : ButtonWidget
	{
		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x0002345B File Offset: 0x0002165B
		// (set) Token: 0x06000C8D RID: 3213 RVA: 0x00023463 File Offset: 0x00021663
		public Brush PartyBackgroundBrush { get; set; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0002346C File Offset: 0x0002166C
		// (set) Token: 0x06000C8F RID: 3215 RVA: 0x00023474 File Offset: 0x00021674
		public Brush CharacterBackgroundBrush { get; set; }

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0002347D File Offset: 0x0002167D
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x00023485 File Offset: 0x00021685
		public ImageWidget BackgroundImageWidget { get; set; }

		// Token: 0x06000C92 RID: 3218 RVA: 0x00023490 File Offset: 0x00021690
		public GameMenuPartyItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x000234E9 File Offset: 0x000216E9
		private string GetRelationBackgroundName(int relation)
		{
			return "";
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x000234F0 File Offset: 0x000216F0
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._popupWidget == null)
			{
				Widget widget = this;
				while (widget != base.EventManager.Root && this._popupWidget == null && this._parentKnowsPopup)
				{
					if (widget is OverlayBaseWidget)
					{
						OverlayBaseWidget overlayBaseWidget = (OverlayBaseWidget)widget;
						if (overlayBaseWidget.PopupWidget == null)
						{
							this._parentKnowsPopup = false;
							break;
						}
						this._popupWidget = overlayBaseWidget.PopupWidget;
					}
					else
					{
						widget = widget.ParentWidget;
					}
				}
			}
			if (this.CurrentCharacterImageWidget != null)
			{
				this.CurrentCharacterImageWidget.Brush.SaturationFactor = (float)(this.IsMergedWithArmy ? 0 : (-100));
				this.CurrentCharacterImageWidget.Brush.ValueFactor = (float)(this.IsMergedWithArmy ? 0 : (-20));
			}
			if (!this._initialized)
			{
				this.BackgroundImageWidget.Brush = (this.IsPartyItem ? this.PartyBackgroundBrush : this.CharacterBackgroundBrush);
				this._initialized = true;
			}
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x000235D7 File Offset: 0x000217D7
		protected override void OnClick()
		{
			base.OnClick();
			if (this._popupWidget != null)
			{
				this._popupWidget.SetCurrentCharacter(this);
			}
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06000C96 RID: 3222 RVA: 0x000235F3 File Offset: 0x000217F3
		// (set) Token: 0x06000C97 RID: 3223 RVA: 0x000235FB File Offset: 0x000217FB
		[Editor(false)]
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (this._relation != value)
				{
					this._relation = value;
					base.OnPropertyChanged(value, "Relation");
				}
			}
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06000C98 RID: 3224 RVA: 0x00023619 File Offset: 0x00021819
		// (set) Token: 0x06000C99 RID: 3225 RVA: 0x00023621 File Offset: 0x00021821
		[Editor(false)]
		public string Location
		{
			get
			{
				return this._location;
			}
			set
			{
				if (this._location != value)
				{
					this._location = value;
					base.OnPropertyChanged<string>(value, "Location");
				}
			}
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x00023644 File Offset: 0x00021844
		// (set) Token: 0x06000C9B RID: 3227 RVA: 0x0002364C File Offset: 0x0002184C
		[Editor(false)]
		public string Power
		{
			get
			{
				return this._power;
			}
			set
			{
				if (this._power != value)
				{
					this._power = value;
					base.OnPropertyChanged<string>(value, "Power");
				}
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06000C9C RID: 3228 RVA: 0x0002366F File Offset: 0x0002186F
		// (set) Token: 0x06000C9D RID: 3229 RVA: 0x00023677 File Offset: 0x00021877
		[Editor(false)]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (this._description != value)
				{
					this._description = value;
					base.OnPropertyChanged<string>(value, "Description");
				}
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06000C9E RID: 3230 RVA: 0x0002369A File Offset: 0x0002189A
		// (set) Token: 0x06000C9F RID: 3231 RVA: 0x000236A2 File Offset: 0x000218A2
		[Editor(false)]
		public string Profession
		{
			get
			{
				return this._profession;
			}
			set
			{
				if (this._profession != value)
				{
					this._profession = value;
					base.OnPropertyChanged<string>(value, "Profession");
				}
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06000CA0 RID: 3232 RVA: 0x000236C5 File Offset: 0x000218C5
		// (set) Token: 0x06000CA1 RID: 3233 RVA: 0x000236CD File Offset: 0x000218CD
		[Editor(false)]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._name != value)
				{
					this._name = value;
					base.OnPropertyChanged<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06000CA2 RID: 3234 RVA: 0x000236F0 File Offset: 0x000218F0
		// (set) Token: 0x06000CA3 RID: 3235 RVA: 0x000236F8 File Offset: 0x000218F8
		[Editor(false)]
		public bool IsMergedWithArmy
		{
			get
			{
				return this._isMergedWithArmy;
			}
			set
			{
				if (this._isMergedWithArmy != value)
				{
					this._isMergedWithArmy = value;
				}
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x0002370A File Offset: 0x0002190A
		// (set) Token: 0x06000CA5 RID: 3237 RVA: 0x00023712 File Offset: 0x00021912
		[Editor(false)]
		public bool IsPartyItem
		{
			get
			{
				return this._isPartyItem;
			}
			set
			{
				if (this._isPartyItem != value)
				{
					this._isPartyItem = value;
				}
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06000CA6 RID: 3238 RVA: 0x00023724 File Offset: 0x00021924
		// (set) Token: 0x06000CA7 RID: 3239 RVA: 0x0002372C File Offset: 0x0002192C
		[Editor(false)]
		public Widget ContextMenu
		{
			get
			{
				return this._contextMenu;
			}
			set
			{
				if (this._contextMenu != value)
				{
					this._contextMenu = value;
					base.OnPropertyChanged<Widget>(value, "ContextMenu");
				}
			}
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x0002374A File Offset: 0x0002194A
		// (set) Token: 0x06000CA9 RID: 3241 RVA: 0x00023752 File Offset: 0x00021952
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

		// Token: 0x040005CC RID: 1484
		private bool _initialized;

		// Token: 0x040005CD RID: 1485
		private int _relation;

		// Token: 0x040005CE RID: 1486
		private string _location = "";

		// Token: 0x040005CF RID: 1487
		private string _description = "";

		// Token: 0x040005D0 RID: 1488
		private string _profession = "";

		// Token: 0x040005D1 RID: 1489
		private string _power = "";

		// Token: 0x040005D2 RID: 1490
		private string _name = "";

		// Token: 0x040005D3 RID: 1491
		private Widget _contextMenu;

		// Token: 0x040005D4 RID: 1492
		private ImageIdentifierWidget _currentCharacterImageWidget;

		// Token: 0x040005D5 RID: 1493
		private OverlayPopupWidget _popupWidget;

		// Token: 0x040005D6 RID: 1494
		private bool _parentKnowsPopup = true;

		// Token: 0x040005D7 RID: 1495
		private bool _isMergedWithArmy = true;

		// Token: 0x040005D8 RID: 1496
		private bool _isPartyItem;
	}
}
