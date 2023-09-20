using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Recruitment
{
	// Token: 0x020000F0 RID: 240
	public class RecruitTroopPanelButtonWidget : ButtonWidget
	{
		// Token: 0x06000C6C RID: 3180 RVA: 0x00022DB7 File Offset: 0x00020FB7
		public RecruitTroopPanelButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00022DC0 File Offset: 0x00020FC0
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.IsTroopEmpty)
			{
				if (this.PlayerHasEnoughRelation)
				{
					this.SetState("EmptyEnoughRelation");
				}
				else
				{
					this.SetState("EmptyNoRelation");
				}
			}
			else if (this.CanBeRecruited)
			{
				this.SetState("Available");
			}
			else
			{
				this.SetState("Unavailable");
			}
			if (!this.PlayerHasEnoughRelation && !this.IsTroopEmpty && this.CharacterImageWidget != null)
			{
				this.CharacterImageWidget.Brush.ValueFactor = -50f;
				this.CharacterImageWidget.Brush.SaturationFactor = -100f;
			}
			if (this.CharacterImageWidget != null)
			{
				this.CharacterImageWidget.IsHidden = this.IsTroopEmpty;
			}
			this.RemoveFromCartButton.SetState(((base.IsHovered || base.IsPressed || base.IsSelected) && ((!this.IsTroopEmpty && this.PlayerHasEnoughRelation) || this.IsInCart)) ? "Hovered" : "Default");
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x00022EC0 File Offset: 0x000210C0
		private bool IsMouseOverWidget()
		{
			Vector2 globalPosition = base.GlobalPosition;
			return this.IsBetween(base.EventManager.MousePosition.X, globalPosition.X, globalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, globalPosition.Y, globalPosition.Y + base.Size.Y);
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x00022F34 File Offset: 0x00021134
		private bool IsBetween(float number, float min, float max)
		{
			return number >= min && number <= max;
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00022F43 File Offset: 0x00021143
		// (set) Token: 0x06000C71 RID: 3185 RVA: 0x00022F4B File Offset: 0x0002114B
		[Editor(false)]
		public bool CanBeRecruited
		{
			get
			{
				return this._canBeRecruited;
			}
			set
			{
				if (this._canBeRecruited != value)
				{
					this._canBeRecruited = value;
					base.OnPropertyChanged(value, "CanBeRecruited");
				}
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x00022F69 File Offset: 0x00021169
		// (set) Token: 0x06000C73 RID: 3187 RVA: 0x00022F71 File Offset: 0x00021171
		[Editor(false)]
		public bool IsInCart
		{
			get
			{
				return this._isInCart;
			}
			set
			{
				if (this._isInCart != value)
				{
					this._isInCart = value;
					base.OnPropertyChanged(value, "IsInCart");
				}
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x00022F8F File Offset: 0x0002118F
		// (set) Token: 0x06000C75 RID: 3189 RVA: 0x00022F97 File Offset: 0x00021197
		[Editor(false)]
		public ButtonWidget RemoveFromCartButton
		{
			get
			{
				return this._removeFromCartButton;
			}
			set
			{
				if (this._removeFromCartButton != value)
				{
					this._removeFromCartButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "RemoveFromCartButton");
				}
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06000C76 RID: 3190 RVA: 0x00022FB5 File Offset: 0x000211B5
		// (set) Token: 0x06000C77 RID: 3191 RVA: 0x00022FBD File Offset: 0x000211BD
		[Editor(false)]
		public ImageIdentifierWidget CharacterImageWidget
		{
			get
			{
				return this._characterImageWidget;
			}
			set
			{
				if (this._characterImageWidget != value)
				{
					this._characterImageWidget = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "CharacterImageWidget");
				}
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x00022FDB File Offset: 0x000211DB
		// (set) Token: 0x06000C79 RID: 3193 RVA: 0x00022FE3 File Offset: 0x000211E3
		[Editor(false)]
		public bool IsTroopEmpty
		{
			get
			{
				return this._isTroopEmpty;
			}
			set
			{
				if (this._isTroopEmpty != value)
				{
					this._isTroopEmpty = value;
					base.OnPropertyChanged(value, "IsTroopEmpty");
				}
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06000C7A RID: 3194 RVA: 0x00023001 File Offset: 0x00021201
		// (set) Token: 0x06000C7B RID: 3195 RVA: 0x00023009 File Offset: 0x00021209
		[Editor(false)]
		public bool PlayerHasEnoughRelation
		{
			get
			{
				return this._playerHasEnoughRelation;
			}
			set
			{
				if (this._playerHasEnoughRelation != value)
				{
					this._playerHasEnoughRelation = value;
					base.OnPropertyChanged(value, "PlayerHasEnoughRelation");
				}
			}
		}

		// Token: 0x040005BB RID: 1467
		private bool _canBeRecruited;

		// Token: 0x040005BC RID: 1468
		private bool _isInCart;

		// Token: 0x040005BD RID: 1469
		private bool _playerHasEnoughRelation;

		// Token: 0x040005BE RID: 1470
		private bool _isTroopEmpty;

		// Token: 0x040005BF RID: 1471
		private ButtonWidget _removeFromCartButton;

		// Token: 0x040005C0 RID: 1472
		private ImageIdentifierWidget _characterImageWidget;
	}
}
