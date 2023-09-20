using System;

namespace TaleWorlds.GauntletUI
{
	public class ContainerItemDescription
	{
		public string WidgetId { get; set; }

		public int WidgetIndex { get; set; }

		public float WidthStretchRatio { get; set; }

		public float HeightStretchRatio { get; set; }

		public ContainerItemDescription()
		{
			this.WidgetId = "";
			this.WidgetIndex = -1;
			this.WidthStretchRatio = 1f;
			this.HeightStretchRatio = 1f;
		}
	}
}
