using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	public static class WidgetInstantiationResultDatabindingExtension
	{
		public static GauntletView GetGauntletView(this WidgetInstantiationResult widgetInstantiationResult)
		{
			Widget widget;
			if (widgetInstantiationResult.CustomWidgetInstantiationData != null)
			{
				widget = widgetInstantiationResult.CustomWidgetInstantiationData.Widget;
			}
			else
			{
				widget = widgetInstantiationResult.Widget;
			}
			return widget.GetComponent<GauntletView>();
		}
	}
}
