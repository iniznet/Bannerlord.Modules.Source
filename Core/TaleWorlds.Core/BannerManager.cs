using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public class BannerManager
	{
		public static BannerManager Instance { get; private set; }

		public MBReadOnlyList<BannerIconGroup> BannerIconGroups
		{
			get
			{
				return this._bannerIconGroups;
			}
		}

		public int BaseBackgroundId { get; private set; }

		private BannerManager()
		{
			this._bannerIconGroups = new MBList<BannerIconGroup>();
			this._colorPalette = new Dictionary<int, BannerColor>();
			this.ReadOnlyColorPalette = this._colorPalette.GetReadOnlyDictionary<int, BannerColor>();
		}

		public static void Initialize()
		{
			if (BannerManager.Instance == null)
			{
				BannerManager.Instance = new BannerManager();
			}
		}

		public static MBReadOnlyDictionary<int, BannerColor> ColorPalette
		{
			get
			{
				return BannerManager.Instance.ReadOnlyColorPalette;
			}
		}

		public static uint GetColor(int id)
		{
			if (BannerManager.ColorPalette.ContainsKey(id))
			{
				return BannerManager.ColorPalette[id].Color;
			}
			return 3735928559U;
		}

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

		public void SetBaseBackgroundId(int id)
		{
			this.BaseBackgroundId = id;
		}

		public void LoadBannerIcons(string xmlPath)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(xmlPath);
			this.LoadFromXml(xmlDocument);
		}

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

		public const int DarkRed = 1;

		public const int Green = 120;

		public const int Blue = 119;

		public const int Purple = 4;

		public const int DarkPurple = 6;

		public const int Orange = 9;

		public const int DarkBlue = 12;

		public const int Red = 118;

		public const int Yellow = 121;

		public MBReadOnlyDictionary<int, BannerColor> ReadOnlyColorPalette;

		private Dictionary<int, BannerColor> _colorPalette;

		private MBList<BannerIconGroup> _bannerIconGroups;

		private int _availablePatternCount;

		private int _availableIconCount;
	}
}
