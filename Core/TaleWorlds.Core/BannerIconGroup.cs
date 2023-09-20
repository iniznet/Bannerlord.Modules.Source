using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000013 RID: 19
	public class BannerIconGroup
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000043EF File Offset: 0x000025EF
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x000043F7 File Offset: 0x000025F7
		public TextObject Name { get; private set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004400 File Offset: 0x00002600
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00004408 File Offset: 0x00002608
		public bool IsPattern { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00004411 File Offset: 0x00002611
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00004419 File Offset: 0x00002619
		public int Id { get; private set; }

		// Token: 0x060000DE RID: 222 RVA: 0x00004422 File Offset: 0x00002622
		internal BannerIconGroup()
		{
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000442C File Offset: 0x0000262C
		public void Deserialize(XmlNode xmlNode, MBList<BannerIconGroup> previouslyAddedGroups)
		{
			this._allIcons = new Dictionary<int, BannerIconData>();
			this._availableIcons = new Dictionary<int, BannerIconData>();
			this._allBackgrounds = new Dictionary<int, string>();
			this.AllIcons = new MBReadOnlyDictionary<int, BannerIconData>(this._allIcons);
			this.AvailableIcons = new MBReadOnlyDictionary<int, BannerIconData>(this._availableIcons);
			this.AllBackgrounds = new MBReadOnlyDictionary<int, string>(this._allBackgrounds);
			this.Id = Convert.ToInt32(xmlNode.Attributes["id"].Value);
			this.Name = new TextObject(xmlNode.Attributes["name"].Value, null);
			this.IsPattern = Convert.ToBoolean(xmlNode.Attributes["is_pattern"].Value);
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (xmlNode2.Name == "Icon")
				{
					int id2 = Convert.ToInt32(xmlNode2.Attributes["id"].Value);
					string value = xmlNode2.Attributes["material_name"].Value;
					int num = int.Parse(xmlNode2.Attributes["texture_index"].Value);
					if (!this._allIcons.ContainsKey(id2) && !previouslyAddedGroups.Any((BannerIconGroup x) => x.AllIcons.ContainsKey(id2)))
					{
						this._allIcons.Add(id2, new BannerIconData(value, num));
						if (xmlNode2.Attributes["is_reserved"] == null || !Convert.ToBoolean(xmlNode2.Attributes["is_reserved"].Value))
						{
							this._availableIcons.Add(id2, new BannerIconData(value, num));
						}
					}
				}
				else if (xmlNode2.Name == "Background")
				{
					int id = Convert.ToInt32(xmlNode2.Attributes["id"].Value);
					string value2 = xmlNode2.Attributes["mesh_name"].Value;
					if (xmlNode2.Attributes["is_base_background"] != null && Convert.ToBoolean(xmlNode2.Attributes["is_base_background"].Value))
					{
						BannerManager.Instance.SetBaseBackgroundId(id);
					}
					if (!this._allBackgrounds.ContainsKey(id) && !previouslyAddedGroups.Any((BannerIconGroup x) => x.AllBackgrounds.ContainsKey(id)))
					{
						this._allBackgrounds.Add(id, value2);
					}
				}
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000471C File Offset: 0x0000291C
		public void Merge(BannerIconGroup otherGroup)
		{
			foreach (KeyValuePair<int, BannerIconData> keyValuePair in otherGroup._allIcons)
			{
				if (!this._allIcons.ContainsKey(keyValuePair.Key))
				{
					this._allIcons.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<int, string> keyValuePair2 in otherGroup._allBackgrounds)
			{
				if (!this._allBackgrounds.ContainsKey(keyValuePair2.Key))
				{
					this._allBackgrounds.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
			}
			foreach (KeyValuePair<int, BannerIconData> keyValuePair3 in otherGroup._availableIcons)
			{
				if (!this._availableIcons.ContainsKey(keyValuePair3.Key))
				{
					this._availableIcons.Add(keyValuePair3.Key, keyValuePair3.Value);
				}
			}
		}

		// Token: 0x040000F1 RID: 241
		public MBReadOnlyDictionary<int, BannerIconData> AllIcons;

		// Token: 0x040000F2 RID: 242
		public MBReadOnlyDictionary<int, string> AllBackgrounds;

		// Token: 0x040000F3 RID: 243
		public MBReadOnlyDictionary<int, BannerIconData> AvailableIcons;

		// Token: 0x040000F4 RID: 244
		private Dictionary<int, BannerIconData> _allIcons;

		// Token: 0x040000F5 RID: 245
		private Dictionary<int, string> _allBackgrounds;

		// Token: 0x040000F6 RID: 246
		private Dictionary<int, BannerIconData> _availableIcons;
	}
}
