using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	// Token: 0x02000008 RID: 8
	public class SubModuleInfo
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002BA5 File Offset: 0x00000DA5
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002BAD File Offset: 0x00000DAD
		public string Name { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002BB6 File Offset: 0x00000DB6
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002BBE File Offset: 0x00000DBE
		public string DLLName { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002BC7 File Offset: 0x00000DC7
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002BCF File Offset: 0x00000DCF
		public string DLLPath { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002BD8 File Offset: 0x00000DD8
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002BE0 File Offset: 0x00000DE0
		public bool IsTWCertifiedDLL { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002BE9 File Offset: 0x00000DE9
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002BF1 File Offset: 0x00000DF1
		public bool DLLExists { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002BFA File Offset: 0x00000DFA
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002C02 File Offset: 0x00000E02
		public List<string> Assemblies { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002C0B File Offset: 0x00000E0B
		// (set) Token: 0x06000045 RID: 69 RVA: 0x00002C13 File Offset: 0x00000E13
		public string SubModuleClassType { get; private set; }

		// Token: 0x06000046 RID: 70 RVA: 0x00002C1C File Offset: 0x00000E1C
		public SubModuleInfo()
		{
			this.Tags = new List<Tuple<SubModuleInfo.SubModuleTags, string>>();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002C30 File Offset: 0x00000E30
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

		// Token: 0x06000048 RID: 72 RVA: 0x00002E38 File Offset: 0x00001038
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

		// Token: 0x0400001C RID: 28
		private const string CertHashString = "29B0C803942C9D4221EF0CFB1AB1FEE47683DF7D";

		// Token: 0x0400001D RID: 29
		private const string CertSerialNum = "61EB518586D5D0884531D7FBC0316B69";

		// Token: 0x04000025 RID: 37
		public readonly List<Tuple<SubModuleInfo.SubModuleTags, string>> Tags;

		// Token: 0x02000010 RID: 16
		public enum SubModuleTags
		{
			// Token: 0x04000042 RID: 66
			RejectedPlatform,
			// Token: 0x04000043 RID: 67
			ExclusivePlatform,
			// Token: 0x04000044 RID: 68
			DedicatedServerType,
			// Token: 0x04000045 RID: 69
			IsNoRenderModeElement,
			// Token: 0x04000046 RID: 70
			DependantRuntimeLibrary,
			// Token: 0x04000047 RID: 71
			PlayerHostedDedicatedServer,
			// Token: 0x04000048 RID: 72
			EngineType
		}
	}
}
