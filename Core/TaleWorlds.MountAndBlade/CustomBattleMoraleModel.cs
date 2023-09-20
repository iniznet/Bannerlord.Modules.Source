using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MBHelpers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001E0 RID: 480
	public class CustomBattleMoraleModel : BattleMoraleModel
	{
		// Token: 0x06001B3C RID: 6972 RVA: 0x0005FE2C File Offset: 0x0005E02C
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow)
		{
			float battleImportance = affectedAgent.GetBattleImportance();
			Team team = affectedAgent.Team;
			BattleSideEnum battleSideEnum = ((team != null) ? team.Side : BattleSideEnum.None);
			float num = this.CalculateCasualtiesFactor(battleSideEnum);
			SkillObject relevantSkillFromWeaponClass = WeaponComponentData.GetRelevantSkillFromWeaponClass((WeaponClass)killingBlow.WeaponClass);
			bool flag = relevantSkillFromWeaponClass == DefaultSkills.Bow || relevantSkillFromWeaponClass == DefaultSkills.Crossbow || relevantSkillFromWeaponClass == DefaultSkills.Throwing;
			bool flag2 = killingBlow.WeaponRecordWeaponFlags.HasAnyFlag(WeaponFlags.AffectsArea | WeaponFlags.AffectsAreaBig | WeaponFlags.MultiplePenetration);
			float num2 = 0.75f;
			if (flag2)
			{
				num2 = 0.25f;
				if (killingBlow.WeaponRecordWeaponFlags.HasAllFlags(WeaponFlags.Burning | WeaponFlags.MultiplePenetration))
				{
					num2 += num2 * 0.25f;
				}
			}
			else if (flag)
			{
				num2 = 0.5f;
			}
			num2 = Math.Max(0f, num2);
			FactoredNumber factoredNumber = new FactoredNumber(battleImportance * 3f * num2);
			FactoredNumber factoredNumber2 = new FactoredNumber(battleImportance * 4f * num2 * num);
			Formation formation = affectedAgent.Formation;
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(formation);
			if (activeBanner != null)
			{
				BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedMoraleShock, activeBanner, ref factoredNumber2);
			}
			return new ValueTuple<float, float>(MathF.Max(factoredNumber2.ResultNumber, 0f), MathF.Max(factoredNumber.ResultNumber, 0f));
		}

		// Token: 0x06001B3D RID: 6973 RVA: 0x0005FF5C File Offset: 0x0005E15C
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent)
		{
			float battleImportance = agent.GetBattleImportance();
			Team team = agent.Team;
			BattleSideEnum battleSideEnum = ((team != null) ? team.Side : BattleSideEnum.None);
			float num = this.CalculateCasualtiesFactor(battleSideEnum);
			float num2 = battleImportance * 2f;
			float num3 = battleImportance * num * 1.1f;
			if (agent.Character != null)
			{
				FactoredNumber factoredNumber = new FactoredNumber(num3);
				Formation formation = agent.Formation;
				BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(formation);
				if (activeBanner != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedMoraleShock, activeBanner, ref factoredNumber);
				}
				num3 = factoredNumber.ResultNumber;
			}
			return new ValueTuple<float, float>(MathF.Max(num3, 0f), MathF.Max(num2, 0f));
		}

		// Token: 0x06001B3E RID: 6974 RVA: 0x0005FFFB File Offset: 0x0005E1FB
		public override float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange)
		{
			return maxMoraleChange / MathF.Max(1f, agent.Character.GetMoraleResistance());
		}

		// Token: 0x06001B3F RID: 6975 RVA: 0x00060014 File Offset: 0x0005E214
		public override float GetEffectiveInitialMorale(Agent agent, float baseMorale)
		{
			return baseMorale;
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x00060017 File Offset: 0x0005E217
		public override bool CanPanicDueToMorale(Agent agent)
		{
			return true;
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x0006001C File Offset: 0x0005E21C
		public override float CalculateCasualtiesFactor(BattleSideEnum battleSide)
		{
			float num = 1f;
			if (Mission.Current != null && battleSide != BattleSideEnum.None)
			{
				float removedAgentRatioForSide = Mission.Current.GetRemovedAgentRatioForSide(battleSide);
				num += removedAgentRatioForSide * 2f;
				num = MathF.Max(0f, num);
			}
			return num;
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x00060060 File Offset: 0x0005E260
		public override float GetAverageMorale(Formation formation)
		{
			float num = 0f;
			int num2 = 0;
			if (formation != null)
			{
				using (List<IFormationUnit>.Enumerator enumerator = formation.Arrangement.GetAllUnits().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent;
						if ((agent = enumerator.Current as Agent) != null && agent.IsActive() && agent.IsHuman && agent.IsAIControlled)
						{
							num2++;
							num += agent.GetMorale();
						}
					}
				}
			}
			if (num2 > 0)
			{
				return MBMath.ClampFloat(num / (float)num2, 0f, 100f);
			}
			return 0f;
		}
	}
}
