using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000088 RID: 136
	public struct AIBehaviorTuple : IEquatable<AIBehaviorTuple>
	{
		// Token: 0x0600106F RID: 4207 RVA: 0x0004A6B9 File Offset: 0x000488B9
		public AIBehaviorTuple(IMapPoint party, AiBehavior aiBehavior, bool willGatherArmy = false)
		{
			this.Party = party;
			this.AiBehavior = aiBehavior;
			this.WillGatherArmy = willGatherArmy;
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x0004A6D0 File Offset: 0x000488D0
		public override bool Equals(object obj)
		{
			if (!(obj is AIBehaviorTuple))
			{
				return false;
			}
			AIBehaviorTuple aibehaviorTuple = (AIBehaviorTuple)obj;
			return aibehaviorTuple.Party == this.Party && aibehaviorTuple.AiBehavior == this.AiBehavior && aibehaviorTuple.WillGatherArmy == this.WillGatherArmy;
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0004A71A File Offset: 0x0004891A
		public bool Equals(AIBehaviorTuple other)
		{
			return other.Party == this.Party && other.AiBehavior == this.AiBehavior && other.WillGatherArmy == this.WillGatherArmy;
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x0004A748 File Offset: 0x00048948
		public override int GetHashCode()
		{
			int aiBehavior = (int)this.AiBehavior;
			int num = aiBehavior.GetHashCode();
			num = ((this.Party != null) ? ((num * 397) ^ this.Party.GetHashCode()) : num);
			return (num * 397) ^ this.WillGatherArmy.GetHashCode();
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x0004A798 File Offset: 0x00048998
		public static bool operator ==(AIBehaviorTuple a, AIBehaviorTuple b)
		{
			return a.Party == b.Party && a.AiBehavior == b.AiBehavior && a.WillGatherArmy == b.WillGatherArmy;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x0004A7C6 File Offset: 0x000489C6
		public static bool operator !=(AIBehaviorTuple a, AIBehaviorTuple b)
		{
			return !(a == b);
		}

		// Token: 0x040005DC RID: 1500
		public IMapPoint Party;

		// Token: 0x040005DD RID: 1501
		public AiBehavior AiBehavior;

		// Token: 0x040005DE RID: 1502
		public bool WillGatherArmy;
	}
}
