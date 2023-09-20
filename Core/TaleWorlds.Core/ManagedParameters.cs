using System;
using System.IO;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public sealed class ManagedParameters : IManagedParametersInitializer
	{
		public static ManagedParameters Instance { get; } = new ManagedParameters();

		public static float GetParameter(ManagedParametersEnum managedParameterType)
		{
			return ManagedParameters.Instance._managedParametersArray[(int)managedParameterType];
		}

		public static void SetParameter(ManagedParametersEnum managedParameterType, float newValue)
		{
			ManagedParameters.Instance._managedParametersArray[(int)managedParameterType] = newValue;
		}

		public void Initialize(string relativeXmlPath)
		{
			XmlDocument xmlDocument = ManagedParameters.LoadXmlFile(relativeXmlPath);
			this.LoadFromXml(xmlDocument);
		}

		private ManagedParameters()
		{
		}

		private static XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		private void LoadFromXml(XmlNode doc)
		{
			Debug.Print("loading managed_core_parameters.xml", 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (doc.ChildNodes[1].Name != "base")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (doc.ChildNodes[1].ChildNodes[0].Name != "managed_core_parameters")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			XmlNode xmlNode = null;
			if (doc.ChildNodes[1].ChildNodes[0].Name == "managed_core_parameters")
			{
				xmlNode = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0];
			}
			while (xmlNode != null)
			{
				ManagedParametersEnum managedParametersEnum;
				if (xmlNode.Name == "managed_core_parameter" && xmlNode.NodeType != XmlNodeType.Comment && Enum.TryParse<ManagedParametersEnum>(xmlNode.Attributes["id"].Value, true, out managedParametersEnum))
				{
					this._managedParametersArray[(int)managedParametersEnum] = float.Parse(xmlNode.Attributes["value"].Value);
				}
				xmlNode = xmlNode.NextSibling;
			}
		}

		public float GetManagedParameter(ManagedParametersEnum managedParameterEnum)
		{
			return this._managedParametersArray[(int)managedParameterEnum];
		}

		private readonly float[] _managedParametersArray = new float[68];
	}
}
