using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem
{
	public struct AIBehaviorTuple : IEquatable<AIBehaviorTuple>
	{
		public AIBehaviorTuple(IMapPoint party, AiBehavior aiBehavior, bool willGatherArmy = false)
		{
			this.Party = party;
			this.AiBehavior = aiBehavior;
			this.WillGatherArmy = willGatherArmy;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is AIBehaviorTuple))
			{
				return false;
			}
			AIBehaviorTuple aibehaviorTuple = (AIBehaviorTuple)obj;
			return aibehaviorTuple.Party == this.Party && aibehaviorTuple.AiBehavior == this.AiBehavior && aibehaviorTuple.WillGatherArmy == this.WillGatherArmy;
		}

		public bool Equals(AIBehaviorTuple other)
		{
			return other.Party == this.Party && other.AiBehavior == this.AiBehavior && other.WillGatherArmy == this.WillGatherArmy;
		}

		public override int GetHashCode()
		{
			int aiBehavior = (int)this.AiBehavior;
			int num = aiBehavior.GetHashCode();
			num = ((this.Party != null) ? ((num * 397) ^ this.Party.GetHashCode()) : num);
			return (num * 397) ^ this.WillGatherArmy.GetHashCode();
		}

		public static bool operator ==(AIBehaviorTuple a, AIBehaviorTuple b)
		{
			return a.Party == b.Party && a.AiBehavior == b.AiBehavior && a.WillGatherArmy == b.WillGatherArmy;
		}

		public static bool operator !=(AIBehaviorTuple a, AIBehaviorTuple b)
		{
			return !(a == b);
		}

		public IMapPoint Party;

		public AiBehavior AiBehavior;

		public bool WillGatherArmy;
	}
}
