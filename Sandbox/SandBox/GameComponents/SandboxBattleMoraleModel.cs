using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents
{
	public class SandboxBattleMoraleModel : BattleMoraleModel
	{
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow)
		{
			float battleImportance = affectedAgent.GetBattleImportance();
			Team team = affectedAgent.Team;
			BattleSideEnum battleSideEnum = ((team != null) ? team.Side : (-1));
			float num = this.CalculateCasualtiesFactor(battleSideEnum);
			CharacterObject characterObject = ((affectorAgent != null) ? affectorAgent.Character : null) as CharacterObject;
			bool flag = ((affectedAgent != null) ? affectedAgent.Character : null) is CharacterObject;
			SkillObject relevantSkillFromWeaponClass = WeaponComponentData.GetRelevantSkillFromWeaponClass(killingBlow.WeaponClass);
			bool flag2 = relevantSkillFromWeaponClass == DefaultSkills.OneHanded || relevantSkillFromWeaponClass == DefaultSkills.TwoHanded || relevantSkillFromWeaponClass == DefaultSkills.Polearm;
			bool flag3 = relevantSkillFromWeaponClass == DefaultSkills.Bow || relevantSkillFromWeaponClass == DefaultSkills.Crossbow || relevantSkillFromWeaponClass == DefaultSkills.Throwing;
			bool flag4 = Extensions.HasAnyFlag<WeaponFlags>(killingBlow.WeaponRecordWeaponFlags, 1073766400L);
			float num2 = 0.75f;
			if (flag4)
			{
				num2 = 0.25f;
				if (Extensions.HasAllFlags<WeaponFlags>(killingBlow.WeaponRecordWeaponFlags, 1073774592L))
				{
					num2 += num2 * 0.25f;
				}
			}
			else if (flag3)
			{
				num2 = 0.5f;
			}
			num2 = Math.Max(0f, num2);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(battleImportance * 3f * num2, false, null);
			ExplainedNumber explainedNumber2;
			explainedNumber2..ctor(battleImportance * 4f * num2 * num, false, null);
			if (characterObject != null)
			{
				object obj;
				if (affectorAgent == null)
				{
					obj = null;
				}
				else
				{
					Formation formation = affectorAgent.Formation;
					if (formation == null)
					{
						obj = null;
					}
					else
					{
						Agent captain = formation.Captain;
						obj = ((captain != null) ? captain.Character : null);
					}
				}
				CharacterObject characterObject2 = obj as CharacterObject;
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Leadership.MakeADifference, characterObject, true, ref explainedNumber);
				if (flag2)
				{
					if (relevantSkillFromWeaponClass == DefaultSkills.TwoHanded)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.Hope, characterObject, true, ref explainedNumber);
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.Terror, characterObject, true, ref explainedNumber2);
					}
					if (affectorAgent != null && affectorAgent.HasMount)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.ThunderousCharge, characterObject, true, ref explainedNumber2);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.ThunderousCharge, characterObject2, ref explainedNumber2);
					}
				}
				else if (flag3)
				{
					if (relevantSkillFromWeaponClass == DefaultSkills.Crossbow)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.Terror, characterObject2, ref explainedNumber2);
					}
					if (affectorAgent != null && affectorAgent.HasMount)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.AnnoyingBuzz, characterObject, true, ref explainedNumber2);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.AnnoyingBuzz, characterObject2, ref explainedNumber2);
					}
				}
				PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Leadership.HeroicLeader, characterObject2, ref explainedNumber2);
			}
			if (flag)
			{
				object obj2;
				if (affectedAgent == null)
				{
					obj2 = null;
				}
				else
				{
					IAgentOriginBase origin = affectedAgent.Origin;
					obj2 = ((origin != null) ? origin.BattleCombatant : null);
				}
				PartyBase partyBase = obj2 as PartyBase;
				MobileParty mobileParty = ((partyBase != null) ? partyBase.MobileParty : null);
				if (affectedAgentState == 3 && mobileParty != null && mobileParty.HasPerk(DefaultPerks.Medicine.HealthAdvise, true))
				{
					explainedNumber2 = default(ExplainedNumber);
				}
				else
				{
					Formation formation2 = affectedAgent.Formation;
					object obj3;
					if (formation2 == null)
					{
						obj3 = null;
					}
					else
					{
						Agent captain2 = formation2.Captain;
						obj3 = ((captain2 != null) ? captain2.Character : null);
					}
					CharacterObject characterObject3;
					if ((characterObject3 = obj3 as CharacterObject) != null)
					{
						ArrangementOrder arrangementOrder = affectedAgent.Formation.ArrangementOrder;
						if (arrangementOrder == ArrangementOrder.ArrangementOrderShieldWall || arrangementOrder == ArrangementOrder.ArrangementOrderSquare || arrangementOrder == ArrangementOrder.ArrangementOrderSkein || arrangementOrder == ArrangementOrder.ArrangementOrderColumn)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.TightFormations, characterObject3, ref explainedNumber2);
						}
						if (arrangementOrder == ArrangementOrder.ArrangementOrderLine || arrangementOrder == ArrangementOrder.ArrangementOrderLoose || arrangementOrder == ArrangementOrder.ArrangementOrderCircle || arrangementOrder == ArrangementOrder.ArrangementOrderScatter)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.LooseFormations, characterObject3, ref explainedNumber2);
						}
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.StandardBearer, characterObject3, ref explainedNumber2);
					}
					Hero hero = ((mobileParty != null) ? mobileParty.EffectiveQuartermaster : null);
					if (hero != null)
					{
						PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Steward.PriceOfLoyalty, hero.CharacterObject, DefaultSkills.Steward, true, ref explainedNumber2, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
					}
				}
			}
			Formation formation3 = affectedAgent.Formation;
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(formation3);
			if (activeBanner != null)
			{
				BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedMoraleShock, activeBanner, ref explainedNumber2);
			}
			return new ValueTuple<float, float>(MathF.Max(explainedNumber2.ResultNumber, 0f), MathF.Max(explainedNumber.ResultNumber, 0f));
		}

		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent)
		{
			float battleImportance = agent.GetBattleImportance();
			Team team = agent.Team;
			BattleSideEnum battleSideEnum = ((team != null) ? team.Side : (-1));
			float num = this.CalculateCasualtiesFactor(battleSideEnum);
			float num2 = battleImportance * 2f;
			float num3 = battleImportance * num * 1.1f;
			if (((agent != null) ? agent.Character : null) is CharacterObject)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(num3, false, null);
				Formation formation = agent.Formation;
				object obj;
				if (formation == null)
				{
					obj = null;
				}
				else
				{
					Agent captain = formation.Captain;
					obj = ((captain != null) ? captain.Character : null);
				}
				CharacterObject characterObject = obj as CharacterObject;
				BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(formation);
				if (characterObject != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.StandardBearer, characterObject, ref explainedNumber);
				}
				object obj2;
				if (agent == null)
				{
					obj2 = null;
				}
				else
				{
					IAgentOriginBase origin = agent.Origin;
					obj2 = ((origin != null) ? origin.BattleCombatant : null);
				}
				PartyBase partyBase = obj2 as PartyBase;
				MobileParty mobileParty = ((partyBase != null) ? partyBase.MobileParty : null);
				Hero hero = ((mobileParty != null) ? mobileParty.EffectiveQuartermaster : null);
				if (hero != null)
				{
					PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Steward.PriceOfLoyalty, hero.CharacterObject, DefaultSkills.Steward, true, ref explainedNumber, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
				}
				if (activeBanner != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedMoraleShock, activeBanner, ref explainedNumber);
				}
				num3 = explainedNumber.ResultNumber;
			}
			return new ValueTuple<float, float>(MathF.Max(num3, 0f), MathF.Max(num2, 0f));
		}

		public override float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange)
		{
			return maxMoraleChange / MathF.Max(1f, agent.Character.GetMoraleResistance());
		}

		public override float GetEffectiveInitialMorale(Agent agent, float baseMorale)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(baseMorale, false, null);
			object obj;
			if (agent == null)
			{
				obj = null;
			}
			else
			{
				IAgentOriginBase origin = agent.Origin;
				obj = ((origin != null) ? origin.BattleCombatant : null);
			}
			PartyBase partyBase = (PartyBase)obj;
			MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
			CharacterObject characterObject = ((agent != null) ? agent.Character : null) as CharacterObject;
			if (mobileParty != null && characterObject != null)
			{
				Army army = mobileParty.Army;
				CharacterObject characterObject2;
				if (army == null)
				{
					characterObject2 = null;
				}
				else
				{
					MobileParty leaderParty = army.LeaderParty;
					if (leaderParty == null)
					{
						characterObject2 = null;
					}
					else
					{
						Hero leaderHero = leaderParty.LeaderHero;
						characterObject2 = ((leaderHero != null) ? leaderHero.CharacterObject : null);
					}
				}
				CharacterObject characterObject3 = characterObject2;
				Hero leaderHero2 = mobileParty.LeaderHero;
				CharacterObject characterObject4 = ((leaderHero2 != null) ? leaderHero2.CharacterObject : null);
				characterObject3 = ((characterObject3 != characterObject) ? characterObject3 : null);
				characterObject4 = ((characterObject4 != characterObject) ? characterObject4 : null);
				if (characterObject4 != null)
				{
					if (partyBase.Side == 1)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.FerventAttacker, mobileParty, true, ref explainedNumber);
					}
					else if (partyBase.Side == null)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.StoutDefender, mobileParty, true, ref explainedNumber);
					}
					if (characterObject4.Culture == characterObject.Culture)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.GreatLeader, mobileParty, false, ref explainedNumber);
					}
					if (characterObject4.GetPerkValue(DefaultPerks.Leadership.WePledgeOurSwords))
					{
						int num = MathF.Min(partyBase.GetNumberOfHealthyMenOfTier(6), 10);
						explainedNumber.Add((float)num, null, null);
					}
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Throwing.LastHit, mobileParty, false, ref explainedNumber);
					PartyBase partyBase2;
					if (partyBase == null)
					{
						partyBase2 = null;
					}
					else
					{
						MapEventSide mapEventSide = partyBase.MapEventSide;
						partyBase2 = ((mapEventSide != null) ? mapEventSide.LeaderParty : null);
					}
					PartyBase partyBase3 = partyBase2;
					if (partyBase3 != null && partyBase != partyBase3)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Riding.ReliefForce, mobileParty, true, ref explainedNumber);
					}
					if (partyBase.MapEvent != null)
					{
						float num2;
						float num3;
						partyBase.MapEvent.GetStrengthsRelativeToParty(partyBase.Side, ref num2, ref num3);
						if (num2 < num3)
						{
							PerkHelper.AddPerkBonusForParty(DefaultPerks.OneHanded.StandUnited, mobileParty, true, ref explainedNumber);
						}
						if (partyBase.MapEvent.IsSiegeAssault || partyBase.MapEvent.IsSiegeOutside)
						{
							PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.UpliftingSpirit, mobileParty, true, ref explainedNumber);
						}
						bool flag = false;
						foreach (PartyBase partyBase4 in partyBase.MapEvent.InvolvedParties)
						{
							if (partyBase4.Side != partyBase.Side && partyBase4.MapFaction != null && partyBase4.Culture.IsBandit)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.Patrols, mobileParty, true, ref explainedNumber);
						}
					}
					PerkHelper.AddPerkBonusForParty(DefaultPerks.OneHanded.LeadByExample, mobileParty, false, ref explainedNumber);
				}
				if (characterObject3 != null && characterObject3.GetPerkValue(DefaultPerks.Leadership.GreatLeader))
				{
					explainedNumber.Add(DefaultPerks.Leadership.GreatLeader.PrimaryBonus, null, null);
				}
				if (characterObject.IsRanged)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Bow.RenownedArcher, partyBase.MobileParty, true, ref explainedNumber);
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Crossbow.Marksmen, partyBase.MobileParty, false, ref explainedNumber);
				}
				if (mobileParty.IsDisorganized && (mobileParty.MapEvent == null || mobileParty.SiegeEvent == null || mobileParty.MapEventSide.MissionSide != 1) && (characterObject4 == null || !characterObject4.GetPerkValue(DefaultPerks.Tactics.Improviser)))
				{
					explainedNumber.AddFactor(-0.2f, null);
				}
			}
			return explainedNumber.ResultNumber;
		}

		public override bool CanPanicDueToMorale(Agent agent)
		{
			bool flag = true;
			if (agent.IsHuman)
			{
				CharacterObject characterObject = agent.Character as CharacterObject;
				IAgentOriginBase origin = agent.Origin;
				PartyBase partyBase = (PartyBase)((origin != null) ? origin.BattleCombatant : null);
				Hero hero = ((partyBase != null) ? partyBase.LeaderHero : null);
				if (characterObject != null && hero != null && characterObject.Tier >= (int)DefaultPerks.Leadership.LoyaltyAndHonor.PrimaryBonus && hero.GetPerkValue(DefaultPerks.Leadership.LoyaltyAndHonor))
				{
					flag = false;
				}
			}
			return flag;
		}

		public override float CalculateCasualtiesFactor(BattleSideEnum battleSide)
		{
			float num = 1f;
			if (Mission.Current != null && battleSide != -1)
			{
				float removedAgentRatioForSide = Mission.Current.GetRemovedAgentRatioForSide(battleSide);
				num += removedAgentRatioForSide * 2f;
				num = MathF.Max(0f, num);
			}
			return num;
		}

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
						if ((agent = enumerator.Current as Agent) != null && agent.IsHuman && agent.IsAIControlled)
						{
							num2++;
							num += AgentComponentExtensions.GetMorale(agent);
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
