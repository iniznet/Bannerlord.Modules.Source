using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.MapBar
{
	public class MapTimeImageBrushWidget : BrushWidget
	{
		public MapTimeImageBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			StyleLayer layer = base.Brush.DefaultStyle.GetLayer("Default");
			StyleLayer layer2 = base.Brush.DefaultStyle.GetLayer("Part2");
			if (!this._initialized)
			{
				this._offset = layer2.XOffset;
				this._initialized = true;
			}
			float overridenWidth = layer.OverridenWidth;
			float num = -overridenWidth * ((float)this.DayTime / 24f) + this._offset;
			float num2;
			if (this.DayTime > 12.0)
			{
				num2 = num + overridenWidth;
			}
			else
			{
				num2 = num - overridenWidth;
			}
			layer.XOffset = num;
			layer2.XOffset = num2;
			base.OnRender(twoDimensionContext, drawContext);
		}

		[Editor(false)]
		public double DayTime
		{
			get
			{
				return this._dayTime;
			}
			set
			{
				if (this._dayTime != value)
				{
					this._dayTime = value;
					base.OnPropertyChanged(value, "DayTime");
				}
			}
		}

		private float _offset;

		private bool _initialized;

		private double _dayTime;
	}
}
