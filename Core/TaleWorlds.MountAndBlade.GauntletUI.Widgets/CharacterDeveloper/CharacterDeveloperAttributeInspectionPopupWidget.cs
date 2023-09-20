using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x0200015C RID: 348
	public class CharacterDeveloperAttributeInspectionPopupWidget : Widget
	{
		// Token: 0x060011F2 RID: 4594 RVA: 0x00031919 File Offset: 0x0002FB19
		public CharacterDeveloperAttributeInspectionPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x00031924 File Offset: 0x0002FB24
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.ParentWidget.IsVisible && this._latestMouseUpWidgetWhenActivated != base.EventManager.LatestMouseUpWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
			{
				this.Deactivate();
			}
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00031971 File Offset: 0x0002FB71
		private void Activate()
		{
			this._latestMouseUpWidgetWhenActivated = base.EventManager.LatestMouseDownWidget;
			base.ParentWidget.IsVisible = true;
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00031990 File Offset: 0x0002FB90
		private void Deactivate()
		{
			base.EventFired("Deactivate", Array.Empty<object>());
			base.ParentWidget.IsVisible = false;
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x060011F6 RID: 4598 RVA: 0x000319AE File Offset: 0x0002FBAE
		// (set) Token: 0x060011F7 RID: 4599 RVA: 0x000319B6 File Offset: 0x0002FBB6
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

		// Token: 0x04000838 RID: 2104
		private Widget _latestMouseUpWidgetWhenActivated;

		// Token: 0x04000839 RID: 2105
		private bool _isActive;
	}
}
