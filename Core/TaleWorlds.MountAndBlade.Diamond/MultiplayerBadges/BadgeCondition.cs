using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x0200015B RID: 347
	public class BadgeCondition
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060008A2 RID: 2210 RVA: 0x0000E886 File Offset: 0x0000CA86
		// (set) Token: 0x060008A3 RID: 2211 RVA: 0x0000E88E File Offset: 0x0000CA8E
		public ConditionType Type { get; private set; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060008A4 RID: 2212 RVA: 0x0000E897 File Offset: 0x0000CA97
		// (set) Token: 0x060008A5 RID: 2213 RVA: 0x0000E89F File Offset: 0x0000CA9F
		public ConditionGroupType GroupType { get; private set; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060008A6 RID: 2214 RVA: 0x0000E8A8 File Offset: 0x0000CAA8
		// (set) Token: 0x060008A7 RID: 2215 RVA: 0x0000E8B0 File Offset: 0x0000CAB0
		public TextObject Description { get; private set; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x0000E8B9 File Offset: 0x0000CAB9
		// (set) Token: 0x060008A9 RID: 2217 RVA: 0x0000E8C1 File Offset: 0x0000CAC1
		public string StringId { get; private set; }

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x0000E8CA File Offset: 0x0000CACA
		// (set) Token: 0x060008AB RID: 2219 RVA: 0x0000E8D2 File Offset: 0x0000CAD2
		public IReadOnlyDictionary<string, string> Parameters { get; private set; }

		// Token: 0x060008AC RID: 2220 RVA: 0x0000E8DC File Offset: 0x0000CADC
		public BadgeCondition(int index, XmlNode node)
		{
			XmlAttributeCollection attributes = node.Attributes;
			ConditionType conditionType;
			if (!Enum.TryParse<ConditionType>((attributes != null) ? attributes["type"].Value : null, true, out conditionType))
			{
				conditionType = ConditionType.Custom;
				Debug.FailedAssert("No 'type' was provided for a condition", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\MultiplayerBadges\\BadgeCondition.cs", ".ctor", 47);
			}
			this.Type = conditionType;
			ConditionGroupType conditionGroupType = ConditionGroupType.Any;
			XmlAttributeCollection attributes2 = node.Attributes;
			bool flag;
			if (attributes2 == null)
			{
				flag = null != null;
			}
			else
			{
				XmlAttribute xmlAttribute = attributes2["group_type"];
				flag = ((xmlAttribute != null) ? xmlAttribute.Value : null) != null;
			}
			if (flag && !Enum.TryParse<ConditionGroupType>(node.Attributes["group_type"].Value, true, out conditionGroupType))
			{
				Debug.FailedAssert("Provided 'group_type' was wrong for a condition", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\MultiplayerBadges\\BadgeCondition.cs", ".ctor", 54);
			}
			this.GroupType = conditionGroupType;
			XmlAttributeCollection attributes3 = node.Attributes;
			this.Description = new TextObject((attributes3 != null) ? attributes3["description"].Value : null, null);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Parameter")
				{
					string text = xmlNode.Attributes["name"].Value.Trim();
					string text2 = xmlNode.Attributes["value"].Value.Trim();
					dictionary[text] = text2;
				}
			}
			this.Parameters = dictionary;
			XmlAttributeCollection attributes4 = node.Attributes;
			string text3;
			if (attributes4 == null)
			{
				text3 = null;
			}
			else
			{
				XmlAttribute xmlAttribute2 = attributes4["id"];
				text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
			}
			this.StringId = text3;
			string text4;
			if (this.StringId == null && this.Parameters.TryGetValue("property", out text4))
			{
				this.StringId = text4 + ((this.GroupType == ConditionGroupType.Party) ? ".Party" : ((this.GroupType == ConditionGroupType.Solo) ? ".Solo" : ""));
			}
			if (this.StringId == null)
			{
				this.StringId = "condition." + index;
			}
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0000EB04 File Offset: 0x0000CD04
		public bool Check(string value)
		{
			ConditionType type = this.Type;
			if (type != ConditionType.PlayerData)
			{
				return false;
			}
			string text;
			if (!this.Parameters.TryGetValue("value", out text))
			{
				Debug.FailedAssert("Given condition doesn't have a value parameter", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\MultiplayerBadges\\BadgeCondition.cs", "Check", 94);
				return false;
			}
			return value == text;
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0000EB54 File Offset: 0x0000CD54
		public bool Check(int value)
		{
			ConditionType type = this.Type;
			if (type - ConditionType.PlayerDataNumeric > 2)
			{
				return false;
			}
			string text;
			if (this.Parameters.TryGetValue("value", out text))
			{
				int num;
				if (!int.TryParse(text, out num))
				{
					Debug.FailedAssert("Given condition value parameter is not valid number", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\MultiplayerBadges\\BadgeCondition.cs", "Check", 115);
					return false;
				}
				return value == num;
			}
			else
			{
				string text2;
				bool flag = this.Parameters.TryGetValue("min_value", out text2);
				string text3;
				bool flag2 = this.Parameters.TryGetValue("max_value", out text3);
				int minValue = int.MinValue;
				int maxValue = int.MaxValue;
				if (flag && !int.TryParse(text2, out minValue))
				{
					Debug.FailedAssert("Given condition min_value parameter is not valid number", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\MultiplayerBadges\\BadgeCondition.cs", "Check", 129);
					return false;
				}
				if (flag2 && !int.TryParse(text3, out maxValue))
				{
					Debug.FailedAssert("Given condition max_value parameter is not valid number", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\MultiplayerBadges\\BadgeCondition.cs", "Check", 134);
					return false;
				}
				return (flag || flag2) && value >= minValue && value <= maxValue;
			}
		}
	}
}
