using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	public class ModuleInfo
	{
		public bool IsSelected { get; set; }

		public string Id { get; private set; }

		public string Name { get; private set; }

		public bool IsOfficial
		{
			get
			{
				return this.Type > ModuleType.Community;
			}
		}

		public bool IsDefault { get; private set; }

		public bool IsRequiredOfficial
		{
			get
			{
				return this.Type == ModuleType.Official;
			}
		}

		public ApplicationVersion Version { get; private set; }

		public ModuleCategory Category { get; private set; }

		public string FolderPath { get; private set; }

		public ModuleType Type { get; private set; }

		public bool HasMultiplayerCategory
		{
			get
			{
				return this.Category == ModuleCategory.Multiplayer || this.Category == ModuleCategory.MultiplayerOptional;
			}
		}

		public bool IsNative
		{
			get
			{
				return this.Id.Equals("Native", StringComparison.OrdinalIgnoreCase);
			}
		}

		public ModuleInfo()
		{
			this.DependedModules = new List<DependedModule>();
			this.SubModules = new List<SubModuleInfo>();
			this.ModulesToLoadAfterThis = new List<DependedModule>();
			this.IncompatibleModules = new List<DependedModule>();
		}

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

		public void UpdateVersionChangeSet()
		{
			this.Version = new ApplicationVersion(this.Version.ApplicationVersionType, this.Version.Major, this.Version.Minor, this.Version.Revision, 17949);
		}

		private const int ModuleDefaultChangeSet = 0;

		public readonly List<SubModuleInfo> SubModules;

		public readonly List<DependedModule> DependedModules;

		public readonly List<DependedModule> ModulesToLoadAfterThis;

		public readonly List<DependedModule> IncompatibleModules;
	}
}
