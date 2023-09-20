using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000455 RID: 1109
	public static class SetPartyAiAction
	{
		// Token: 0x06003F4D RID: 16205 RVA: 0x0012F570 File Offset: 0x0012D770
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

		// Token: 0x06003F4E RID: 16206 RVA: 0x0012F7CE File Offset: 0x0012D9CE
		public static void GetAction(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.GoToSettlement);
		}

		// Token: 0x06003F4F RID: 16207 RVA: 0x0012F7D8 File Offset: 0x0012D9D8
		public static void GetActionForVisitingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.GoToSettlement);
		}

		// Token: 0x06003F50 RID: 16208 RVA: 0x0012F7E2 File Offset: 0x0012D9E2
		public static void GetActionForPatrollingAroundSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.PatrolAroundSettlement);
		}

		// Token: 0x06003F51 RID: 16209 RVA: 0x0012F7EC File Offset: 0x0012D9EC
		public static void GetActionForRaidingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.RaidSettlement);
		}

		// Token: 0x06003F52 RID: 16210 RVA: 0x0012F7F6 File Offset: 0x0012D9F6
		public static void GetActionForBesiegingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.BesiegeSettlement);
		}

		// Token: 0x06003F53 RID: 16211 RVA: 0x0012F800 File Offset: 0x0012DA00
		public static void GetActionForEngagingParty(MobileParty owner, MobileParty mobileParty)
		{
			SetPartyAiAction.ApplyInternal(owner, mobileParty, SetPartyAiAction.SetPartyAiActionDetail.EngageParty);
		}

		// Token: 0x06003F54 RID: 16212 RVA: 0x0012F80A File Offset: 0x0012DA0A
		public static void GetActionForGoingAroundParty(MobileParty owner, MobileParty mobileParty)
		{
			SetPartyAiAction.ApplyInternal(owner, mobileParty, SetPartyAiAction.SetPartyAiActionDetail.GoAroundParty);
		}

		// Token: 0x06003F55 RID: 16213 RVA: 0x0012F814 File Offset: 0x0012DA14
		public static void GetActionForDefendingSettlement(MobileParty owner, Settlement settlement)
		{
			SetPartyAiAction.ApplyInternal(owner, settlement, SetPartyAiAction.SetPartyAiActionDetail.DefendParty);
		}

		// Token: 0x06003F56 RID: 16214 RVA: 0x0012F81E File Offset: 0x0012DA1E
		public static void GetActionForEscortingParty(MobileParty owner, MobileParty mobileParty)
		{
			SetPartyAiAction.ApplyInternal(owner, mobileParty, SetPartyAiAction.SetPartyAiActionDetail.EscortParty);
		}

		// Token: 0x0200076A RID: 1898
		private enum SetPartyAiActionDetail
		{
			// Token: 0x04001E78 RID: 7800
			GoToSettlement,
			// Token: 0x04001E79 RID: 7801
			PatrolAroundSettlement,
			// Token: 0x04001E7A RID: 7802
			RaidSettlement,
			// Token: 0x04001E7B RID: 7803
			BesiegeSettlement,
			// Token: 0x04001E7C RID: 7804
			EngageParty,
			// Token: 0x04001E7D RID: 7805
			GoAroundParty,
			// Token: 0x04001E7E RID: 7806
			DefendParty,
			// Token: 0x04001E7F RID: 7807
			EscortParty
		}
	}
}
