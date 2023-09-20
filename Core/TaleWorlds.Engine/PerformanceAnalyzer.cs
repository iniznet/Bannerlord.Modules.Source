using System;
using System.Collections.Generic;
using System.Xml;

namespace TaleWorlds.Engine
{
	// Token: 0x02000070 RID: 112
	public class PerformanceAnalyzer
	{
		// Token: 0x060008A0 RID: 2208 RVA: 0x00008A10 File Offset: 0x00006C10
		public void Start(string name)
		{
			PerformanceAnalyzer.PerformanceObject performanceObject = new PerformanceAnalyzer.PerformanceObject(name);
			this.currentObject = performanceObject;
			this.objects.Add(performanceObject);
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x00008A37 File Offset: 0x00006C37
		public void End()
		{
			this.currentObject = null;
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00008A40 File Offset: 0x00006C40
		public void FinalizeAndWrite(string filePath)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlNode xmlNode = xmlDocument.CreateElement("objects");
				xmlDocument.AppendChild(xmlNode);
				foreach (PerformanceAnalyzer.PerformanceObject performanceObject in this.objects)
				{
					XmlNode xmlNode2 = xmlDocument.CreateElement("object");
					performanceObject.Write(xmlNode2, xmlDocument);
					xmlNode.AppendChild(xmlNode2);
				}
				xmlDocument.Save(filePath);
			}
			catch (Exception ex)
			{
				MBDebug.ShowWarning("Exception occurred while trying to write " + filePath + ": " + ex.ToString());
			}
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00008AF8 File Offset: 0x00006CF8
		public void Tick(float dt)
		{
			if (this.currentObject != null)
			{
				this.currentObject.AddFps(Utilities.GetFps(), Utilities.GetMainFps(), Utilities.GetRendererFps());
			}
		}

		// Token: 0x04000148 RID: 328
		private List<PerformanceAnalyzer.PerformanceObject> objects = new List<PerformanceAnalyzer.PerformanceObject>();

		// Token: 0x04000149 RID: 329
		private PerformanceAnalyzer.PerformanceObject currentObject;

		// Token: 0x020000C0 RID: 192
		private class PerformanceObject
		{
			// Token: 0x170000A5 RID: 165
			// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0000F3AB File Offset: 0x0000D5AB
			private float AverageMainFps
			{
				get
				{
					if (this.frameCount > 0)
					{
						return this.totalMainFps / (float)this.frameCount;
					}
					return 0f;
				}
			}

			// Token: 0x170000A6 RID: 166
			// (get) Token: 0x06000C5B RID: 3163 RVA: 0x0000F3CA File Offset: 0x0000D5CA
			private float AverageRendererFps
			{
				get
				{
					if (this.frameCount > 0)
					{
						return this.totalRendererFps / (float)this.frameCount;
					}
					return 0f;
				}
			}

			// Token: 0x170000A7 RID: 167
			// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0000F3E9 File Offset: 0x0000D5E9
			private float AverageFps
			{
				get
				{
					if (this.frameCount > 0)
					{
						return this.totalFps / (float)this.frameCount;
					}
					return 0f;
				}
			}

			// Token: 0x06000C5D RID: 3165 RVA: 0x0000F408 File Offset: 0x0000D608
			public void AddFps(float fps, float main, float renderer)
			{
				this.frameCount++;
				this.totalFps += fps;
				this.totalMainFps += main;
				this.totalRendererFps += renderer;
			}

			// Token: 0x06000C5E RID: 3166 RVA: 0x0000F444 File Offset: 0x0000D644
			public void Write(XmlNode node, XmlDocument document)
			{
				XmlAttribute xmlAttribute = document.CreateAttribute("name");
				xmlAttribute.Value = this.name;
				node.Attributes.Append(xmlAttribute);
				XmlAttribute xmlAttribute2 = document.CreateAttribute("frameCount");
				xmlAttribute2.Value = this.frameCount.ToString();
				node.Attributes.Append(xmlAttribute2);
				XmlAttribute xmlAttribute3 = document.CreateAttribute("averageFps");
				xmlAttribute3.Value = this.AverageFps.ToString();
				node.Attributes.Append(xmlAttribute3);
				XmlAttribute xmlAttribute4 = document.CreateAttribute("averageMainFps");
				xmlAttribute4.Value = this.AverageMainFps.ToString();
				node.Attributes.Append(xmlAttribute4);
				XmlAttribute xmlAttribute5 = document.CreateAttribute("averageRendererFps");
				xmlAttribute5.Value = this.AverageRendererFps.ToString();
				node.Attributes.Append(xmlAttribute5);
			}

			// Token: 0x06000C5F RID: 3167 RVA: 0x0000F52D File Offset: 0x0000D72D
			public PerformanceObject(string objectName)
			{
				this.name = objectName;
			}

			// Token: 0x040003FA RID: 1018
			private string name;

			// Token: 0x040003FB RID: 1019
			private int frameCount;

			// Token: 0x040003FC RID: 1020
			private float totalMainFps;

			// Token: 0x040003FD RID: 1021
			private float totalRendererFps;

			// Token: 0x040003FE RID: 1022
			private float totalFps;
		}
	}
}
