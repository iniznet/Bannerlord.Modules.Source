using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.TooltipExtensions
{
	// Token: 0x02000002 RID: 2
	public class PrefabTooltipExtension : PrefabExtension
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		private static WidgetAttributeTemplate GetHint(WidgetTemplate widgetTemplate)
		{
			return widgetTemplate.GetFirstAttributeIfExist<WidgetAttributeKeyTypeHint>();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		protected override void RegisterAttributeTypes(WidgetAttributeContext widgetAttributeContext)
		{
			WidgetAttributeKeyTypeHint widgetAttributeKeyTypeHint = new WidgetAttributeKeyTypeHint();
			widgetAttributeContext.RegisterKeyType(widgetAttributeKeyTypeHint);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000206A File Offset: 0x0000026A
		protected override void OnWidgetCreated(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, int childCount)
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000206C File Offset: 0x0000026C
		protected override void OnSave(PrefabExtensionContext prefabExtensionContext, XmlNode node, WidgetTemplate widgetTemplate)
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000206E File Offset: 0x0000026E
		protected override void OnAttributesSet(WidgetCreationData widgetCreationData, WidgetInstantiationResult widgetInstantiationResult, Dictionary<string, WidgetAttributeTemplate> parameters)
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002070 File Offset: 0x00000270
		protected override void DoLoading(PrefabExtensionContext prefabExtensionContext, WidgetAttributeContext widgetAttributeContext, WidgetTemplate template, XmlNode node)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002072 File Offset: 0x00000272
		protected override void OnLoadingFinished(WidgetPrefab widgetPrefab)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002074 File Offset: 0x00000274
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
