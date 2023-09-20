using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents
{
	// Token: 0x02000085 RID: 133
	public class SandboxAgentApplyDamageModel : AgentApplyDamageModel
	{
		// Token: 0x06000594 RID: 1428 RVA: 0x000275C0 File Offset: 0x000257C0
		public override float CalculateDamage(in AttackInformation attackInformation, in AttackCollisionData collisionData, in MissionWeapon weapon, float baseDamage)
		{
			Formation attackerFormation = attackInformation.AttackerFormation;
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(attackerFormation);
			CharacterObject characterObject = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerRiderAgentCharacter : attackInformation.AttackerAgentCharacter) as CharacterObject;
			IAgentOriginBase agentOriginBase = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerRiderAgentOrigin : attackInformation.AttackerAgentOrigin);
			CharacterObject characterObject2 = attackInformation.AttackerCaptainCharacter as CharacterObject;
			bool flag = attackInformation.IsAttackerAgentHuman && !attackInformation.DoesAttackerHaveMountAgent;
			bool flag2 = attackInformation.DoesAttackerHaveMountAgent || attackInformation.DoesAttackerHaveRiderAgent;
			CharacterObject characterObject3 = (attackInformation.IsVictimAgentMount ? attackInformation.VictimRiderAgentCharacter : attackInformation.VictimAgentCharacter) as CharacterObject;
			CharacterObject characterObject4 = attackInformation.VictimCaptainCharacter as CharacterObject;
			bool flag3 = attackInformation.IsVictimAgentHuman && !attackInformation.DoesVictimHaveMountAgent;
			bool flag4 = attackInformation.DoesVictimHaveMountAgent || attackInformation.DoesVictimHaveRiderAgent;
			Formation victimFormation = attackInformation.VictimFormation;
			BannerComponent activeBanner2 = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(victimFormation);
			MissionWeapon missionWeapon = attackInformation.VictimMainHandWeapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			AttackCollisionData attackCollisionData = collisionData;
			bool flag5;
			if (!attackCollisionData.AttackBlockedWithShield)
			{
				attackCollisionData = collisionData;
				flag5 = attackCollisionData.CollidedWithShieldOnBack;
			}
			else
			{
				flag5 = true;
			}
			bool flag6 = flag5;
			float num = 0f;
			missionWeapon = weapon;
			WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
			bool flag7 = false;
			if (currentUsageItem2 != null && currentUsageItem2.IsConsumable)
			{
				attackCollisionData = collisionData;
				if (attackCollisionData.CollidedWithShieldOnBack && characterObject3 != null && characterObject3.GetPerkValue(DefaultPerks.Crossbow.Pavise))
				{
					float num2 = MBMath.ClampFloat(DefaultPerks.Crossbow.Pavise.PrimaryBonus / 100f, 0f, 1f);
					flag7 = MBRandom.RandomFloat <= num2;
				}
			}
			if (!flag7)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(baseDamage, false, null);
				if (characterObject != null)
				{
					if (currentUsageItem2 != null)
					{
						if (currentUsageItem2.IsMeleeWeapon)
						{
							if (currentUsageItem2.RelevantSkill == DefaultSkills.OneHanded)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.DeadlyPurpose, characterObject, true, ref explainedNumber);
								if (flag2)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.Cavalry, characterObject, true, ref explainedNumber);
								}
								missionWeapon = attackInformation.OffHandItem;
								if (missionWeapon.IsEmpty)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.Duelist, characterObject, true, ref explainedNumber);
								}
								if (currentUsageItem2.WeaponClass == 6 || currentUsageItem2.WeaponClass == 4)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.ToBeBlunt, characterObject, true, ref explainedNumber);
								}
								if (flag6)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.Prestige, characterObject, true, ref explainedNumber);
								}
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Roguery.Carver, characterObject2, ref explainedNumber);
								PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.OneHanded.WayOfTheSword, characterObject, DefaultSkills.OneHanded, false, ref explainedNumber, 250);
							}
							else if (currentUsageItem2.RelevantSkill == DefaultSkills.TwoHanded)
							{
								if (flag6)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.WoodChopper, characterObject, true, ref explainedNumber);
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.ShieldBreaker, characterObject, true, ref explainedNumber);
									if (flag)
									{
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.WoodChopper, characterObject2, ref explainedNumber);
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.ShieldBreaker, characterObject2, ref explainedNumber);
									}
								}
								if (currentUsageItem2.WeaponClass == 5 || currentUsageItem2.WeaponClass == 8)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.HeadBasher, characterObject, true, ref explainedNumber);
								}
								if (attackInformation.IsVictimAgentMount)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.BeastSlayer, characterObject, true, ref explainedNumber);
									if (flag)
									{
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.BeastSlayer, characterObject2, ref explainedNumber);
									}
								}
								if (attackInformation.AttackerHitPointRate < 0.5f)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.Berserker, characterObject, true, ref explainedNumber);
								}
								else if (attackInformation.AttackerHitPointRate > 0.9f)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.Confidence, characterObject, true, ref explainedNumber);
								}
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.BladeMaster, characterObject, true, ref explainedNumber);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Roguery.DashAndSlash, characterObject2, ref explainedNumber);
								PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.TwoHanded.WayOfTheGreatAxe, characterObject, DefaultSkills.TwoHanded, false, ref explainedNumber, 250);
							}
							else if (currentUsageItem2.RelevantSkill == DefaultSkills.Polearm)
							{
								if (flag2)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.Cavalry, characterObject, true, ref explainedNumber);
								}
								else
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.Pikeman, characterObject, true, ref explainedNumber);
								}
								attackCollisionData = collisionData;
								if (attackCollisionData.StrikeType == 1)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.CleanThrust, characterObject, true, ref explainedNumber);
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.SharpenTheTip, characterObject, true, ref explainedNumber);
								}
								if (attackInformation.IsVictimAgentMount)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.SteedKiller, characterObject, true, ref explainedNumber);
									if (flag)
									{
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.SteedKiller, characterObject2, ref explainedNumber);
									}
								}
								if (attackInformation.IsHeadShot)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.Guards, characterObject, true, ref explainedNumber);
								}
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Phalanx, characterObject2, ref explainedNumber);
								PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Polearm.WayOfTheSpear, characterObject, DefaultSkills.Polearm, false, ref explainedNumber, 250);
							}
							else if (currentUsageItem2.IsShield)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.Basher, characterObject, true, ref explainedNumber);
							}
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.Powerful, characterObject, true, ref explainedNumber);
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.Powerful, characterObject2, ref explainedNumber);
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Engineering.ImprovedTools, characterObject2, ref explainedNumber);
							missionWeapon = weapon;
							bool flag8;
							if (missionWeapon.Item != null)
							{
								missionWeapon = weapon;
								flag8 = missionWeapon.Item.ItemType == 10;
							}
							else
							{
								flag8 = false;
							}
							if (flag8)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.FlexibleFighter, characterObject, true, ref explainedNumber);
							}
							if (flag2)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.MountedWarrior, characterObject, true, ref explainedNumber);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.MountedWarrior, characterObject2, ref explainedNumber);
							}
							else
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.DeadlyPurpose, characterObject2, ref explainedNumber);
								attackCollisionData = collisionData;
								if (attackCollisionData.StrikeType == 1)
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.SharpenTheTip, characterObject2, ref explainedNumber);
								}
							}
							if (activeBanner != null)
							{
								BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedMeleeDamage, activeBanner, ref explainedNumber);
								if (attackInformation.DoesVictimHaveMountAgent)
								{
									BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedDamageAgainstMountedTroops, activeBanner, ref explainedNumber);
								}
							}
						}
						else if (currentUsageItem2.IsConsumable)
						{
							if (currentUsageItem2.RelevantSkill == DefaultSkills.Bow)
							{
								attackCollisionData = collisionData;
								if (attackCollisionData.CollisionBoneIndex != -1)
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.BowControl, characterObject2, ref explainedNumber);
									if (attackInformation.IsHeadShot)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.DeadAim, characterObject, true, ref explainedNumber);
									}
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.StrongBows, characterObject, true, ref explainedNumber);
									if (characterObject.Tier >= 3)
									{
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.StrongBows, characterObject2, ref explainedNumber);
									}
									if (attackInformation.IsVictimAgentMount)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.HunterClan, characterObject, true, ref explainedNumber);
									}
									PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Bow.Deadshot, characterObject, DefaultSkills.Bow, false, ref explainedNumber, 200);
									goto IL_7CD;
								}
							}
							if (currentUsageItem2.RelevantSkill == DefaultSkills.Crossbow)
							{
								attackCollisionData = collisionData;
								if (attackCollisionData.CollisionBoneIndex != -1)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Engineering.TorsionEngines, characterObject, false, ref explainedNumber);
									if (attackInformation.IsVictimAgentMount)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Unhorser, characterObject, true, ref explainedNumber);
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.Unhorser, characterObject2, ref explainedNumber);
									}
									if (attackInformation.IsHeadShot)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Sheriff, characterObject, true, ref explainedNumber);
									}
									if (flag3 && currentUsageItem != null && currentUsageItem.RelevantSkill == DefaultSkills.Crossbow)
									{
										PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.Sheriff, characterObject2, ref explainedNumber);
									}
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.HammerBolts, characterObject2, ref explainedNumber);
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Engineering.DreadfulSieger, characterObject2, ref explainedNumber);
									PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Crossbow.MightyPull, characterObject, DefaultSkills.Crossbow, false, ref explainedNumber, 200);
									goto IL_7CD;
								}
							}
							if (currentUsageItem2.RelevantSkill == DefaultSkills.Throwing)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.StrongArms, characterObject, true, ref explainedNumber);
								if (flag6)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.ShieldBreaker, characterObject, true, ref explainedNumber);
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.ShieldBreaker, characterObject2, ref explainedNumber);
									if (currentUsageItem2.WeaponClass == 19)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.Splinters, characterObject, true, ref explainedNumber);
									}
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.Splinters, characterObject2, ref explainedNumber);
								}
								if (attackInformation.IsVictimAgentMount)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.Hunter, characterObject, true, ref explainedNumber);
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.Hunter, characterObject2, ref explainedNumber);
								}
								if (flag2)
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.MountedSkirmisher, characterObject2, ref explainedNumber);
								}
								else
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.Impale, characterObject2, ref explainedNumber);
								}
								if (flag4)
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.KnockOff, characterObject2, ref explainedNumber);
								}
								if (attackInformation.VictimAgentHealth <= attackInformation.VictimAgentMaxHealth * 0.5f)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.LastHit, characterObject, true, ref explainedNumber);
								}
								if (attackInformation.IsHeadShot)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.HeadHunter, characterObject, true, ref explainedNumber);
								}
								PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Throwing.UnstoppableForce, characterObject, DefaultSkills.Throwing, false, ref explainedNumber, 200);
							}
							IL_7CD:
							if (flag2)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.HorseArcher, characterObject, true, ref explainedNumber);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.HorseArcher, characterObject2, ref explainedNumber);
							}
							if (activeBanner != null)
							{
								BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedRangedDamage, activeBanner, ref explainedNumber);
							}
						}
						missionWeapon = weapon;
						if (missionWeapon.Item != null)
						{
							missionWeapon = weapon;
							if (missionWeapon.Item.IsCivilian)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Roguery.Carver, characterObject, true, ref explainedNumber);
							}
						}
					}
					attackCollisionData = collisionData;
					if (attackCollisionData.IsHorseCharge)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.FullSpeed, characterObject, true, ref explainedNumber);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.FullSpeed, characterObject2, ref explainedNumber);
						if (characterObject.GetPerkValue(DefaultPerks.Riding.TheWayOfTheSaddle))
						{
							float num3 = MathF.Max((float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(characterObject, agentOriginBase, attackerFormation, DefaultSkills.Riding) - DefaultPerks.Riding.TheWayOfTheSaddle.PrimaryBonus, 0f);
							num3 *= DefaultPerks.Riding.TheWayOfTheSaddle.SecondaryBonus / 10f;
							explainedNumber.Add(num3, null, null);
						}
						if (activeBanner != null)
						{
							BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedChargeDamage, activeBanner, ref explainedNumber);
						}
					}
					if (flag)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.HeadBasher, characterObject2, ref explainedNumber);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.RecklessCharge, characterObject2, ref explainedNumber);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Pikeman, characterObject2, ref explainedNumber);
						if (flag4)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Braced, characterObject2, ref explainedNumber);
						}
					}
					if (flag2)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Cavalry, characterObject2, ref explainedNumber);
					}
					if (currentUsageItem2 == null)
					{
						attackCollisionData = collisionData;
						if (attackCollisionData.IsAlternativeAttack && characterObject.GetPerkValue(DefaultPerks.Athletics.StrongLegs))
						{
							explainedNumber.AddFactor(1f, null);
						}
					}
					if (flag6)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Engineering.WallBreaker, characterObject2, ref explainedNumber);
					}
					attackCollisionData = collisionData;
					if (attackCollisionData.EntityExists)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.Vandal, characterObject2, ref explainedNumber);
					}
					if (characterObject3 != null)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.Coaching, characterObject2, ref explainedNumber);
						if (characterObject3.Culture.IsBandit)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.LawKeeper, characterObject2, ref explainedNumber);
						}
						if (flag2 && flag3)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.Gensdarmes, characterObject2, ref explainedNumber);
						}
					}
					if (characterObject.Culture.IsBandit)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Roguery.PartnersInCrime, characterObject2, ref explainedNumber);
					}
				}
				float num4 = 1f;
				if (Mission.Current.IsSallyOutBattle)
				{
					DestructableComponent hitObjectDestructibleComponent = attackInformation.HitObjectDestructibleComponent;
					if (hitObjectDestructibleComponent != null && hitObjectDestructibleComponent.GameEntity.GetFirstScriptOfType<SiegeWeapon>() != null)
					{
						num4 *= 4.5f;
					}
				}
				explainedNumber..ctor(explainedNumber.ResultNumber * num4, false, null);
				if (attackInformation.DoesAttackerHaveMountAgent && (currentUsageItem2 == null || currentUsageItem2.RelevantSkill != DefaultSkills.Crossbow))
				{
					int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(characterObject, agentOriginBase, attackerFormation, DefaultSkills.Riding);
					float num5 = -0.01f * MathF.Max(0f, DefaultSkillEffects.HorseWeaponDamagePenalty.GetPrimaryValue(effectiveSkill));
					explainedNumber.AddFactor(num5, null);
				}
				if (characterObject3 != null)
				{
					if (currentUsageItem2 != null)
					{
						if (currentUsageItem2.IsConsumable)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.SkirmishPhaseMaster, characterObject3, true, ref explainedNumber);
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.Skirmisher, characterObject4, ref explainedNumber);
							if (characterObject3.IsRanged)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.SkirmishPhaseMaster, characterObject4, ref explainedNumber);
							}
							if (currentUsageItem != null)
							{
								if (currentUsageItem.WeaponClass == 16)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.CounterFire, characterObject3, true, ref explainedNumber);
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.CounterFire, characterObject4, ref explainedNumber);
								}
								else if (currentUsageItem.RelevantSkill == DefaultSkills.Throwing)
								{
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.Skirmisher, characterObject3, true, ref explainedNumber);
								}
							}
							if (activeBanner2 != null)
							{
								BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedRangedAttackDamage, activeBanner2, ref explainedNumber);
							}
						}
						else if (currentUsageItem2.IsMeleeWeapon)
						{
							if (characterObject4 != null)
							{
								Formation victimFormation2 = attackInformation.VictimFormation;
								if (victimFormation2 != null && victimFormation2.ArrangementOrder.OrderEnum == 5)
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.Basher, characterObject4, ref explainedNumber);
								}
							}
							if (activeBanner2 != null)
							{
								BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedMeleeAttackDamage, activeBanner2, ref explainedNumber);
							}
						}
					}
					if (flag6)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.SteelCoreShields, characterObject3, true, ref explainedNumber);
						if (flag3)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.SteelCoreShields, characterObject4, ref explainedNumber);
						}
						attackCollisionData = collisionData;
						if (attackCollisionData.AttackBlockedWithShield)
						{
							attackCollisionData = collisionData;
							if (!attackCollisionData.CorrectSideShieldBlock)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.ShieldWall, characterObject3, true, ref explainedNumber);
							}
						}
					}
					attackCollisionData = collisionData;
					if (attackCollisionData.IsHorseCharge)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.SureFooted, characterObject3, true, ref explainedNumber);
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.Braced, characterObject3, true, ref explainedNumber);
						if (characterObject4 != null)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.SureFooted, characterObject4, ref explainedNumber);
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.Braced, characterObject4, ref explainedNumber);
						}
					}
					attackCollisionData = collisionData;
					if (attackCollisionData.IsFallDamage)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.StrongLegs, characterObject3, true, ref explainedNumber);
					}
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.EliteReserves, characterObject4, ref explainedNumber);
				}
				num = explainedNumber.ResultNumber;
			}
			return MathF.Max(0f, num);
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00028238 File Offset: 0x00026438
		public override bool DecideCrushedThrough(Agent attackerAgent, Agent defenderAgent, float totalAttackEnergy, Agent.UsageDirection attackDirection, StrikeType strikeType, WeaponComponentData defendItem, bool isPassiveUsage)
		{
			EquipmentIndex equipmentIndex = attackerAgent.GetWieldedItemIndex(1);
			if (equipmentIndex == -1)
			{
				equipmentIndex = attackerAgent.GetWieldedItemIndex(0);
			}
			if (((equipmentIndex != -1) ? attackerAgent.Equipment[equipmentIndex].CurrentUsageItem : null) == null || isPassiveUsage || strikeType != null || attackDirection != null)
			{
				return false;
			}
			float num = 58f;
			if (defendItem != null && defendItem.IsShield)
			{
				num *= 1.2f;
			}
			return totalAttackEnergy > num;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x000282A8 File Offset: 0x000264A8
		public override void DecideMissileWeaponFlags(Agent attackerAgent, MissionWeapon missileWeapon, ref WeaponFlags missileWeaponFlags)
		{
			CharacterObject characterObject = ((attackerAgent != null) ? attackerAgent.Character : null) as CharacterObject;
			if (characterObject != null && missileWeapon.CurrentUsageItem.WeaponClass == 21 && characterObject.GetPerkValue(DefaultPerks.Throwing.Impale))
			{
				missileWeaponFlags |= 131072L;
			}
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x000282F2 File Offset: 0x000264F2
		public override bool CanWeaponIgnoreFriendlyFireChecks(WeaponComponentData weapon)
		{
			return weapon != null && weapon.IsConsumable && Extensions.HasAnyFlag<WeaponFlags>(weapon.WeaponFlags, 131072L) && Extensions.HasAnyFlag<WeaponFlags>(weapon.WeaponFlags, 1073741824L);
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00028328 File Offset: 0x00026528
		public override bool CanWeaponDismount(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			CharacterObject characterObject;
			return MBMath.IsBetween(blow.VictimBodyPart, 0, 6) && ((!attackerAgent.HasMount && blow.StrikeType == null && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 33554432L)) || (blow.StrikeType == 1 && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 16777216L)) || ((characterObject = attackerAgent.Character as CharacterObject) != null && ((attackerWeapon.RelevantSkill == DefaultSkills.Crossbow && attackerWeapon.IsConsumable && characterObject.GetPerkValue(DefaultPerks.Crossbow.HammerBolts)) || (attackerWeapon.RelevantSkill == DefaultSkills.Throwing && attackerWeapon.IsConsumable && characterObject.GetPerkValue(DefaultPerks.Throwing.KnockOff)))));
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x000283EC File Offset: 0x000265EC
		public override void CalculateCollisionStunMultipliers(Agent attackerAgent, Agent defenderAgent, bool isAlternativeAttack, CombatCollisionResult collisionResult, WeaponComponentData attackerWeapon, WeaponComponentData defenderWeapon, out float attackerStunMultiplier, out float defenderStunMultiplier)
		{
			float num = 1f;
			float num2 = 1f;
			CharacterObject characterObject;
			if ((characterObject = attackerAgent.Character as CharacterObject) != null && (collisionResult == 3 || collisionResult == 4) && characterObject.GetPerkValue(DefaultPerks.Athletics.MightyBlow))
			{
				num += num * DefaultPerks.Athletics.MightyBlow.PrimaryBonus;
			}
			defenderStunMultiplier = MathF.Max(0f, num);
			attackerStunMultiplier = MathF.Max(0f, num2);
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00028458 File Offset: 0x00026658
		public override bool CanWeaponKnockback(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			AttackCollisionData attackCollisionData = collisionData;
			return MBMath.IsBetween(attackCollisionData.VictimHitBodyPart, 0, 6) && !Extensions.HasAnyFlag<WeaponFlags>(attackerWeapon.WeaponFlags, 67108864L) && (attackerWeapon.IsConsumable || (blow.BlowFlag & 128) != null || (blow.StrikeType == 1 && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 64L)));
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x000284C8 File Offset: 0x000266C8
		public override bool CanWeaponKnockDown(Agent attackerAgent, Agent victimAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			if (attackerWeapon.WeaponClass == 18)
			{
				return true;
			}
			AttackCollisionData attackCollisionData = collisionData;
			BoneBodyPartType victimHitBodyPart = attackCollisionData.VictimHitBodyPart;
			bool flag = MBMath.IsBetween(victimHitBodyPart, 0, 6);
			if (!victimAgent.HasMount && victimHitBodyPart == 8)
			{
				flag = true;
			}
			return flag && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 67108864L) && ((attackerWeapon.IsPolearm && blow.StrikeType == 1) || (attackerWeapon.IsMeleeWeapon && blow.StrikeType == null && MissionCombatMechanicsHelper.DecideSweetSpotCollision(ref collisionData)));
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x00028554 File Offset: 0x00026754
		public override float GetDismountPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			float num = 0f;
			if (blow.StrikeType == null && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 33554432L))
			{
				num += 0.25f;
			}
			CharacterObject characterObject;
			if (attackerWeapon != null && (characterObject = attackerAgent.Character as CharacterObject) != null)
			{
				if (attackerWeapon.RelevantSkill == DefaultSkills.Polearm && characterObject.GetPerkValue(DefaultPerks.Polearm.Braced))
				{
					num += DefaultPerks.Polearm.Braced.PrimaryBonus;
				}
				else if (attackerWeapon.RelevantSkill == DefaultSkills.Crossbow && attackerWeapon.IsConsumable && characterObject.GetPerkValue(DefaultPerks.Crossbow.HammerBolts))
				{
					num += DefaultPerks.Crossbow.HammerBolts.PrimaryBonus;
				}
				else if (attackerWeapon.RelevantSkill == DefaultSkills.Throwing && attackerWeapon.IsConsumable && characterObject.GetPerkValue(DefaultPerks.Throwing.KnockOff))
				{
					num += DefaultPerks.Throwing.KnockOff.PrimaryBonus;
				}
			}
			return MathF.Max(0f, num);
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0002863C File Offset: 0x0002683C
		public override float GetKnockBackPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			float num = 0f;
			CharacterObject characterObject;
			if (attackerWeapon != null && attackerWeapon.RelevantSkill == DefaultSkills.Polearm && (characterObject = ((attackerAgent != null) ? attackerAgent.Character : null) as CharacterObject) != null && characterObject.GetPerkValue(DefaultPerks.Polearm.KeepAtBay))
			{
				num += DefaultPerks.Polearm.KeepAtBay.PrimaryBonus;
			}
			return num;
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x00028690 File Offset: 0x00026890
		public override float GetKnockDownPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			float num = 0f;
			if (attackerWeapon.WeaponClass == 18)
			{
				num += 0.25f;
			}
			else if (attackerWeapon.IsMeleeWeapon)
			{
				CharacterObject characterObject = ((attackerAgent != null) ? attackerAgent.Character : null) as CharacterObject;
				AttackCollisionData attackCollisionData;
				if (blow.StrikeType == null)
				{
					attackCollisionData = collisionData;
					if (attackCollisionData.VictimHitBodyPart == 8)
					{
						num += 0.1f;
					}
					if (characterObject != null && attackerWeapon.RelevantSkill == DefaultSkills.TwoHanded && characterObject.GetPerkValue(DefaultPerks.TwoHanded.ShowOfStrength))
					{
						num += DefaultPerks.TwoHanded.ShowOfStrength.PrimaryBonus;
					}
				}
				attackCollisionData = collisionData;
				if (attackCollisionData.VictimHitBodyPart == null)
				{
					num += 0.15f;
				}
				if (characterObject != null && attackerWeapon.RelevantSkill == DefaultSkills.Polearm && characterObject.GetPerkValue(DefaultPerks.Polearm.HardKnock))
				{
					num += DefaultPerks.Polearm.HardKnock.PrimaryBonus;
				}
			}
			return num;
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x00028767 File Offset: 0x00026967
		public override float GetHorseChargePenetration()
		{
			return 0.37f;
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x00028770 File Offset: 0x00026970
		public override float CalculateStaggerThresholdMultiplier(Agent defenderAgent)
		{
			float num = 1f;
			CharacterObject characterObject = defenderAgent.Character as CharacterObject;
			Formation formation = defenderAgent.Formation;
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
			CharacterObject characterObject2 = obj as CharacterObject;
			if (characterObject != null)
			{
				if (characterObject2 == characterObject)
				{
					characterObject2 = null;
				}
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(1f, false, null);
				if (defenderAgent.HasMount)
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.DauntlessSteed, characterObject, true, ref explainedNumber);
				}
				else
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.Spartan, characterObject, true, ref explainedNumber);
				}
				if (!defenderAgent.WieldedWeapon.IsEmpty && defenderAgent.WieldedWeapon.CurrentUsageItem.WeaponClass == 16 && defenderAgent.GetCurrentActionType(1) == 18)
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.DeftHands, characterObject, true, ref explainedNumber);
					if (characterObject2 != null)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.DeftHands, characterObject2, ref explainedNumber);
					}
				}
				num = explainedNumber.ResultNumber;
			}
			return MathF.Max(num, 0f);
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x00028858 File Offset: 0x00026A58
		public override float CalculatePassiveAttackDamage(BasicCharacterObject attackerCharacter, in AttackCollisionData collisionData, float baseDamage)
		{
			CharacterObject characterObject = attackerCharacter as CharacterObject;
			if (characterObject != null)
			{
				AttackCollisionData attackCollisionData = collisionData;
				if (attackCollisionData.AttackBlockedWithShield && characterObject.GetPerkValue(DefaultPerks.Polearm.UnstoppableForce))
				{
					baseDamage += baseDamage * DefaultPerks.Polearm.UnstoppableForce.PrimaryBonus;
				}
			}
			return baseDamage;
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x000288A0 File Offset: 0x00026AA0
		public override MeleeCollisionReaction DecidePassiveAttackCollisionReaction(Agent attacker, Agent defender, bool isFatalHit)
		{
			MeleeCollisionReaction meleeCollisionReaction = 3;
			if (isFatalHit && attacker.HasMount)
			{
				float num = 0.05f;
				CharacterObject characterObject;
				if ((characterObject = attacker.Character as CharacterObject) != null && characterObject.GetPerkValue(DefaultPerks.Polearm.Skewer))
				{
					num += DefaultPerks.Polearm.Skewer.PrimaryBonus;
				}
				if (MBRandom.RandomFloat < num)
				{
					meleeCollisionReaction = 0;
				}
			}
			return meleeCollisionReaction;
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x000288F4 File Offset: 0x00026AF4
		public override float CalculateShieldDamage(in AttackInformation attackInformation, float baseDamage)
		{
			Formation victimFormation = attackInformation.VictimFormation;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(baseDamage, false, null);
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(victimFormation);
			if (activeBanner != null)
			{
				BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedShieldDamage, activeBanner, ref explainedNumber);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0002893C File Offset: 0x00026B3C
		public override float GetDamageMultiplierForBodyPart(BoneBodyPartType bodyPart, DamageTypes type, bool isHuman)
		{
			float num = 1f;
			switch (bodyPart)
			{
			case -1:
				num = 1f;
				break;
			case 0:
				switch (type)
				{
				case -1:
					num = 2f;
					break;
				case 0:
					num = 1.2f;
					break;
				case 1:
					if (isHuman)
					{
						num = 2f;
					}
					else
					{
						num = 1.2f;
					}
					break;
				case 2:
					num = 1.2f;
					break;
				}
				break;
			case 1:
				switch (type)
				{
				case -1:
					num = 2f;
					break;
				case 0:
					num = 1.2f;
					break;
				case 1:
					if (isHuman)
					{
						num = 2f;
					}
					else
					{
						num = 1.2f;
					}
					break;
				case 2:
					num = 1.2f;
					break;
				}
				break;
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
				num = (isHuman ? 1f : 0.8f);
				break;
			case 8:
				num = 0.8f;
				break;
			}
			return num;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00028A3C File Offset: 0x00026C3C
		public override bool DecideAgentShrugOffBlow(Agent victimAgent, AttackCollisionData collisionData, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentShrugOffBlow(victimAgent, collisionData, ref blow);
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00028A46 File Offset: 0x00026C46
		public override bool DecideAgentDismountedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentDismountedByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00028A54 File Offset: 0x00026C54
		public override bool DecideAgentKnockedBackByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentKnockedBackByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00028A62 File Offset: 0x00026C62
		public override bool DecideAgentKnockedDownByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentKnockedDownByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00028A70 File Offset: 0x00026C70
		public override bool DecideMountRearedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideMountRearedByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x040002BA RID: 698
		private const float SallyOutSiegeEngineDamageMultiplier = 4.5f;
	}
}
