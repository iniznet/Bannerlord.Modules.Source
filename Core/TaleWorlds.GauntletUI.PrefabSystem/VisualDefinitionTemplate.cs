using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x0200000B RID: 11
	public class VisualDefinitionTemplate
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000028D0 File Offset: 0x00000AD0
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000028D8 File Offset: 0x00000AD8
		public string Name { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000028E1 File Offset: 0x00000AE1
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000028E9 File Offset: 0x00000AE9
		public float TransitionDuration { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000028F2 File Offset: 0x00000AF2
		// (set) Token: 0x06000047 RID: 71 RVA: 0x000028FA File Offset: 0x00000AFA
		public float DelayOnBegin { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002903 File Offset: 0x00000B03
		// (set) Token: 0x06000049 RID: 73 RVA: 0x0000290B File Offset: 0x00000B0B
		public bool EaseIn { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002914 File Offset: 0x00000B14
		// (set) Token: 0x0600004B RID: 75 RVA: 0x0000291C File Offset: 0x00000B1C
		public Dictionary<string, VisualStateTemplate> VisualStates { get; private set; }

		// Token: 0x0600004C RID: 76 RVA: 0x00002925 File Offset: 0x00000B25
		public VisualDefinitionTemplate()
		{
			this.VisualStates = new Dictionary<string, VisualStateTemplate>();
			this.TransitionDuration = 0.2f;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002943 File Offset: 0x00000B43
		public void AddVisualState(VisualStateTemplate visualState)
		{
			this.VisualStates.Add(visualState.State, visualState);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002958 File Offset: 0x00000B58
		public VisualDefinition CreateVisualDefinition(BrushFactory brushFactory, SpriteData spriteData, Dictionary<string, VisualDefinitionTemplate> visualDefinitionTemplates, Dictionary<string, ConstantDefinition> constants, Dictionary<string, WidgetAttributeTemplate> parameters, Dictionary<string, string> defaultParameters)
		{
			VisualDefinition visualDefinition = new VisualDefinition(this.Name, this.TransitionDuration, this.DelayOnBegin, this.EaseIn);
			foreach (VisualStateTemplate visualStateTemplate in this.VisualStates.Values)
			{
				VisualState visualState = visualStateTemplate.CreateVisualState(brushFactory, spriteData, visualDefinitionTemplates, constants, parameters, defaultParameters);
				visualDefinition.AddVisualState(visualState);
			}
			return visualDefinition;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000029E0 File Offset: 0x00000BE0
		internal void Save(XmlNode rootNode)
		{
			XmlDocument ownerDocument = rootNode.OwnerDocument;
			XmlNode xmlNode = ownerDocument.CreateElement("VisualDefinition");
			XmlAttribute xmlAttribute = ownerDocument.CreateAttribute("Name");
			xmlAttribute.InnerText = this.Name;
			xmlNode.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = ownerDocument.CreateAttribute("TransitionDuration");
			xmlAttribute2.InnerText = this.TransitionDuration.ToString();
			xmlNode.Attributes.Append(xmlAttribute2);
			XmlAttribute xmlAttribute3 = ownerDocument.CreateAttribute("DelayOnBegin");
			xmlAttribute3.InnerText = this.DelayOnBegin.ToString();
			xmlNode.Attributes.Append(xmlAttribute3);
			XmlAttribute xmlAttribute4 = ownerDocument.CreateAttribute("EaseIn");
			xmlAttribute4.InnerText = this.EaseIn.ToString();
			xmlNode.Attributes.Append(xmlAttribute4);
			foreach (VisualStateTemplate visualStateTemplate in this.VisualStates.Values)
			{
				XmlNode xmlNode2 = ownerDocument.CreateElement("VisualState");
				XmlAttribute xmlAttribute5 = ownerDocument.CreateAttribute("State");
				xmlAttribute5.InnerText = visualStateTemplate.State;
				xmlNode2.Attributes.Append(xmlAttribute5);
				foreach (KeyValuePair<string, string> keyValuePair in visualStateTemplate.GetAttributes())
				{
					XmlAttribute xmlAttribute6 = ownerDocument.CreateAttribute(keyValuePair.Key);
					xmlAttribute6.InnerText = keyValuePair.Value;
					xmlNode2.Attributes.Append(xmlAttribute6);
				}
				xmlNode.AppendChild(xmlNode2);
			}
			rootNode.AppendChild(xmlNode);
		}
	}
}
