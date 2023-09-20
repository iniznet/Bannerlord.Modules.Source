using System;
using System.Globalization;
using System.Xml;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x02000157 RID: 343
	public class Badge
	{
		// Token: 0x17000301 RID: 769
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x0000E651 File Offset: 0x0000C851
		public int Index { get; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x0600088F RID: 2191 RVA: 0x0000E659 File Offset: 0x0000C859
		public BadgeType Type { get; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000890 RID: 2192 RVA: 0x0000E661 File Offset: 0x0000C861
		// (set) Token: 0x06000891 RID: 2193 RVA: 0x0000E669 File Offset: 0x0000C869
		public string StringId { get; private set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06000892 RID: 2194 RVA: 0x0000E672 File Offset: 0x0000C872
		// (set) Token: 0x06000893 RID: 2195 RVA: 0x0000E67A File Offset: 0x0000C87A
		public string GroupId { get; private set; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06000894 RID: 2196 RVA: 0x0000E683 File Offset: 0x0000C883
		// (set) Token: 0x06000895 RID: 2197 RVA: 0x0000E68B File Offset: 0x0000C88B
		public TextObject Name { get; private set; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06000896 RID: 2198 RVA: 0x0000E694 File Offset: 0x0000C894
		// (set) Token: 0x06000897 RID: 2199 RVA: 0x0000E69C File Offset: 0x0000C89C
		public TextObject Description { get; private set; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x0000E6A5 File Offset: 0x0000C8A5
		// (set) Token: 0x06000899 RID: 2201 RVA: 0x0000E6AD File Offset: 0x0000C8AD
		public bool IsVisibleOnlyWhenEarned { get; private set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x0600089A RID: 2202 RVA: 0x0000E6B6 File Offset: 0x0000C8B6
		// (set) Token: 0x0600089B RID: 2203 RVA: 0x0000E6BE File Offset: 0x0000C8BE
		public DateTime PeriodStart { get; private set; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x0600089C RID: 2204 RVA: 0x0000E6C7 File Offset: 0x0000C8C7
		// (set) Token: 0x0600089D RID: 2205 RVA: 0x0000E6CF File Offset: 0x0000C8CF
		public DateTime PeriodEnd { get; private set; }

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x0600089E RID: 2206 RVA: 0x0000E6D8 File Offset: 0x0000C8D8
		public bool IsActive
		{
			get
			{
				return DateTime.UtcNow >= this.PeriodStart && DateTime.UtcNow <= this.PeriodEnd;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x0000E6FE File Offset: 0x0000C8FE
		public bool IsTimed
		{
			get
			{
				return this.PeriodStart > DateTime.MinValue || this.PeriodEnd < DateTime.MaxValue;
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0000E724 File Offset: 0x0000C924
		public Badge(int index, BadgeType badgeType)
		{
			this.Index = index;
			this.Type = badgeType;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0000E73C File Offset: 0x0000C93C
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
