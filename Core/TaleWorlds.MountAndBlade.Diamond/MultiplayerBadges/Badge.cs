using System;
using System.Globalization;
using System.Xml;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	public class Badge
	{
		public int Index { get; }

		public BadgeType Type { get; }

		public string StringId { get; private set; }

		public string GroupId { get; private set; }

		public TextObject Name { get; private set; }

		public TextObject Description { get; private set; }

		public bool IsVisibleOnlyWhenEarned { get; private set; }

		public DateTime PeriodStart { get; private set; }

		public DateTime PeriodEnd { get; private set; }

		public bool IsActive
		{
			get
			{
				return DateTime.UtcNow >= this.PeriodStart && DateTime.UtcNow <= this.PeriodEnd;
			}
		}

		public bool IsTimed
		{
			get
			{
				return this.PeriodStart > DateTime.MinValue || this.PeriodEnd < DateTime.MaxValue;
			}
		}

		public Badge(int index, BadgeType badgeType)
		{
			this.Index = index;
			this.Type = badgeType;
		}

		public virtual void Deserialize(XmlNode node)
		{
			this.StringId = node.Attributes["id"].Value;
			XmlAttributeCollection attributes = node.Attributes;
			string text;
			if (attributes == null)
			{
				text = null;
			}
			else
			{
				XmlAttribute xmlAttribute = attributes["group_id"];
				text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
			}
			string text2 = text;
			this.GroupId = (string.IsNullOrWhiteSpace(text2) ? null : text2);
			string value = node.Attributes["name"].Value;
			string value2 = node.Attributes["description"].Value;
			XmlAttribute xmlAttribute2 = node.Attributes["is_visible_only_when_earned"];
			this.IsVisibleOnlyWhenEarned = Convert.ToBoolean((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
			XmlAttribute xmlAttribute3 = node.Attributes["period_start"];
			DateTime dateTime;
			this.PeriodStart = (DateTime.TryParse((xmlAttribute3 != null) ? xmlAttribute3.Value : null, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime) ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : DateTime.MinValue);
			XmlAttribute xmlAttribute4 = node.Attributes["period_end"];
			DateTime dateTime2;
			this.PeriodEnd = (DateTime.TryParse((xmlAttribute4 != null) ? xmlAttribute4.Value : null, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime2) ? DateTime.SpecifyKind(dateTime2, DateTimeKind.Utc) : DateTime.MaxValue);
			this.Name = new TextObject(value, null);
			this.Description = new TextObject(value2, null);
		}
	}
}
