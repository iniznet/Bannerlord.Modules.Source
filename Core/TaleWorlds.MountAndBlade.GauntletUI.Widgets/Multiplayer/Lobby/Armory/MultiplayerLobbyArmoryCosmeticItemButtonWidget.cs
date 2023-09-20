using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyArmoryCosmeticItemButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyArmoryCosmeticItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.EventManager.HoveredView == this && Input.IsKeyPressed(InputKey.ControllerRUp))
			{
				this.OnMouseAlternatePressed();
				return;
			}
			if (base.EventManager.HoveredView == this && Input.IsKeyReleased(InputKey.ControllerRUp))
			{
				this.OnMouseAlternateReleased();
			}
		}

		private void UpdateSelectableState()
		{
			this._selectableTimer = 0f;
			base.IsDisabled = !this.IsSelectable;
			this._animationStartAlpha = (this.IsSelectable ? this.NonSelectableStateAlpha : this.SelectableStateAlpha);
			this._animationTargetAlpha = (this.IsSelectable ? this.SelectableStateAlpha : this.NonSelectableStateAlpha);
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.AnimateSelectableState), 1);
		}

		private void AnimateSelectableState(float dt)
		{
			this._selectableTimer += dt;
			float num;
			if (this._selectableTimer < this.SelectableStateAnimationDuration)
			{
				num = this._selectableTimer / this.SelectableStateAnimationDuration;
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.AnimateSelectableState), 1);
			}
			else
			{
				num = 1f;
			}
			float num2 = MathF.Lerp(this._animationStartAlpha, this._animationTargetAlpha, num, 1E-05f);
			base.IsVisible = num2 != 0f;
			this.SetGlobalAlphaRecursively(num2);
		}

		protected override void OnClick()
		{
			base.OnClick();
			if (this.IsUnlocked)
			{
				this.HandleSoundEvent();
				return;
			}
			base.EventFired("Obtain", Array.Empty<object>());
		}

		protected override void OnAlternateClick()
		{
			base.OnAlternateClick();
			this.HandleSoundEvent();
		}

		private void HandleSoundEvent()
		{
			int itemType = this.ItemType;
			switch (itemType)
			{
			case 12:
				base.EventFired("WearHelmet", Array.Empty<object>());
				return;
			case 13:
				base.EventFired("WearArmorBig", Array.Empty<object>());
				return;
			case 14:
			case 15:
				break;
			default:
				if (itemType != 22)
				{
					base.EventFired("WearGeneric", Array.Empty<object>());
					return;
				}
				break;
			}
			base.EventFired("WearArmorSmall", Array.Empty<object>());
		}

		public int ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (value != this._itemType)
				{
					this._itemType = value;
					base.OnPropertyChanged(value, "ItemType");
				}
			}
		}

		public bool IsUnlocked
		{
			get
			{
				return this._isUnlocked;
			}
			set
			{
				if (value != this._isUnlocked)
				{
					this._isUnlocked = value;
					base.OnPropertyChanged(value, "IsUnlocked");
				}
			}
		}

		public float SelectableStateAnimationDuration
		{
			get
			{
				return this._selectableStateAnimationDuration;
			}
			set
			{
				if (value != this._selectableStateAnimationDuration)
				{
					this._selectableStateAnimationDuration = value;
					base.OnPropertyChanged(value, "SelectableStateAnimationDuration");
				}
			}
		}

		public float SelectableStateAlpha
		{
			get
			{
				return this._selectableStateAlpha;
			}
			set
			{
				if (value != this._selectableStateAlpha)
				{
					this._selectableStateAlpha = value;
					base.OnPropertyChanged(value, "SelectableStateAlpha");
				}
			}
		}

		public float NonSelectableStateAlpha
		{
			get
			{
				return this._nonSelectableStateAlpha;
			}
			set
			{
				if (value != this._nonSelectableStateAlpha)
				{
					this._nonSelectableStateAlpha = value;
					base.OnPropertyChanged(value, "NonSelectableStateAlpha");
				}
			}
		}

		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChanged(value, "IsSelectable");
					this.UpdateSelectableState();
				}
			}
		}

		private float _selectableTimer;

		private float _animationTargetAlpha;

		private float _animationStartAlpha;

		private int _itemType;

		private bool _isUnlocked;

		private float _selectableStateAnimationDuration;

		private float _selectableStateAlpha;

		private float _nonSelectableStateAlpha;

		private bool _isSelectable;
	}
}
