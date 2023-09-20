using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.Data
{
	public class GeneratedWidgetData : WidgetComponent
	{
		public GeneratedWidgetData(Widget target)
			: base(target)
		{
		}

		public object Data { get; set; }
	}
}
