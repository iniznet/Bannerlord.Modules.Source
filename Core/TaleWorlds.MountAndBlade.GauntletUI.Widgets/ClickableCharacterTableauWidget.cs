using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200000E RID: 14
	public class ClickableCharacterTableauWidget : CharacterTableauWidget
	{
		// Token: 0x0600008D RID: 141 RVA: 0x000034C2 File Offset: 0x000016C2
		public ClickableCharacterTableauWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000034D8 File Offset: 0x000016D8
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isMouseDown && !this._isDragging && (this._mousePressPos - base.EventManager.MousePosition).LengthSquared >= this._dragThresholdSqr)
			{
				this._isDragging = true;
				base.SetTextureProviderProperty("CurrentlyRotating", true);
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000353F File Offset: 0x0000173F
		protected override void OnMousePressed()
		{
			this._isMouseDown = true;
			this._mousePressPos = base.EventManager.MousePosition;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000355E File Offset: 0x0000175E
		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
			if (!this._isDragging)
			{
				base.EventFired("Click", Array.Empty<object>());
			}
			this._isDragging = false;
			this._isMouseDown = false;
		}

		// Token: 0x04000044 RID: 68
		private const float DragThreshold = 5f;

		// Token: 0x04000045 RID: 69
		private float _dragThresholdSqr = 25f;

		// Token: 0x04000046 RID: 70
		private bool _isMouseDown;

		// Token: 0x04000047 RID: 71
		private bool _isDragging;

		// Token: 0x04000048 RID: 72
		private Vec2 _mousePressPos;
	}
}
