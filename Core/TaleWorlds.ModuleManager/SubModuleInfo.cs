using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	public class SubModuleInfo
	{
		public string Name { get; private set; }

		public string DLLName { get; private set; }

		public string DLLPath { get; private set; }

		public bool IsTWCertifiedDLL { get; private set; }

		public bool DLLExists { get; private set; }

		public List<string> Assemblies { get; private set; }

		public string SubModuleClassType { get; private set; }

		public SubModuleInfo()
		{
			this.Tags = new List<Tuple<SubModuleInfo.SubModuleTags, string>>();
		}

		public void LoadFrom(XmlNode subModuleNode, string path, bool isOfficial)
		{
			this.Tags.Clear();
			this.Name = subModuleNode.SelectSingleNode("Name").Attributes["value"].InnerText;
			this.DLLName = subModuleNode.SelectSingleNode("DLLName").Attributes["value"].InnerText;
			string text = this.DLLName;
			text = Path.Combine(path, "bin\\Win64_Shipping_Client", this.DLLName);
			if (!string.IsNullOrEmpty(this.DLLName))
			{
				this.DLLExists = File.Exists(text);
				this.DLLPath = text;
				if (!this.DLLExists)
				{
					Debug.Print("Couldn't find .dll: " + this.DLLPath, 0, Debug.DebugColor.White, 17592186044416UL);
				}
				this.IsTWCertifiedDLL = this.DLLExists && this.GetIsTWCertified(text, isOfficial);
			}
			this.SubModuleClassType = subModuleNode.SelectSingleNode("SubModuleClassType").Attributes["value"].InnerText;
			this.Assemblies = new List<string>();
			if (subModuleNode.SelectSingleNode("Assemblies") != null)
			{
				XmlNodeList xmlNodeList = subModuleNode.SelectSingleNode("Assemblies").SelectNodes("Assembly");
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					this.Assemblies.Add(xmlNodeList[i].Attributes["value"].InnerText);
				}
			}
			XmlNodeList xmlNodeList2 = subModuleNode.SelectSingleNode("Tags").SelectNodes("Tag");
			for (int j = 0; j < xmlNodeList2.Count; j++)
			{
				SubModuleInfo.SubModuleTags subModuleTags;
				if (Enum.TryParse<SubModuleInfo.SubModuleTags>(xmlNodeList2[j].Attributes["key"].InnerText, out subModuleTags))
				{
					string innerText = xmlNodeList2[j].Attributes["value"].InnerText;
					this.Tags.Add(new Tuple<SubModuleInfo.SubModuleTags, string>(subModuleTags, innerText));
					if (subModuleTags == SubModuleInfo.SubModuleTags.DedicatedServerType && innerText != "none")
					{
						this.IsTWCertifiedDLL = true;
					}
				}
			}
		}

		private bool GetIsTWCertified(string fileName, bool isOfficial)
		{
			bool flag;
			try
			{
				X509Certificate2 x509Certificate = new X509Certificate2(fileName);
				X509Chain x509Chain = X509Chain.Create();
				x509Chain.Build(x509Certificate);
				foreach (X509ChainElement x509ChainElement in x509Chain.ChainElements)
				{
					if (x509ChainElement.Certificate.GetCertHashString() == "29B0C803942C9D4221EF0CFB1AB1FEE47683DF7D" && x509ChainElement.Certificate.GetSerialNumberString() == "61EB518586D5D0884531D7FBC0316B69")
					{
						return true;
					}
				}
				flag = false;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private const string CertHashString = "29B0C803942C9D4221EF0CFB1AB1FEE47683DF7D";

		private const string CertSerialNum = "61EB518586D5D0884531D7FBC0316B69";

		public readonly List<Tuple<SubModuleInfo.SubModuleTags, string>> Tags;

		public enum SubModuleTags
		{
			RejectedPlatform,
			ExclusivePlatform,
			DedicatedServerType,
			IsNoRenderModeElement,
			DependantRuntimeLibrary,
			PlayerHostedDedicatedServer,
			EngineType
		}
	}
}
