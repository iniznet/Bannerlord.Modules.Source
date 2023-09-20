using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	public class AlwaysVisibleNameMarkerListPanel : ListPanel
	{
		private float _normalOpacity
		{
			get
			{
				return 0.5f;
			}
		}

		private float _screenCenterOpacity
		{
			get
			{
				return 0.15f;
			}
		}

		private float _stayOnScreenTimeInSeconds
		{
			get
			{
				return 5f;
			}
		}

		public AlwaysVisibleNameMarkerListPanel(UIContext context)
			: base(context)
		{
			this._parentScreenWidget = base.EventManager.Root.GetChild(0).GetChild(0);
		}

		protected override void OnLateUpdate(float dt)
		{
			foreach (Widget widget in base.AllChildrenAndThis)
			{
				widget.IsVisible = true;
			}
			base.ScaledPositionYOffset = this.Position.y - base.Size.Y / 2f;
			base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
			this.UpdateOpacity();
			if (this._totalDt > this._stayOnScreenTimeInSeconds)
			{
				base.EventFired("Remove", Array.Empty<object>());
			}
			this._totalDt += dt;
		}

		private void UpdateOpacity()
		{
			Vec2 vec = new Vec2(base.Context.TwoDimensionContext.Platform.Width / 2f, base.Context.TwoDimensionContext.Platform.Height / 2f);
			Vec2 vec2 = new Vec2(base.ScaledPositionXOffset, base.ScaledPositionYOffset);
			float num = ((vec2.Distance(vec) <= 150f) ? this._screenCenterOpacity : this._normalOpacity);
			this.SetGlobalAlphaRecursively(num);
		}

		[DataSourceProperty]
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

		private Widget _parentScreenWidget;

		private float _totalDt;

		private Vec2 _position;
	}
}
