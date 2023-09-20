using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets.Graph
{
	public class GraphLineWidget : Widget
	{
		public string LineBrushStateName { get; set; }

		public GraphLineWidget(UIContext context)
			: base(context)
		{
		}

		private void OnPointContainerEventFire(Widget widget, string eventName, object[] eventArgs)
		{
			GraphLinePointWidget graphLinePointWidget;
			if (eventName == "ItemAdd" && eventArgs.Length != 0 && (graphLinePointWidget = eventArgs[0] as GraphLinePointWidget) != null)
			{
				Action<GraphLineWidget, GraphLinePointWidget> onPointAdded = this.OnPointAdded;
				if (onPointAdded == null)
				{
					return;
				}
				onPointAdded(this, graphLinePointWidget);
			}
		}

		public Widget PointContainerWidget
		{
			get
			{
				return this._pointContainerWidget;
			}
			set
			{
				if (value != this._pointContainerWidget)
				{
					if (this._pointContainerWidget != null)
					{
						this._pointContainerWidget.EventFire -= this.OnPointContainerEventFire;
					}
					this._pointContainerWidget = value;
					if (this._pointContainerWidget != null)
					{
						this._pointContainerWidget.EventFire += this.OnPointContainerEventFire;
					}
					base.OnPropertyChanged<Widget>(value, "PointContainerWidget");
				}
			}
		}

		public Action<GraphLineWidget, GraphLinePointWidget> OnPointAdded;

		private Widget _pointContainerWidget;
	}
}
