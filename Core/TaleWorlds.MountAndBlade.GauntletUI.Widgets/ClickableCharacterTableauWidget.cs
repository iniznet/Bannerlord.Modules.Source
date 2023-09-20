using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ClickableCharacterTableauWidget : CharacterTableauWidget
	{
		public ClickableCharacterTableauWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isMouseDown && !this._isDragging && (this._mousePressPos - base.EventManager.MousePosition).LengthSquared >= this._dragThresholdSqr)
			{
				this._isDragging = true;
				base.SetTextureProviderProperty("CurrentlyRotating", true);
			}
		}

		protected override void OnMousePressed()
		{
			this._isMouseDown = true;
			this._mousePressPos = base.EventManager.MousePosition;
		}

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

		private const float DragThreshold = 5f;

		private float _dragThresholdSqr = 25f;

		private bool _isMouseDown;

		private bool _isDragging;

		private Vec2 _mousePressPos;
	}
}
