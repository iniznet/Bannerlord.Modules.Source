using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x0200015F RID: 351
	public class CharacterDeveloperPerkSelectionWidget : Widget
	{
		// Token: 0x0600120E RID: 4622 RVA: 0x00031F64 File Offset: 0x00030164
		public CharacterDeveloperPerkSelectionWidget(UIContext context)
			: base(context)
		{
			base.IsVisible = false;
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00031F80 File Offset: 0x00030180
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible && this._latestMouseUpWidgetWhenActivated != base.EventManager.LatestMouseUpWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
			{
				this.Deactivate();
			}
			this.UpdatePosition();
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x00031FD0 File Offset: 0x000301D0
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

		// Token: 0x06001211 RID: 4625 RVA: 0x00032179 File Offset: 0x00030379
		private void Activate()
		{
			if (this._latestMouseUpWidgetWhenActivated == null)
			{
				this._latestMouseUpWidgetWhenActivated = base.EventManager.LatestMouseDownWidget;
			}
			base.IsVisible = true;
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x0003219B File Offset: 0x0003039B
		private void Deactivate()
		{
			base.EventFired("Deactivate", Array.Empty<object>());
			base.IsVisible = false;
			this._latestMouseUpWidgetWhenActivated = null;
		}

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06001213 RID: 4627 RVA: 0x000321BB File Offset: 0x000303BB
		// (set) Token: 0x06001214 RID: 4628 RVA: 0x000321C3 File Offset: 0x000303C3
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

		// Token: 0x04000844 RID: 2116
		private float _distBetweenPerkItemsMultiplier = 16f;

		// Token: 0x04000845 RID: 2117
		private Widget _latestMouseUpWidgetWhenActivated;

		// Token: 0x04000846 RID: 2118
		private bool _isActive;
	}
}
