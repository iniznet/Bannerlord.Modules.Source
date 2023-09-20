using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	// Token: 0x02000006 RID: 6
	public class ModuleInfo
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002621 File Offset: 0x00000821
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002629 File Offset: 0x00000829
		public bool IsSelected { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002632 File Offset: 0x00000832
		// (set) Token: 0x06000024 RID: 36 RVA: 0x0000263A File Offset: 0x0000083A
		public string Id { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002643 File Offset: 0x00000843
		// (set) Token: 0x06000026 RID: 38 RVA: 0x0000264B File Offset: 0x0000084B
		public string Name { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002654 File Offset: 0x00000854
		public bool IsOfficial
		{
			get
			{
				return this.Type > ModuleType.Community;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000265F File Offset: 0x0000085F
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002667 File Offset: 0x00000867
		public bool IsDefault { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002670 File Offset: 0x00000870
		public bool IsRequiredOfficial
		{
			get
			{
				return this.Type == ModuleType.Official;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000267B File Offset: 0x0000087B
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002683 File Offset: 0x00000883
		public ApplicationVersion Version { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000268C File Offset: 0x0000088C
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002694 File Offset: 0x00000894
		public ModuleCategory Category { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x0000269D File Offset: 0x0000089D
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000026A5 File Offset: 0x000008A5
		public string FolderPath { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000031 RID: 49 RVA: 0x000026AE File Offset: 0x000008AE
		// (set) Token: 0x06000032 RID: 50 RVA: 0x000026B6 File Offset: 0x000008B6
		public ModuleType Type { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000026BF File Offset: 0x000008BF
		public bool HasMultiplayerCategory
		{
			get
			{
				return this.Category == ModuleCategory.Multiplayer || this.Category == ModuleCategory.MultiplayerOptional;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000026D5 File Offset: 0x000008D5
		public bool IsNative
		{
			get
			{
				return this.Id.Equals("Native", StringComparison.OrdinalIgnoreCase);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000026E8 File Offset: 0x000008E8
		public ModuleInfo()
		{
			this.DependedModules = new List<DependedModule>();
			this.SubModules = new List<SubModuleInfo>();
			this.ModulesToLoadAfterThis = new List<DependedModule>();
			this.IncompatibleModules = new List<DependedModule>();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000271C File Offset: 0x0000091C
		public void LoadWithFullPath(string fullPath)
		{
			this.SubModules.Clear();
			this.DependedModules.Clear();
			this.ModulesToLoadAfterThis.Clear();
			this.IncompatibleModules.Clear();
			this.FolderPath = fullPath;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(this.FolderPath + "/SubModule.xml");
			XmlNode xmlNode = xmlDocument.SelectSingleNode("Module");
			this.Name = xmlNode.SelectSingleNode("Name").Attributes["value"].InnerText;
			this.Id = xmlNode.SelectSingleNode("Id").Attributes["value"].InnerText;
			this.Version = ApplicationVersion.FromString(xmlNode.SelectSingleNode("Version").Attributes["value"].InnerText, 0);
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("DefaultModule");
			this.IsDefault = xmlNode2 != null && xmlNode2.Attributes["value"].InnerText.Equals("true");
			XmlNode xmlNode3 = xmlNode.SelectSingleNode("ModuleType");
			ModuleType moduleType;
			if (xmlNode3 != null && Enum.TryParse<ModuleType>(xmlNode3.Attributes["value"].InnerText, out moduleType))
			{
				this.Type = moduleType;
			}
			this.IsSelected = this.IsNative;
			this.Category = ModuleCategory.Singleplayer;
			XmlNode xmlNode4 = xmlNode.SelectSingleNode("ModuleCategory");
			ModuleCategory moduleCategory;
			if (xmlNode4 != null && Enum.TryParse<ModuleCategory>(xmlNode4.Attributes["value"].InnerText, out moduleCategory))
			{
				this.Category = moduleCategory;
			}
			XmlNode xmlNode5 = xmlNode.SelectSingleNode("DependedModules");
			XmlNodeList xmlNodeList = ((xmlNode5 != null) ? xmlNode5.SelectNodes("DependedModule") : null);
			if (xmlNodeList != null)
			{
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					string innerText = xmlNodeList[i].Attributes["Id"].InnerText;
					ApplicationVersion applicationVersion = ApplicationVersion.Empty;
					bool flag = false;
					if (xmlNodeList[i].Attributes["DependentVersion"] != null)
					{
						try
						{
							applicationVersion = ApplicationVersion.FromString(xmlNodeList[i].Attributes["DependentVersion"].InnerText, 0);
						}
						catch
						{
							string.Concat(new string[] { "Couldn't parse dependent version of ", innerText, " for ", this.Id, ". Using default version." });
						}
					}
					XmlAttribute xmlAttribute = xmlNodeList[i].Attributes["Optional"];
					bool flag2;
					if (bool.TryParse((xmlAttribute != null) ? xmlAttribute.InnerText : null, out flag2))
					{
						flag = flag2;
					}
					this.DependedModules.Add(new DependedModule(innerText, applicationVersion, flag));
				}
			}
			XmlNode xmlNode6 = xmlNode.SelectSingleNode("ModulesToLoadAfterThis");
			XmlNodeList xmlNodeList2 = ((xmlNode6 != null) ? xmlNode6.SelectNodes("Module") : null);
			if (xmlNodeList2 != null)
			{
				for (int j = 0; j < xmlNodeList2.Count; j++)
				{
					string innerText2 = xmlNodeList2[j].Attributes["Id"].InnerText;
					this.ModulesToLoadAfterThis.Add(new DependedModule(innerText2, ApplicationVersion.Empty, false));
				}
			}
			XmlNode xmlNode7 = xmlNode.SelectSingleNode("IncompatibleModules");
			XmlNodeList xmlNodeList3 = ((xmlNode7 != null) ? xmlNode7.SelectNodes("Module") : null);
			if (xmlNodeList3 != null)
			{
				for (int k = 0; k < xmlNodeList3.Count; k++)
				{
					string innerText3 = xmlNodeList3[k].Attributes["Id"].InnerText;
					this.IncompatibleModules.Add(new DependedModule(innerText3, ApplicationVersion.Empty, false));
				}
			}
			XmlNode xmlNode8 = xmlNode.SelectSingleNode("SubModules");
			XmlNodeList xmlNodeList4 = ((xmlNode8 != null) ? xmlNode8.SelectNodes("SubModule") : null);
			if (xmlNodeList4 != null)
			{
				for (int l = 0; l < xmlNodeList4.Count; l++)
				{
					SubModuleInfo subModuleInfo = new SubModuleInfo();
					try
					{
						subModuleInfo.LoadFrom(xmlNodeList4[l], this.FolderPath, this.IsOfficial);
					}
					catch
					{
						string.Format("Cannot load a submodule {0} under {1}", l, this.FolderPath);
					}
					this.SubModules.Add(subModuleInfo);
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002B50 File Offset: 0x00000D50
		public void UpdateVersionChangeSet()
		{
			this.Version = new ApplicationVersion(this.Version.ApplicationVersionType, this.Version.Major, this.Version.Minor, this.Version.Revision, 17949);
		}

		// Token: 0x0400000B RID: 11
		private const int ModuleDefaultChangeSet = 0;

		// Token: 0x04000014 RID: 20
		public readonly List<SubModuleInfo> SubModules;

		// Token: 0x04000015 RID: 21
		public readonly List<DependedModule> DependedModules;

		// Token: 0x04000016 RID: 22
		public readonly List<DependedModule> ModulesToLoadAfterThis;

		// Token: 0x04000017 RID: 23
		public readonly List<DependedModule> IncompatibleModules;
	}
}
