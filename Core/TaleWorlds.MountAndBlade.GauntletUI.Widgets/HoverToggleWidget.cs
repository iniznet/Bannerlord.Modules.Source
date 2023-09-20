using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000020 RID: 32
	public class HoverToggleWidget : Widget
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000194 RID: 404 RVA: 0x000066D9 File Offset: 0x000048D9
		// (set) Token: 0x06000195 RID: 405 RVA: 0x000066E1 File Offset: 0x000048E1
		public bool IsOverWidget { get; private set; }

		// Token: 0x06000196 RID: 406 RVA: 0x000066EA File Offset: 0x000048EA
		public HoverToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000066F4 File Offset: 0x000048F4
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.IsVisible)
			{
				this.IsOverWidget = this.IsMouseOverWidget();
				if (this.IsOverWidget && !this._hoverBegan)
				{
					base.EventFired("HoverBegin", Array.Empty<object>());
					this._hoverBegan = true;
				}
				else if (!this.IsOverWidget && this._hoverBegan)
				{
					base.EventFired("HoverEnd", Array.Empty<object>());
					this._hoverBegan = false;
				}
				if (this.WidgetToShow != null)
				{
					this.WidgetToShow.IsVisible = this._hoverBegan;
				}
			}
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00006788 File Offset: 0x00004988
		private bool IsMouseOverWidget()
		{
			return this.IsBetween(base.EventManager.MousePosition.X, base.GlobalPosition.X, base.GlobalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, base.GlobalPosition.Y, base.GlobalPosition.Y + base.Size.Y);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00006809 File Offset: 0x00004A09
		private bool IsBetween(float number, float min, float max)
		{
			return number > min && number < max;
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600019A RID: 410 RVA: 0x00006815 File Offset: 0x00004A15
		// (set) Token: 0x0600019B RID: 411 RVA: 0x0000681D File Offset: 0x00004A1D
		[Editor(false)]
		public Widget WidgetToShow
		{
			get
			{
				return this._widgetToShow;
			}
			set
			{
				if (this._widgetToShow != value)
				{
					this._widgetToShow = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToShow");
				}
			}
		}

		// Token: 0x040000C7 RID: 199
		private bool _hoverBegan;

		// Token: 0x040000C8 RID: 200
		private Widget _widgetToShow;
	}
}
