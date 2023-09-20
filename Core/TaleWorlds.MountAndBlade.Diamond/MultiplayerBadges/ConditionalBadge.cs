using System;
using System.Collections.Generic;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x0200015E RID: 350
	public class ConditionalBadge : Badge
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x0000F475 File Offset: 0x0000D675
		// (set) Token: 0x060008BF RID: 2239 RVA: 0x0000F47D File Offset: 0x0000D67D
		public IReadOnlyList<BadgeCondition> BadgeConditions { get; private set; }

		// Token: 0x060008C0 RID: 2240 RVA: 0x0000F486 File Offset: 0x0000D686
		public ConditionalBadge(int index, BadgeType badgeType)
			: base(index, badgeType)
		{
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0000F490 File Offset: 0x0000D690
		public override void Deserialize(XmlNode node)
		{
			base.Deserialize(node);
			List<BadgeCondition> list = new List<BadgeCondition>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Condition")
				{
					BadgeCondition badgeCondition = new BadgeCondition(list.Count, xmlNode);
					list.Add(badgeCondition);
				}
			}
			this.BadgeConditions = list;
		}
	}
}
