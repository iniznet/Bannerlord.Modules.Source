using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class CharacterDeveloperPerkSelectionWidget : Widget
	{
		public CharacterDeveloperPerkSelectionWidget(UIContext context)
			: base(context)
		{
			base.IsVisible = false;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible && this._latestMouseUpWidgetWhenActivated != base.EventManager.LatestMouseUpWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
			{
				this.Deactivate();
			}
			this.UpdatePosition();
		}

		private void UpdatePosition()
		{
			if (base.IsVisible && this._latestMouseUpWidgetWhenActivated != null)
			{
				float num = this._latestMouseUpWidgetWhenActivated.GlobalPosition.X + this._latestMouseUpWidgetWhenActivated.Size.X + this._distBetweenPerkItemsMultiplier * 2f * base._scaleToUse;
				float num2 = 0f;
				if (base.GetChild(0).ChildCount > 1)
				{
					PerkItemButtonWidget perkItemButtonWidget;
					if ((perkItemButtonWidget = this._latestMouseUpWidgetWhenActivated as PerkItemButtonWidget) != null)
					{
						if (perkItemButtonWidget.AlternativeType == 1)
						{
							num2 = this._latestMouseUpWidgetWhenActivated.GlobalPosition.Y + (this._latestMouseUpWidgetWhenActivated.Size.Y - 4f * base._scaleToUse) - base.Size.Y / 2f;
						}
						else if (perkItemButtonWidget.AlternativeType == 2)
						{
							num2 = this._latestMouseUpWidgetWhenActivated.GlobalPosition.Y - base.Size.Y / 2f;
						}
					}
				}
				else
				{
					num2 = this._latestMouseUpWidgetWhenActivated.GlobalPosition.Y + this._latestMouseUpWidgetWhenActivated.Size.Y / 2f - base.Size.Y / 2f;
				}
				base.ScaledPositionXOffset = MathF.Clamp(num - base.EventManager.LeftUsableAreaStart, 0f, base.EventManager.PageSize.X - base.Size.X);
				base.ScaledPositionYOffset = MathF.Clamp(num2 - base.EventManager.TopUsableAreaStart, 0f, base.EventManager.PageSize.Y - base.Size.Y);
			}
		}

		private void Activate()
		{
			if (this._latestMouseUpWidgetWhenActivated == null)
			{
				this._latestMouseUpWidgetWhenActivated = base.EventManager.LatestMouseDownWidget;
			}
			base.IsVisible = true;
		}

		private void Deactivate()
		{
			base.EventFired("Deactivate", Array.Empty<object>());
			base.IsVisible = false;
			this._latestMouseUpWidgetWhenActivated = null;
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					if (this._isActive)
					{
						this.Activate();
						return;
					}
					this.Deactivate();
				}
			}
		}

		private float _distBetweenPerkItemsMultiplier = 16f;

		private Widget _latestMouseUpWidgetWhenActivated;

		private bool _isActive;
	}
}
