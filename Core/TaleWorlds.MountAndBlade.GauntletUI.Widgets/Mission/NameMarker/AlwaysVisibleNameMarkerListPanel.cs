using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	// Token: 0x020000D9 RID: 217
	public class AlwaysVisibleNameMarkerListPanel : ListPanel
	{
		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000B01 RID: 2817 RVA: 0x0001E879 File Offset: 0x0001CA79
		private float _normalOpacity
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000B02 RID: 2818 RVA: 0x0001E880 File Offset: 0x0001CA80
		private float _screenCenterOpacity
		{
			get
			{
				return 0.15f;
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000B03 RID: 2819 RVA: 0x0001E887 File Offset: 0x0001CA87
		private float _stayOnScreenTimeInSeconds
		{
			get
			{
				return 5f;
			}
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x0001E88E File Offset: 0x0001CA8E
		public AlwaysVisibleNameMarkerListPanel(UIContext context)
			: base(context)
		{
			this._parentScreenWidget = base.EventManager.Root.GetChild(0).GetChild(0);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x0001E8B4 File Offset: 0x0001CAB4
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

		// Token: 0x06000B06 RID: 2822 RVA: 0x0001E978 File Offset: 0x0001CB78
		private void UpdateOpacity()
		{
			Vec2 vec = new Vec2(base.Context.TwoDimensionContext.Platform.Width / 2f, base.Context.TwoDimensionContext.Platform.Height / 2f);
			Vec2 vec2 = new Vec2(base.ScaledPositionXOffset, base.ScaledPositionYOffset);
			float num = ((vec2.Distance(vec) <= 150f) ? this._screenCenterOpacity : this._normalOpacity);
			this.SetGlobalAlphaRecursively(num);
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000B07 RID: 2823 RVA: 0x0001E9FF File Offset: 0x0001CBFF
		// (set) Token: 0x06000B08 RID: 2824 RVA: 0x0001EA07 File Offset: 0x0001CC07
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

		// Token: 0x04000506 RID: 1286
		private Widget _parentScreenWidget;

		// Token: 0x04000507 RID: 1287
		private float _totalDt;

		// Token: 0x04000508 RID: 1288
		private Vec2 _position;
	}
}
