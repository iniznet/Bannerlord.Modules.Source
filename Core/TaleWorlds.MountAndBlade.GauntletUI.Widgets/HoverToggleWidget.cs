using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class HoverToggleWidget : Widget
	{
		public bool IsOverWidget { get; private set; }

		public HoverToggleWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool IsMouseOverWidget()
		{
			return this.IsBetween(base.EventManager.MousePosition.X, base.GlobalPosition.X, base.GlobalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, base.GlobalPosition.Y, base.GlobalPosition.Y + base.Size.Y);
		}

		private bool IsBetween(float number, float min, float max)
		{
			return number > min && number < max;
		}

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

		private bool _hoverBegan;

		private Widget _widgetToShow;
	}
}
