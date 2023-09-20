using System;
using System.Collections.Generic;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	public class ConditionalBadge : Badge
	{
		public IReadOnlyList<BadgeCondition> BadgeConditions { get; private set; }

		public ConditionalBadge(int index, BadgeType badgeType)
			: base(index, badgeType)
		{
		}

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
