using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class CompassElementWidget : Widget
	{
		public CompassElementWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.HandleDistanceFading(dt);
		}

		private void HandleDistanceFading(float dt)
		{
			if (this.Distance < 10)
			{
				this._alpha -= 2f * dt;
			}
			else
			{
				this._alpha += 2f * dt;
			}
			this._alpha = MBMath.ClampFloat(this._alpha, 0f, 1f);
			if (this.BannerWidget != null)
			{
				int childCount = this.BannerWidget.ChildCount;
				for (int i = 0; i < childCount; i++)
				{
					Widget child = this.FlagWidget.GetChild(i);
					Color color = child.Color;
					color.Alpha = this._alpha;
					child.Color = color;
				}
			}
			if (this.FlagWidget != null)
			{
				int childCount2 = this.FlagWidget.ChildCount;
				for (int j = 0; j < childCount2; j++)
				{
					Widget child2 = this.FlagWidget.GetChild(j);
					Color color2 = child2.Color;
					color2.Alpha = this._alpha;
					child2.Color = color2;
				}
			}
			base.IsVisible = this._alpha > 1E-05f;
		}

		[DataSourceProperty]
		public float Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (Math.Abs(this._position - value) > 1E-45f)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		[DataSourceProperty]
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (this._distance != value)
				{
					this._distance = value;
					base.OnPropertyChanged(value, "Distance");
				}
			}
		}

		[DataSourceProperty]
		public Widget BannerWidget
		{
			get
			{
				return this._bannerWidget;
			}
			set
			{
				if (this._bannerWidget != value)
				{
					this._bannerWidget = value;
					base.OnPropertyChanged<Widget>(value, "BannerWidget");
				}
			}
		}

		[DataSourceProperty]
		public Widget FlagWidget
		{
			get
			{
				return this._flagWidget;
			}
			set
			{
				if (this._flagWidget != value)
				{
					this._flagWidget = value;
					base.OnPropertyChanged<Widget>(value, "FlagWidget");
				}
			}
		}

		private float _alpha = 1f;

		private float _position;

		private int _distance;

		private Widget _bannerWidget;

		private Widget _flagWidget;
	}
}
