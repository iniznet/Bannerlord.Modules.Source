using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public class BannerIconGroup
	{
		public TextObject Name { get; private set; }

		public bool IsPattern { get; private set; }

		public int Id { get; private set; }

		internal BannerIconGroup()
		{
		}

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

		public MBReadOnlyDictionary<int, BannerIconData> AllIcons;

		public MBReadOnlyDictionary<int, string> AllBackgrounds;

		public MBReadOnlyDictionary<int, BannerIconData> AvailableIcons;

		private Dictionary<int, BannerIconData> _allIcons;

		private Dictionary<int, string> _allBackgrounds;

		private Dictionary<int, BannerIconData> _availableIcons;
	}
}
