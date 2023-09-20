using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class SetPartyAiAction
	{
		private static void ApplyInternal(MobileParty owner, IMapPoint subject, SetPartyAiAction.SetPartyAiActionDetail detail)
		{
			if (detail == SetPartyAiAction.SetPartyAiActionDetail.GoToSettlement)
			{
				if (owner.DefaultBehavior != AiBehavior.GoToSettlement || owner.TargetSettlement != subject)
				{
					owner.Ai.SetMoveGoToSettlement((Settlement)subject);
				}
				if (owner.Army != null)
				{
					owner.Army.ArmyType = Army.ArmyTypes.Patrolling;
					owner.Army.AIBehavior = Army.AIBehaviorFlags.GoToSettlement;
					owner.Army.AiBehaviorObject = subject;
					return;
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.PatrolAroundSettlement)
			{
				if (owner.DefaultBehavior != AiBehavior.PatrolAroundPoint || owner.TargetSettlement != subject)
				{
					owner.Ai.SetMovePatrolAroundSettlement((Settlement)subject);
				}
				if (owner.Army != null)
				{
					owner.Army.ArmyType = Army.ArmyTypes.Patrolling;
					owner.Army.AIBehavior = Army.AIBehaviorFlags.Patrolling;
					owner.Army.AiBehaviorObject = subject;
					return;
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.RaidSettlement)
			{
				if (owner.DefaultBehavior != AiBehavior.RaidSettlement || owner.TargetSettlement != subject)
				{
					owner.Ai.SetMoveRaidSettlement((Settlement)subject);
					if (owner.Army != null)
					{
						owner.Army.AIBehavior = Army.AIBehaviorFlags.TravellingToAssignment;
						owner.Army.ArmyType = Army.ArmyTypes.Raider;
						owner.Army.AiBehaviorObject = subject;
						return;
					}
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.BesiegeSettlement)
			{
				if (owner.DefaultBehavior != AiBehavior.BesiegeSettlement || owner.TargetSettlement != subject)
				{
					owner.Ai.SetMoveBesiegeSettlement((Settlement)subject);
					if (owner.Army != null)
					{
						owner.Army.AIBehavior = Army.AIBehaviorFlags.TravellingToAssignment;
						owner.Army.ArmyType = Army.ArmyTypes.Besieger;
						owner.Army.AiBehaviorObject = subject;
						return;
					}
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.GoAroundParty)
			{
				if (owner.DefaultBehavior != AiBehavior.GoAroundParty || owner != subject)
				{
					owner.Ai.SetMoveGoAroundParty((MobileParty)subject);
					return;
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.EngageParty)
			{
				if (owner.DefaultBehavior != AiBehavior.EngageParty || owner != subject)
				{
					owner.Ai.SetMoveEngageParty((MobileParty)subject);
					return;
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.DefendParty)
			{
				if (owner.DefaultBehavior != AiBehavior.DefendSettlement || owner != subject)
				{
					owner.Ai.SetMoveDefendSettlement((Settlement)subject);
					if (owner.Army != null)
					{
						owner.Army.AIBehavior = Army.AIBehaviorFlags.Defending;
						owner.Army.ArmyType = Army.ArmyTypes.Defender;
						owner.Army.AiBehaviorObject = subject;
						return;
					}
				}
			}
			else if (detail == SetPartyAiAction.SetPartyAiActionDetail.EscortParty && (owner.DefaultBehavior != AiBehavior.EscortParty || owner.TargetParty != subject))
			{
				MobileParty mobileParty = (MobileParty)subject;
				owner.Ai.SetMoveEscortParty(mobileParty);
				if (owner != MobileParty.MainParty && owner.Army == null && mobileParty.Army != null)
				{
					owner.Army = mobileParty.Army;
				}
			}
		}

		public static void GetAction(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.GoToSettlement);
		}

		public static void GetActionForVisitingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.GoToSettlement);
		}

		public static void GetActionForPatrollingAroundSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.PatrolAroundSettlement);
		}

		public static void GetActionForRaidingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.RaidSettlement);
		}

		public static void GetActionForBesiegingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.BesiegeSettlement);
		}

		public static void GetActionForEngagingParty(MobileParty owner, MobileParty mobileParty)
		{
			SetPartyAiAction.ApplyInternal(owner, mobileParty, SetPartyAiAction.SetPartyAiActionDetail.EngageParty);
		}

		public static void GetActionForGoingAroundParty(MobileParty owner, MobileParty mobileParty)
		{
			SetPartyAiAction.ApplyInternal(owner, mobileParty, SetPartyAiAction.SetPartyAiActionDetail.GoAroundParty);
		}

		public static void GetActionForDefendingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.DefendParty);
		}

		public static void GetActionForEscortingParty(MobileParty owner, MobileParty mobileParty)
		{
			SetPartyAiAction.ApplyInternal(owner, mobileParty, SetPartyAiAction.SetPartyAiActionDetail.EscortParty);
		}

		private enum SetPartyAiActionDetail
		{
			GoToSettlement,
			PatrolAroundSettlement,
			RaidSettlement,
			BesiegeSettlement,
			EngageParty,
			GoAroundParty,
			DefendParty,
			EscortParty
		}
	}
}
