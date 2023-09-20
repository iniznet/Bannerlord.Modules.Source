using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.Core
{
	// Token: 0x02000015 RID: 21
	public class BannerManager
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000489A File Offset: 0x00002A9A
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x000048A1 File Offset: 0x00002AA1
		public static BannerManager Instance { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x000048A9 File Offset: 0x00002AA9
		public MBReadOnlyList<BannerIconGroup> BannerIconGroups
		{
			get
			{
				return this._bannerIconGroups;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x000048B1 File Offset: 0x00002AB1
		// (set) Token: 0x060000EA RID: 234 RVA: 0x000048B9 File Offset: 0x00002AB9
		public int BaseBackgroundId { get; private set; }

		// Token: 0x060000EB RID: 235 RVA: 0x000048C2 File Offset: 0x00002AC2
		private BannerManager()
		{
			this._bannerIconGroups = new MBList<BannerIconGroup>();
			this._colorPalette = new Dictionary<int, BannerColor>();
			this.ReadOnlyColorPalette = this._colorPalette.GetReadOnlyDictionary<int, BannerColor>();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000048F1 File Offset: 0x00002AF1
		public static void Initialize()
		{
			if (BannerManager.Instance == null)
			{
				BannerManager.Instance = new BannerManager();
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00004904 File Offset: 0x00002B04
		public static MBReadOnlyDictionary<int, BannerColor> ColorPalette
		{
			get
			{
				return BannerManager.Instance.ReadOnlyColorPalette;
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00004910 File Offset: 0x00002B10
		public static uint GetColor(int id)
		{
			if (BannerManager.ColorPalette.ContainsKey(id))
			{
				return BannerManager.ColorPalette[id].Color;
			}
			return 3735928559U;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00004944 File Offset: 0x00002B44
		public static int GetColorId(uint color)
		{
			for (int i = 0; i < BannerManager.ColorPalette.Count; i++)
			{
				if (BannerManager.ColorPalette[i].Color == color)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00004980 File Offset: 0x00002B80
		public BannerIconData GetIconDataFromIconId(int id)
		{
			using (List<BannerIconGroup>.Enumerator enumerator = this._bannerIconGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BannerIconData bannerIconData;
					if (enumerator.Current.AllIcons.TryGetValue(id, out bannerIconData))
					{
						return bannerIconData;
					}
				}
			}
			return default(BannerIconData);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000049EC File Offset: 0x00002BEC
		public int GetRandomBackgroundId(MBFastRandom random)
		{
			int num = random.Next(0, this._availablePatternCount);
			foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
			{
				if (bannerIconGroup.IsPattern)
				{
					if (num < bannerIconGroup.AllBackgrounds.Count)
					{
						return bannerIconGroup.AllBackgrounds.ElementAt(num).Key;
					}
					num -= bannerIconGroup.AllBackgrounds.Count;
				}
			}
			return -1;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00004A88 File Offset: 0x00002C88
		public int GetRandomBannerIconId(MBFastRandom random)
		{
			int num = random.Next(0, this._availableIconCount);
			foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
			{
				if (!bannerIconGroup.IsPattern)
				{
					if (num < bannerIconGroup.AvailableIcons.Count)
					{
						return bannerIconGroup.AvailableIcons.ElementAt(num).Key;
					}
					num -= bannerIconGroup.AvailableIcons.Count;
				}
			}
			return -1;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00004B24 File Offset: 0x00002D24
		public string GetBackgroundMeshName(int id)
		{
			foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
			{
				if (bannerIconGroup.IsPattern && bannerIconGroup.AllBackgrounds.ContainsKey(id))
				{
					return bannerIconGroup.AllBackgrounds[id];
				}
			}
			return null;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00004B98 File Offset: 0x00002D98
		public string GetIconSourceTextureName(int id)
		{
			foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
			{
				if (!bannerIconGroup.IsPattern && bannerIconGroup.AllBackgrounds.ContainsKey(id))
				{
					return bannerIconGroup.AllBackgrounds[id];
				}
			}
			return null;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00004C0C File Offset: 0x00002E0C
		public void SetBaseBackgroundId(int id)
		{
			this.BaseBackgroundId = id;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00004C18 File Offset: 0x00002E18
		public void LoadBannerIcons(string xmlPath)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/banner_icons.xml");
			this.LoadFromXml(xmlDocument);
			foreach (ModuleInfo moduleInfo in ModuleHelper.GetModules())
			{
				if (!moduleInfo.IsNative)
				{
					string text = moduleInfo.FolderPath + "/ModuleData/banner_icons.xml";
					if (File.Exists(text))
					{
						xmlDocument = this.LoadXmlFile(text);
						this.LoadFromXml(xmlDocument);
					}
				}
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00004CB0 File Offset: 0x00002EB0
		private XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00004CFC File Offset: 0x00002EFC
		private void LoadFromXml(XmlDocument doc)
		{
			Debug.Print("loading banner_icons.xml:", 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (doc.ChildNodes[1].Name != "base")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			XmlNode xmlNode = doc.ChildNodes[1].ChildNodes[0];
			if (xmlNode.Name != "BannerIconData")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (xmlNode.Name == "BannerIconData")
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.Name == "BannerIconGroup")
					{
						BannerIconGroup bannerIconGroup = new BannerIconGroup();
						bannerIconGroup.Deserialize(xmlNode2, this._bannerIconGroups);
						BannerIconGroup bannerIconGroup3 = this._bannerIconGroups.FirstOrDefault((BannerIconGroup x) => x.Id == bannerIconGroup.Id);
						if (bannerIconGroup3 == null)
						{
							this._bannerIconGroups.Add(bannerIconGroup);
						}
						else
						{
							bannerIconGroup3.Merge(bannerIconGroup);
						}
					}
					if (xmlNode2.Name == "BannerColors")
					{
						foreach (object obj2 in xmlNode2.ChildNodes)
						{
							XmlNode xmlNode3 = (XmlNode)obj2;
							if (xmlNode3.Name == "Color")
							{
								int num = Convert.ToInt32(xmlNode3.Attributes["id"].Value);
								if (!this._colorPalette.ContainsKey(num))
								{
									uint num2 = Convert.ToUInt32(xmlNode3.Attributes["hex"].Value, 16);
									XmlAttribute xmlAttribute = xmlNode3.Attributes["player_can_choose_for_sigil"];
									bool flag = Convert.ToBoolean(((xmlAttribute != null) ? xmlAttribute.Value : null) ?? "false");
									XmlAttribute xmlAttribute2 = xmlNode3.Attributes["player_can_choose_for_background"];
									bool flag2 = Convert.ToBoolean(((xmlAttribute2 != null) ? xmlAttribute2.Value : null) ?? "false");
									this._colorPalette.Add(num, new BannerColor(num2, flag, flag2));
								}
							}
						}
					}
				}
			}
			this._availablePatternCount = 0;
			this._availableIconCount = 0;
			foreach (BannerIconGroup bannerIconGroup2 in this._bannerIconGroups)
			{
				if (bannerIconGroup2.IsPattern)
				{
					this._availablePatternCount += bannerIconGroup2.AllBackgrounds.Count;
				}
				else
				{
					this._availableIconCount += bannerIconGroup2.AvailableIcons.Count;
				}
			}
		}

		// Token: 0x040000FB RID: 251
		public const int DarkRed = 1;

		// Token: 0x040000FC RID: 252
		public const int Green = 120;

		// Token: 0x040000FD RID: 253
		public const int Blue = 119;

		// Token: 0x040000FE RID: 254
		public const int Purple = 4;

		// Token: 0x040000FF RID: 255
		public const int DarkPurple = 6;

		// Token: 0x04000100 RID: 256
		public const int Orange = 9;

		// Token: 0x04000101 RID: 257
		public const int DarkBlue = 12;

		// Token: 0x04000102 RID: 258
		public const int Red = 118;

		// Token: 0x04000103 RID: 259
		public const int Yellow = 121;

		// Token: 0x04000105 RID: 261
		public MBReadOnlyDictionary<int, BannerColor> ReadOnlyColorPalette;

		// Token: 0x04000106 RID: 262
		private Dictionary<int, BannerColor> _colorPalette;

		// Token: 0x04000107 RID: 263
		private MBList<BannerIconGroup> _bannerIconGroups;

		// Token: 0x04000109 RID: 265
		private int _availablePatternCount;

		// Token: 0x0400010A RID: 266
		private int _availableIconCount;
	}
}
