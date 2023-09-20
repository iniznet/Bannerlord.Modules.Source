using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	// Token: 0x020000FB RID: 251
	public class MapSiegeConstructionControllerWidget : Widget
	{
		// Token: 0x06000CFB RID: 3323 RVA: 0x000246EF File Offset: 0x000228EF
		public MapSiegeConstructionControllerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x000246F8 File Offset: 0x000228F8
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

		// Token: 0x06000CFD RID: 3325 RVA: 0x0002479B File Offset: 0x0002299B
		public void SetCurrentPOIWidget(MapSiegePOIBrushWidget widget)
		{
			if (widget == null || widget == this._currentWidget)
			{
				this._currentWidget = null;
				return;
			}
			this._currentWidget = (widget.IsPlayerSidePOI ? widget : null);
		}

		// Token: 0x04000605 RID: 1541
		private MapSiegePOIBrushWidget _currentWidget;
	}
}
