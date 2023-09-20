using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapEvents
{
	public class MapEventVisualItemWidget : Widget
	{
		public MapEventVisualItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this.UpdatePosition();
			this.UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			base.IsVisible = this.IsVisibleOnMap;
		}

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

		private Vec2 _position;

		private bool _isVisibleOnMap;
	}
}
