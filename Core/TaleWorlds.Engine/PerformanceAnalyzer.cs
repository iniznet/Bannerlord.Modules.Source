using System;
using System.Collections.Generic;
using System.Xml;

namespace TaleWorlds.Engine
{
	public class PerformanceAnalyzer
	{
		public void Start(string name)
		{
			PerformanceAnalyzer.PerformanceObject performanceObject = new PerformanceAnalyzer.PerformanceObject(name);
			this.currentObject = performanceObject;
			this.objects.Add(performanceObject);
		}

		public void End()
		{
			this.currentObject = null;
		}

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

		public void Tick(float dt)
		{
			if (this.currentObject != null)
			{
				this.currentObject.AddFps(Utilities.GetFps(), Utilities.GetMainFps(), Utilities.GetRendererFps());
			}
		}

		private List<PerformanceAnalyzer.PerformanceObject> objects = new List<PerformanceAnalyzer.PerformanceObject>();

		private PerformanceAnalyzer.PerformanceObject currentObject;

		private class PerformanceObject
		{
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

			public void AddFps(float fps, float main, float renderer)
			{
				this.frameCount++;
				this.totalFps += fps;
				this.totalMainFps += main;
				this.totalRendererFps += renderer;
			}

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

			public PerformanceObject(string objectName)
			{
				this.name = objectName;
			}

			private string name;

			private int frameCount;

			private float totalMainFps;

			private float totalRendererFps;

			private float totalFps;
		}
	}
}
