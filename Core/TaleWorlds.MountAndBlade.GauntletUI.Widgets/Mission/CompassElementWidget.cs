using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C5 RID: 197
	public class CompassElementWidget : Widget
	{
		// Token: 0x060009E7 RID: 2535 RVA: 0x0001C424 File Offset: 0x0001A624
		public CompassElementWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0001C438 File Offset: 0x0001A638
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.HandleDistanceFading(dt);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0001C448 File Offset: 0x0001A648
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

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x060009EA RID: 2538 RVA: 0x0001C54A File Offset: 0x0001A74A
		// (set) Token: 0x060009EB RID: 2539 RVA: 0x0001C552 File Offset: 0x0001A752
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

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x060009EC RID: 2540 RVA: 0x0001C57B File Offset: 0x0001A77B
		// (set) Token: 0x060009ED RID: 2541 RVA: 0x0001C583 File Offset: 0x0001A783
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

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x060009EE RID: 2542 RVA: 0x0001C5A1 File Offset: 0x0001A7A1
		// (set) Token: 0x060009EF RID: 2543 RVA: 0x0001C5A9 File Offset: 0x0001A7A9
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

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x060009F0 RID: 2544 RVA: 0x0001C5C7 File Offset: 0x0001A7C7
		// (set) Token: 0x060009F1 RID: 2545 RVA: 0x0001C5CF File Offset: 0x0001A7CF
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

		// Token: 0x04000486 RID: 1158
		private float _alpha = 1f;

		// Token: 0x04000487 RID: 1159
		private float _position;

		// Token: 0x04000488 RID: 1160
		private int _distance;

		// Token: 0x04000489 RID: 1161
		private Widget _bannerWidget;

		// Token: 0x0400048A RID: 1162
		private Widget _flagWidget;
	}
}
