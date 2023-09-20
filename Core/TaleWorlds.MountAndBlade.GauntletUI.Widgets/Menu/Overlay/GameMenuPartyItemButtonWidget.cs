using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	public class GameMenuPartyItemButtonWidget : ButtonWidget
	{
		public Brush PartyBackgroundBrush { get; set; }

		public Brush CharacterBackgroundBrush { get; set; }

		public ImageWidget BackgroundImageWidget { get; set; }

		public GameMenuPartyItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		private string GetRelationBackgroundName(int relation)
		{
			return "";
		}

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

		protected override void OnClick()
		{
			base.OnClick();
			if (this._popupWidget != null)
			{
				this._popupWidget.SetCurrentCharacter(this);
			}
		}

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

		private bool _initialized;

		private int _relation;

		private string _location = "";

		private string _description = "";

		private string _profession = "";

		private string _power = "";

		private string _name = "";

		private Widget _contextMenu;

		private ImageIdentifierWidget _currentCharacterImageWidget;

		private OverlayPopupWidget _popupWidget;

		private bool _parentKnowsPopup = true;

		private bool _isMergedWithArmy = true;

		private bool _isPartyItem;
	}
}
