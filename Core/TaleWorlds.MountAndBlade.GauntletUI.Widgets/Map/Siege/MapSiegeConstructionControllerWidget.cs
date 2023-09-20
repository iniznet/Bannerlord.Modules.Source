using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	public class MapSiegeConstructionControllerWidget : Widget
	{
		public MapSiegeConstructionControllerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			float num;
			if (this._currentWidget != null)
			{
				base.PositionXOffset = this._currentWidget.PositionXOffset + this._currentWidget.Size.X * base._inverseScaleToUse;
				base.PositionYOffset = this._currentWidget.PositionYOffset;
				num = this._currentWidget.ReadOnlyBrush.GlobalAlphaFactor;
			}
			else
			{
				base.PositionXOffset = -1000f;
				base.PositionYOffset = -1000f;
				num = 0f;
			}
			base.IsEnabled = num >= 0.95f;
			this.SetGlobalAlphaRecursively(num);
		}

		public void SetCurrentPOIWidget(MapSiegePOIBrushWidget widget)
		{
			if (widget == null || widget == this._currentWidget)
			{
				this._currentWidget = null;
				return;
			}
			this._currentWidget = (widget.IsPlayerSidePOI ? widget : null);
		}

		private MapSiegePOIBrushWidget _currentWidget;
	}
}
