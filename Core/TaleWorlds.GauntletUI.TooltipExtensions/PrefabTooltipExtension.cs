using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.TooltipExtensions
{
	public class PrefabTooltipExtension : PrefabExtension
	{
		private static WidgetAttributeTemplate GetHint(WidgetTemplate widgetTemplate)
		{
			return widgetTemplate.GetFirstAttributeIfExist<WidgetAttributeKeyTypeHint>();
		}

		protected override void RegisterAttributeTypes(WidgetAttributeContext widgetAttributeContext)
		{
			WidgetAttributeKeyTypeHint widgetAttributeKeyTypeHint = new WidgetAttributeKeyTypeHint();
			widgetAttributeContext.RegisterKeyType(widgetAttributeKeyTypeHint);
		}

		protected override void OnWidgetCreated(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, int childCount)
		{
		}

		protected override void OnSave(PrefabExtensionContext prefabExtensionContext, XmlNode node, WidgetTemplate widgetTemplate)
		{
		}

		protected override void OnAttributesSet(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
		}

		protected override void DoLoading(PrefabExtensionContext prefabExtensionContext, WidgetAttributeContext widgetAttributeContext, WidgetTemplate template, XmlNode node)
		{
		}

		protected override void OnLoadingFinished(WidgetPrefab widgetPrefab)
		{
		}

		protected override void AfterAttributesSet(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
			WidgetAttributeTemplate hint = PrefabTooltipExtension.GetHint(widgetInstantiationResult.Template);
			if (hint != null)
			{
				GauntletMovie extensionData = widgetCreationData.GetExtensionData<GauntletMovie>();
				UIContext context = widgetCreationData.Context;
				Widget widget = widgetInstantiationResult.Widget;
				WidgetFactory widgetFactory = widgetCreationData.WidgetFactory;
				WidgetPrefab customType = widgetFactory.GetCustomType("Hint");
				TooltipExtensionWidget tooltipExtensionWidget = new TooltipExtensionWidget(context);
				widget.AddChild(tooltipExtensionWidget);
				if (extensionData != null)
				{
					GauntletView component = widget.GetComponent<GauntletView>();
					tooltipExtensionWidget.WidthSizePolicy = SizePolicy.CoverChildren;
					tooltipExtensionWidget.HeightSizePolicy = SizePolicy.CoverChildren;
					tooltipExtensionWidget.IsEnabled = false;
					GauntletView gauntletView = new GauntletView(extensionData, component, tooltipExtensionWidget, 64);
					component.AddChild(gauntletView);
					tooltipExtensionWidget.AddComponent(gauntletView);
				}
				WidgetCreationData widgetCreationData2 = new WidgetCreationData(context, widgetFactory, tooltipExtensionWidget);
				if (extensionData != null)
				{
					widgetCreationData2.AddExtensionData(extensionData);
				}
				customType.Instantiate(widgetCreationData2, new Dictionary<string, WidgetAttributeTemplate> { 
				{
					"HintText",
					new WidgetAttributeTemplate
					{
						Key = "HintText",
						KeyType = new WidgetAttributeKeyTypeAttribute(),
						Value = hint.Value,
						ValueType = hint.ValueType
					}
				} });
			}
			foreach (WidgetInstantiationResult widgetInstantiationResult2 in widgetInstantiationResult.Children)
			{
				this.AfterAttributesSet(widgetCreationData, widgetInstantiationResult2, parameters);
			}
		}
	}
}
