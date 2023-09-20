using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class AutoHideTextWidget : TextWidget
	{
		public AutoHideTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.WidgetToHideIfEmpty != null)
			{
				this.WidgetToHideIfEmpty.IsVisible = base.Text != string.Empty;
			}
			base.IsVisible = base.Text != string.Empty;
		}

		[Editor(false)]
		public Widget WidgetToHideIfEmpty
		{
			get
			{
				return this._widgetToHideIfEmpty;
			}
			set
			{
				if (this._widgetToHideIfEmpty != value)
				{
					this._widgetToHideIfEmpty = value;
					base.OnPropertyChanged<Widget>(value, "WidgetToHideIfEmpty");
				}
			}
		}

		private Widget _widgetToHideIfEmpty;
	}
}
