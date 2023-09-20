using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.MapBar
{
	// Token: 0x020000F8 RID: 248
	public class MapTimeImageBrushWidget : BrushWidget
	{
		// Token: 0x06000CE2 RID: 3298 RVA: 0x0002408B File Offset: 0x0002228B
		public MapTimeImageBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x00024094 File Offset: 0x00022294
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

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06000CE4 RID: 3300 RVA: 0x0002413C File Offset: 0x0002233C
		// (set) Token: 0x06000CE5 RID: 3301 RVA: 0x00024144 File Offset: 0x00022344
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

		// Token: 0x040005F6 RID: 1526
		private float _offset;

		// Token: 0x040005F7 RID: 1527
		private bool _initialized;

		// Token: 0x040005F8 RID: 1528
		private double _dayTime;
	}
}
