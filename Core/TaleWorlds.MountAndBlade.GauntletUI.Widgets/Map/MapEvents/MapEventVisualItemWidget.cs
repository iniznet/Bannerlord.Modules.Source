using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapEvents
{
	// Token: 0x02000105 RID: 261
	public class MapEventVisualItemWidget : Widget
	{
		// Token: 0x06000D82 RID: 3458 RVA: 0x00025F0A File Offset: 0x0002410A
		public MapEventVisualItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x00025F13 File Offset: 0x00024113
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this.UpdatePosition();
			this.UpdateVisibility();
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x00025F28 File Offset: 0x00024128
		private void UpdateVisibility()
		{
			base.IsVisible = this.IsVisibleOnMap;
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x00025F38 File Offset: 0x00024138
		private void UpdatePosition()
		{
			if (this.IsVisibleOnMap)
			{
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
				base.ScaledPositionYOffset = this.Position.y - base.Size.Y;
				return;
			}
			base.ScaledPositionXOffset = -10000f;
			base.ScaledPositionYOffset = -10000f;
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06000D86 RID: 3462 RVA: 0x00025FA4 File Offset: 0x000241A4
		// (set) Token: 0x06000D87 RID: 3463 RVA: 0x00025FAC File Offset: 0x000241AC
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06000D88 RID: 3464 RVA: 0x00025FCF File Offset: 0x000241CF
		// (set) Token: 0x06000D89 RID: 3465 RVA: 0x00025FD7 File Offset: 0x000241D7
		public bool IsVisibleOnMap
		{
			get
			{
				return this._isVisibleOnMap;
			}
			set
			{
				if (this._isVisibleOnMap != value)
				{
					this._isVisibleOnMap = value;
					base.OnPropertyChanged(value, "IsVisibleOnMap");
				}
			}
		}

		// Token: 0x0400063D RID: 1597
		private Vec2 _position;

		// Token: 0x0400063E RID: 1598
		private bool _isVisibleOnMap;
	}
}
