using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets.Graph
{
	// Token: 0x02000014 RID: 20
	public class GraphLineWidget : Widget
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00006609 File Offset: 0x00004809
		// (set) Token: 0x06000116 RID: 278 RVA: 0x00006611 File Offset: 0x00004811
		public string LineBrushStateName { get; set; }

		// Token: 0x06000117 RID: 279 RVA: 0x0000661A File Offset: 0x0000481A
		public GraphLineWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00006624 File Offset: 0x00004824
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

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00006660 File Offset: 0x00004860
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00006668 File Offset: 0x00004868
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

		// Token: 0x04000081 RID: 129
		public Action<GraphLineWidget, GraphLinePointWidget> OnPointAdded;

		// Token: 0x04000082 RID: 130
		private Widget _pointContainerWidget;
	}
}
