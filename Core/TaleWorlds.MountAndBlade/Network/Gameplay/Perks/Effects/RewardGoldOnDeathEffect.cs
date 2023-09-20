using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003CE RID: 974
	public class RewardGoldOnDeathEffect : MPPerkEffect
	{
		// Token: 0x060033ED RID: 13293 RVA: 0x000D73CC File Offset: 0x000D55CC
		protected RewardGoldOnDeathEffect()
		{
		}

		// Token: 0x060033EE RID: 13294 RVA: 0x000D73D4 File Offset: 0x000D55D4
		protected override void Deserialize(XmlNode node)
		{
			string text;
			if (node == null)
			{
				text = null;
			}
			else
			{
				XmlAttributeCollection attributes = node.Attributes;
				if (attributes == null)
				{
					text = null;
				}
				else
				{
					XmlAttribute xmlAttribute = attributes["is_disabled_in_warmup"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			base.IsDisabledInWarmup = ((text2 != null) ? text2.ToLower() : null) == "true";
			string text3;
			if (node == null)
			{
				text3 = null;
			}
			else
			{
				XmlAttributeCollection attributes2 = node.Attributes;
				if (attributes2 == null)
				{
					text3 = null;
				}
				else
				{
					XmlAttribute xmlAttribute2 = attributes2["value"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null || !int.TryParse(text4, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\RewardGoldOnDeathEffect.cs", "Deserialize", 41);
			}
			string text5;
			if (node == null)
			{
				text5 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text5 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["number_of_receivers"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			if (text6 == null || !int.TryParse(text6, out this._count))
			{
				Debug.FailedAssert("provided 'number_of_receivers' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\RewardGoldOnDeathEffect.cs", "Deserialize", 47);
			}
			string text7;
			if (node == null)
			{
				text7 = null;
			}
			else
			{
				XmlAttributeCollection attributes4 = node.Attributes;
				if (attributes4 == null)
				{
					text7 = null;
				}
				else
				{
					XmlAttribute xmlAttribute4 = attributes4["order_by"];
					text7 = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
				}
			}
			string text8 = text7;
			this._orderBy = RewardGoldOnDeathEffect.OrderBy.Random;
			if (text8 != null && !Enum.TryParse<RewardGoldOnDeathEffect.OrderBy>(text8, true, out this._orderBy))
			{
				this._orderBy = RewardGoldOnDeathEffect.OrderBy.Random;
				Debug.FailedAssert("provided 'order_by' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\RewardGoldOnDeathEffect.cs", "Deserialize", 55);
			}
		}

		// Token: 0x060033EF RID: 13295 RVA: 0x000D7529 File Offset: 0x000D5729
		public override bool GetIsTeamRewardedOnDeath()
		{
			return true;
		}

		// Token: 0x060033F0 RID: 13296 RVA: 0x000D752C File Offset: 0x000D572C
		public override void CalculateRewardedGoldOnDeath(Agent agent, List<ValueTuple<MissionPeer, int>> teamMembers)
		{
			Agent agent2 = agent;
			if (((agent2 != null) ? agent2.MissionPeer : null) == null)
			{
				Agent agent3 = agent;
				if (agent3 != null)
				{
					MissionPeer owningAgentMissionPeer = agent3.OwningAgentMissionPeer;
				}
			}
			switch (this._orderBy)
			{
			case RewardGoldOnDeathEffect.OrderBy.WealthAscending:
				teamMembers.Sort((ValueTuple<MissionPeer, int> a, ValueTuple<MissionPeer, int> b) => a.Item1.Representative.Gold.CompareTo(b.Item1.Representative.Gold));
				break;
			case RewardGoldOnDeathEffect.OrderBy.WealthDescending:
				teamMembers.Sort((ValueTuple<MissionPeer, int> a, ValueTuple<MissionPeer, int> b) => b.Item1.Representative.Gold.CompareTo(a.Item1.Representative.Gold));
				break;
			case RewardGoldOnDeathEffect.OrderBy.DistanceAscending:
				teamMembers.Sort((ValueTuple<MissionPeer, int> a, ValueTuple<MissionPeer, int> b) => this.SortByDistance(agent, a.Item1.Representative.ControlledAgent, b.Item1.Representative.ControlledAgent));
				break;
			case RewardGoldOnDeathEffect.OrderBy.DistanceDescending:
				teamMembers.Sort((ValueTuple<MissionPeer, int> a, ValueTuple<MissionPeer, int> b) => this.SortByDistance(agent, b.Item1.Representative.ControlledAgent, a.Item1.Representative.ControlledAgent));
				break;
			}
			int num = this._count;
			int num2 = 0;
			while (num2 < teamMembers.Count && num > 0)
			{
				bool flag;
				if (this._orderBy >= RewardGoldOnDeathEffect.OrderBy.DistanceAscending)
				{
					MissionRepresentativeBase representative = teamMembers[num2].Item1.Representative;
					if (representative == null)
					{
						flag = false;
					}
					else
					{
						Agent controlledAgent = representative.ControlledAgent;
						bool? flag2 = ((controlledAgent != null) ? new bool?(controlledAgent.IsActive()) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					ValueTuple<MissionPeer, int> valueTuple = teamMembers[num2];
					valueTuple.Item2 += this._value;
					teamMembers[num2] = valueTuple;
					num--;
				}
				num2++;
			}
		}

		// Token: 0x060033F1 RID: 13297 RVA: 0x000D76B0 File Offset: 0x000D58B0
		private int SortByDistance(Agent from, Agent a, Agent b)
		{
			if (a == null || !a.IsActive())
			{
				if (b != null && b.IsActive())
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (b == null || !b.IsActive())
				{
					return -1;
				}
				return a.Position.DistanceSquared(from.Position).CompareTo(b.Position.DistanceSquared(from.Position));
			}
		}

		// Token: 0x04001619 RID: 5657
		protected static string StringType = "RewardGoldOnDeath";

		// Token: 0x0400161A RID: 5658
		private int _value;

		// Token: 0x0400161B RID: 5659
		private int _count;

		// Token: 0x0400161C RID: 5660
		private RewardGoldOnDeathEffect.OrderBy _orderBy;

		// Token: 0x020006C2 RID: 1730
		private enum OrderBy
		{
			// Token: 0x0400229A RID: 8858
			Random,
			// Token: 0x0400229B RID: 8859
			WealthAscending,
			// Token: 0x0400229C RID: 8860
			WealthDescending,
			// Token: 0x0400229D RID: 8861
			DistanceAscending,
			// Token: 0x0400229E RID: 8862
			DistanceDescending,
			// Token: 0x0400229F RID: 8863
			DeadCanReceiveEnd = 3
		}
	}
}
