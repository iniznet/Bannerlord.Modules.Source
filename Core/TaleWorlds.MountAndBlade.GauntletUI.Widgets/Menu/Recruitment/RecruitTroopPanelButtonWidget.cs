using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Recruitment
{
	public class RecruitTroopPanelButtonWidget : ButtonWidget
	{
		public RecruitTroopPanelButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool IsMouseOverWidget()
		{
			Vector2 globalPosition = base.GlobalPosition;
			return this.IsBetween(base.EventManager.MousePosition.X, globalPosition.X, globalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, globalPosition.Y, globalPosition.Y + base.Size.Y);
		}

		private bool IsBetween(float number, float min, float max)
		{
			return number >= min && number <= max;
		}

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

		private bool _canBeRecruited;

		private bool _isInCart;

		private bool _playerHasEnoughRelation;

		private bool _isTroopEmpty;

		private ButtonWidget _removeFromCartButton;

		private ImageIdentifierWidget _characterImageWidget;
	}
}
