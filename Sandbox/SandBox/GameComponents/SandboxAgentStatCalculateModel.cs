﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.GameComponents
{
	public class SandboxAgentStatCalculateModel : AgentStatCalculateModel
	{
		public override float GetDifficultyModifier()
		{
			Campaign campaign = Campaign.Current;
			float? num;
			if (campaign == null)
			{
				num = null;
			}
			else
			{
				GameModels models = campaign.Models;
				if (models == null)
				{
					num = null;
				}
				else
				{
					DifficultyModel difficultyModel = models.DifficultyModel;
					num = ((difficultyModel != null) ? new float?(difficultyModel.GetCombatAIDifficultyMultiplier()) : null);
				}
			}
			float? num2 = num;
			if (num2 == null)
			{
				return 0.5f;
			}
			return num2.GetValueOrDefault();
		}

		public override bool CanAgentRideMount(Agent agent, Agent targetMount)
		{
			return agent.CheckSkillForMounting(targetMount);
		}

		public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
		{
			agentDrivenProperties.ArmorEncumbrance = this.GetEffectiveArmorEncumbrance(agent, spawnEquipment);
			if (agent.IsHero)
			{
				CharacterObject characterObject = agent.Character as CharacterObject;
				AgentFlag agentFlag = agent.GetAgentFlags();
				if (characterObject.GetPerkValue(DefaultPerks.Bow.HorseMaster))
				{
					agentFlag |= 16777216;
				}
				if (characterObject.GetPerkValue(DefaultPerks.Crossbow.MountedCrossbowman))
				{
					agentFlag |= 33554432;
				}
				if (characterObject.GetPerkValue(DefaultPerks.TwoHanded.ProjectileDeflection))
				{
					agentFlag |= 67108864;
				}
				agent.SetAgentFlags(agentFlag);
			}
			else
			{
				agent.HealthLimit = this.GetEffectiveMaxHealth(agent);
				agent.Health = agent.HealthLimit;
			}
			this.UpdateAgentStats(agent, agentDrivenProperties);
		}

		public override void InitializeMissionEquipment(Agent agent)
		{
			if (agent.IsHuman)
			{
				CharacterObject characterObject = agent.Character as CharacterObject;
				if (characterObject != null)
				{
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
					MapEvent mapEvent = ((partyBase != null) ? partyBase.MapEvent : null);
					MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
					CharacterObject characterObject2 = PartyBaseHelper.GetVisualPartyLeader(partyBase);
					if (characterObject2 == characterObject)
					{
						characterObject2 = null;
					}
					MissionEquipment equipment = agent.Equipment;
					for (int i = 0; i < 5; i++)
					{
						EquipmentIndex equipmentIndex = i;
						MissionWeapon missionWeapon = equipment[equipmentIndex];
						if (!missionWeapon.IsEmpty)
						{
							WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
							if (currentUsageItem != null)
							{
								if (currentUsageItem.IsConsumable && currentUsageItem.RelevantSkill != null)
								{
									ExplainedNumber explainedNumber;
									explainedNumber..ctor(0f, false, null);
									if (currentUsageItem.RelevantSkill == DefaultSkills.Bow)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.DeepQuivers, characterObject, true, ref explainedNumber);
										if (characterObject2 != null && characterObject2.GetPerkValue(DefaultPerks.Bow.DeepQuivers))
										{
											explainedNumber.Add(DefaultPerks.Bow.DeepQuivers.SecondaryBonus, null, null);
										}
									}
									else if (currentUsageItem.RelevantSkill == DefaultSkills.Crossbow)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Fletcher, characterObject, true, ref explainedNumber);
										if (characterObject2 != null && characterObject2.GetPerkValue(DefaultPerks.Crossbow.Fletcher))
										{
											explainedNumber.Add(DefaultPerks.Crossbow.Fletcher.SecondaryBonus, null, null);
										}
									}
									else if (currentUsageItem.RelevantSkill == DefaultSkills.Throwing)
									{
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.WellPrepared, characterObject, true, ref explainedNumber);
										PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.Resourceful, characterObject, true, ref explainedNumber);
										if (agent.HasMount)
										{
											PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.Saddlebags, characterObject, true, ref explainedNumber);
										}
										PerkHelper.AddPerkBonusForParty(DefaultPerks.Throwing.WellPrepared, mobileParty, false, ref explainedNumber);
									}
									int num = MathF.Round(explainedNumber.ResultNumber);
									ExplainedNumber explainedNumber2;
									explainedNumber2..ctor((float)((int)missionWeapon.Amount + num), false, null);
									if (mobileParty != null && mapEvent != null && mapEvent.AttackerSide == partyBase.MapEventSide && mapEvent.EventType == 5)
									{
										PerkHelper.AddPerkBonusForParty(DefaultPerks.Engineering.MilitaryPlanner, mobileParty, true, ref explainedNumber2);
									}
									int num2 = MathF.Round(explainedNumber2.ResultNumber);
									if (num2 != (int)missionWeapon.Amount)
									{
										equipment.SetAmountOfSlot(equipmentIndex, (short)num2, true);
									}
								}
								else if (currentUsageItem.IsShield)
								{
									ExplainedNumber explainedNumber3;
									explainedNumber3..ctor((float)missionWeapon.HitPoints, false, null);
									PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Engineering.Scaffolds, characterObject, false, ref explainedNumber3);
									int num3 = MathF.Round(explainedNumber3.ResultNumber);
									if (num3 != (int)missionWeapon.HitPoints)
									{
										equipment.SetHitPointsOfSlot(equipmentIndex, (short)num3, true);
									}
								}
							}
						}
					}
				}
			}
		}

		public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			if (agent.IsHuman)
			{
				this.UpdateHumanStats(agent, agentDrivenProperties);
				return;
			}
			this.UpdateHorseStats(agent, agentDrivenProperties);
		}

		public override int GetEffectiveSkill(Agent agent, SkillObject skill)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)base.GetEffectiveSkill(agent, skill), false, null);
			CharacterObject characterObject = agent.Character as CharacterObject;
			Formation formation = agent.Formation;
			IAgentOriginBase origin = agent.Origin;
			PartyBase partyBase = (PartyBase)((origin != null) ? origin.BattleCombatant : null);
			MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
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
			if (characterObject2 == characterObject)
			{
				characterObject2 = null;
			}
			if (characterObject2 != null)
			{
				bool flag = skill == DefaultSkills.Bow || skill == DefaultSkills.Crossbow || skill == DefaultSkills.Throwing;
				bool flag2 = skill == DefaultSkills.OneHanded || skill == DefaultSkills.TwoHanded || skill == DefaultSkills.Polearm;
				if ((characterObject.IsInfantry && flag) || (characterObject.IsRanged && flag2))
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.FlexibleFighter, characterObject2, ref explainedNumber);
				}
			}
			if (skill == DefaultSkills.Bow)
			{
				if (characterObject2 != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.DeadAim, characterObject2, ref explainedNumber);
					if (characterObject.HasMount())
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.HorseMaster, characterObject2, ref explainedNumber);
					}
				}
			}
			else if (skill == DefaultSkills.Throwing)
			{
				if (characterObject2 != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.StrongArms, characterObject2, ref explainedNumber);
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.RunningThrow, characterObject2, ref explainedNumber);
				}
			}
			else if (skill == DefaultSkills.Crossbow && characterObject2 != null)
			{
				PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.DonkeysSwiftness, characterObject2, ref explainedNumber);
			}
			if (mobileParty != null && mobileParty.HasPerk(DefaultPerks.Roguery.OneOfTheFamily, false) && characterObject.Occupation == 15 && (skill.CharacterAttribute == DefaultCharacterAttributes.Vigor || skill.CharacterAttribute == DefaultCharacterAttributes.Control))
			{
				explainedNumber.Add(DefaultPerks.Roguery.OneOfTheFamily.PrimaryBonus, DefaultPerks.Roguery.OneOfTheFamily.Name, null);
			}
			if (characterObject.HasMount())
			{
				if (skill == DefaultSkills.Riding && characterObject2 != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.NimbleSteed, characterObject2, ref explainedNumber);
				}
			}
			else
			{
				if (mobileParty != null && formation != null)
				{
					bool flag3 = skill == DefaultSkills.OneHanded || skill == DefaultSkills.TwoHanded || skill == DefaultSkills.Polearm;
					bool flag4 = formation.ArrangementOrder.OrderEnum == 5;
					if (flag3 && flag4)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Polearm.Phalanx, mobileParty, true, ref explainedNumber);
					}
				}
				if (characterObject2 != null)
				{
					if (skill == DefaultSkills.OneHanded)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.WrappedHandles, characterObject2, ref explainedNumber);
					}
					else if (skill == DefaultSkills.TwoHanded)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.StrongGrip, characterObject2, ref explainedNumber);
					}
					else if (skill == DefaultSkills.Polearm)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.CleanThrust, characterObject2, ref explainedNumber);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.CounterWeight, characterObject2, ref explainedNumber);
					}
				}
			}
			return (int)explainedNumber.ResultNumber;
		}

		public override float GetWeaponDamageMultiplier(Agent agent, WeaponComponentData weapon)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(1f, false, null);
			SkillObject skillObject = ((weapon != null) ? weapon.RelevantSkill : null);
			CharacterObject characterObject;
			if ((characterObject = agent.Character as CharacterObject) != null && skillObject != null)
			{
				if (skillObject == DefaultSkills.OneHanded)
				{
					int effectiveSkill = this.GetEffectiveSkill(agent, skillObject);
					SkillHelper.AddSkillBonusForCharacter(skillObject, DefaultSkillEffects.OneHandedDamage, characterObject, ref explainedNumber, effectiveSkill, true, 0);
				}
				else if (skillObject == DefaultSkills.TwoHanded)
				{
					int effectiveSkill2 = this.GetEffectiveSkill(agent, skillObject);
					SkillHelper.AddSkillBonusForCharacter(skillObject, DefaultSkillEffects.TwoHandedDamage, characterObject, ref explainedNumber, effectiveSkill2, true, 0);
				}
				else if (skillObject == DefaultSkills.Polearm)
				{
					int effectiveSkill3 = this.GetEffectiveSkill(agent, skillObject);
					SkillHelper.AddSkillBonusForCharacter(skillObject, DefaultSkillEffects.PolearmDamage, characterObject, ref explainedNumber, effectiveSkill3, true, 0);
				}
				else if (skillObject == DefaultSkills.Bow)
				{
					int effectiveSkill4 = this.GetEffectiveSkill(agent, skillObject);
					SkillHelper.AddSkillBonusForCharacter(skillObject, DefaultSkillEffects.BowDamage, characterObject, ref explainedNumber, effectiveSkill4, true, 0);
				}
				else if (skillObject == DefaultSkills.Throwing)
				{
					int effectiveSkill5 = this.GetEffectiveSkill(agent, skillObject);
					SkillHelper.AddSkillBonusForCharacter(skillObject, DefaultSkillEffects.ThrowingDamage, characterObject, ref explainedNumber, effectiveSkill5, true, 0);
				}
			}
			return Math.Max(0f, explainedNumber.ResultNumber);
		}

		public override float GetKnockBackResistance(Agent agent)
		{
			if (agent.IsHuman)
			{
				int effectiveSkill = this.GetEffectiveSkill(agent, DefaultSkills.Athletics);
				float num = DefaultSkillEffects.KnockBackResistance.GetPrimaryValue(effectiveSkill) * 0.01f;
				return Math.Max(0f, num);
			}
			return float.MaxValue;
		}

		public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = -1)
		{
			if (agent.IsHuman)
			{
				int effectiveSkill = this.GetEffectiveSkill(agent, DefaultSkills.Athletics);
				float num = DefaultSkillEffects.KnockDownResistance.GetPrimaryValue(effectiveSkill) * 0.01f;
				if (agent.HasMount)
				{
					num += 0.1f;
				}
				else if (strikeType == 1)
				{
					num += 0.15f;
				}
				return Math.Max(0f, num);
			}
			return float.MaxValue;
		}

		public override float GetDismountResistance(Agent agent)
		{
			if (agent.IsHuman)
			{
				int effectiveSkill = this.GetEffectiveSkill(agent, DefaultSkills.Riding);
				float num = DefaultSkillEffects.DismountResistance.GetPrimaryValue(effectiveSkill) * 0.01f;
				return Math.Max(0f, num);
			}
			return float.MaxValue;
		}

		public override float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
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
			CharacterObject characterObject2 = obj as CharacterObject;
			if (characterObject == characterObject2)
			{
				characterObject2 = null;
			}
			float num = 0f;
			if (weapon.IsRangedWeapon)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(1f, false, null);
				if (characterObject != null)
				{
					if (weapon.RelevantSkill == DefaultSkills.Bow)
					{
						SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Bow, DefaultSkillEffects.BowAccuracy, characterObject, ref explainedNumber, weaponSkill, false, 0);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.QuickAdjustments, characterObject2, ref explainedNumber);
					}
					else if (weapon.RelevantSkill == DefaultSkills.Crossbow)
					{
						SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Crossbow, DefaultSkillEffects.CrossbowAccuracy, characterObject, ref explainedNumber, weaponSkill, false, 0);
					}
					else if (weapon.RelevantSkill == DefaultSkills.Throwing)
					{
						SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Throwing, DefaultSkillEffects.ThrowingAccuracy, characterObject, ref explainedNumber, weaponSkill, false, 0);
					}
				}
				num = (100f - (float)weapon.Accuracy) * explainedNumber.ResultNumber * 0.001f;
			}
			else if (Extensions.HasAllFlags<WeaponFlags>(weapon.WeaponFlags, 64L))
			{
				num = 1f - (float)weaponSkill * 0.01f;
			}
			return MathF.Max(num, 0f);
		}

		public override float GetInteractionDistance(Agent agent)
		{
			CharacterObject characterObject;
			if (agent.HasMount && (characterObject = agent.Character as CharacterObject) != null && characterObject.GetPerkValue(DefaultPerks.Throwing.LongReach))
			{
				return 3f;
			}
			return base.GetInteractionDistance(agent);
		}

		public override float GetMaxCameraZoom(Agent agent)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(1f, false, null);
			if (characterObject != null)
			{
				MissionEquipment equipment = agent.Equipment;
				EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(0);
				WeaponComponentData weaponComponentData = ((wieldedItemIndex != -1) ? equipment[wieldedItemIndex].CurrentUsageItem : null);
				if (weaponComponentData != null)
				{
					if (weaponComponentData.RelevantSkill == DefaultSkills.Bow)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.EagleEye, characterObject, true, ref explainedNumber);
					}
					else if (weaponComponentData.RelevantSkill == DefaultSkills.Crossbow)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.LongShots, characterObject, true, ref explainedNumber);
					}
					else if (weaponComponentData.RelevantSkill == DefaultSkills.Throwing)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.Focus, characterObject, true, ref explainedNumber);
					}
				}
			}
			return explainedNumber.ResultNumber;
		}

		public List<PerkObject> GetPerksOfAgent(CharacterObject agentCharacter, SkillObject skill = null, bool filterPerkRole = false, SkillEffect.PerkRole perkRole = 12)
		{
			List<PerkObject> list = new List<PerkObject>();
			if (agentCharacter != null)
			{
				foreach (PerkObject perkObject in PerkObject.All)
				{
					if (agentCharacter.GetPerkValue(perkObject) && (skill == null || skill == perkObject.Skill))
					{
						if (filterPerkRole)
						{
							if (perkObject.PrimaryRole == perkRole || perkObject.SecondaryRole == perkRole)
							{
								list.Add(perkObject);
							}
						}
						else
						{
							list.Add(perkObject);
						}
					}
				}
			}
			return list;
		}

		public override string GetMissionDebugInfoForAgent(Agent agent)
		{
			string text = "";
			text += "Base: Initial stats modified only by skills\n";
			text += "Effective (Eff): Stats that are modified by perks & mission effects\n\n";
			string text2 = "{0,-20}";
			text = string.Concat(new string[]
			{
				text,
				string.Format(text2, "Name"),
				": ",
				agent.Name,
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				string.Format(text2, "Age"),
				": ",
				(int)agent.Age,
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				string.Format(text2, "Health"),
				": ",
				agent.Health,
				"\n"
			});
			int num = (agent.IsHuman ? agent.Character.MaxHitPoints() : agent.Monster.HitPoints);
			text = string.Concat(new object[]
			{
				text,
				string.Format(text2, "Max.Health"),
				": ",
				num,
				"(Base)\n"
			});
			text = string.Concat(new object[]
			{
				text,
				string.Format(text2, ""),
				"  ",
				MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveMaxHealth(agent),
				"(Eff)\n"
			});
			text = string.Concat(new string[]
			{
				text,
				string.Format(text2, "Team"),
				": ",
				(agent.Team != null) ? (agent.Team.IsAttacker ? "Attacker" : "Defender") : "N/A",
				"\n"
			});
			if (agent.IsHuman)
			{
				string text3 = text2 + ": {1,4:G}, {2,4:G}";
				text += "-------------------------------------\n";
				text = text + string.Format(text2 + ": {1,4}, {2,4}", "Skills", "Base", "Eff") + "\n";
				text += "-------------------------------------\n";
				foreach (SkillObject skillObject in Skills.All)
				{
					int skillValue = agent.Character.GetSkillValue(skillObject);
					int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(agent, skillObject);
					string text4 = string.Format(text3, skillObject.Name, skillValue, effectiveSkill);
					text = text + text4 + "\n";
				}
				text += "-------------------------------------\n";
				CharacterObject characterObject = agent.Character as CharacterObject;
				string debugPerkInfoForAgent = this.GetDebugPerkInfoForAgent(characterObject, false, 12);
				if (debugPerkInfoForAgent.Length > 0)
				{
					text = text + string.Format(text2 + ": ", "Perks") + "\n";
					text += "-------------------------------------\n";
					text += debugPerkInfoForAgent;
					text += "-------------------------------------\n";
				}
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
				CharacterObject characterObject2 = obj as CharacterObject;
				string debugPerkInfoForAgent2 = this.GetDebugPerkInfoForAgent(characterObject2, true, 13);
				if (debugPerkInfoForAgent2.Length > 0)
				{
					text = string.Concat(new object[]
					{
						text,
						string.Format(text2 + ": ", "Captain Perks"),
						characterObject2.Name,
						"\n"
					});
					text += "-------------------------------------\n";
					text += debugPerkInfoForAgent2;
					text += "-------------------------------------\n";
				}
				IAgentOriginBase origin = agent.Origin;
				PartyBase partyBase = (PartyBase)((origin != null) ? origin.BattleCombatant : null);
				PartyBase partyBase2;
				if (partyBase == null)
				{
					partyBase2 = null;
				}
				else
				{
					MobileParty mobileParty = partyBase.MobileParty;
					partyBase2 = ((mobileParty != null) ? mobileParty.Party : null);
				}
				CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(partyBase2);
				string debugPerkInfoForAgent3 = this.GetDebugPerkInfoForAgent(visualPartyLeader, true, 5);
				if (debugPerkInfoForAgent3.Length > 0)
				{
					text = string.Concat(new object[]
					{
						text,
						string.Format(text2 + ": ", "Party Leader Perks"),
						visualPartyLeader.Name,
						"\n"
					});
					text += "-------------------------------------\n";
					text += debugPerkInfoForAgent3;
					text += "-------------------------------------\n";
				}
			}
			return text;
		}

		public float GetEffectiveArmorEncumbrance(Agent agent, Equipment equipment)
		{
			float totalWeightOfArmor = equipment.GetTotalWeightOfArmor(agent.IsHuman);
			float num = 1f;
			CharacterObject characterObject;
			if ((characterObject = agent.Character as CharacterObject) != null && characterObject.GetPerkValue(DefaultPerks.Athletics.FormFittingArmor))
			{
				num += DefaultPerks.Athletics.FormFittingArmor.PrimaryBonus;
			}
			return MathF.Max(0f, totalWeightOfArmor * num);
		}

		public override float GetEffectiveMaxHealth(Agent agent)
		{
			if (agent.IsHero)
			{
				return (float)agent.Character.MaxHitPoints();
			}
			float baseHealthLimit = agent.BaseHealthLimit;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(baseHealthLimit, false, null);
			if (agent.IsHuman)
			{
				CharacterObject characterObject = agent.Character as CharacterObject;
				IAgentOriginBase agentOriginBase = ((agent != null) ? agent.Origin : null);
				PartyBase partyBase = (PartyBase)((agentOriginBase != null) ? agentOriginBase.BattleCombatant : null);
				MobileParty mobileParty = ((partyBase != null) ? partyBase.MobileParty : null);
				CharacterObject characterObject2;
				if (mobileParty == null)
				{
					characterObject2 = null;
				}
				else
				{
					Hero leaderHero = mobileParty.LeaderHero;
					characterObject2 = ((leaderHero != null) ? leaderHero.CharacterObject : null);
				}
				CharacterObject characterObject3 = characterObject2;
				if (characterObject != null && characterObject3 != null)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.TwoHanded.ThickHides, mobileParty, false, ref explainedNumber);
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Polearm.HardyFrontline, mobileParty, true, ref explainedNumber);
					if (characterObject.IsRanged)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Crossbow.PickedShots, mobileParty, false, ref explainedNumber);
					}
					if (!agent.HasMount)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Athletics.WellBuilt, mobileParty, false, ref explainedNumber);
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Polearm.HardKnock, mobileParty, false, ref explainedNumber);
						if (characterObject.IsInfantry)
						{
							PerkHelper.AddPerkBonusForParty(DefaultPerks.OneHanded.UnwaveringDefense, mobileParty, false, ref explainedNumber);
						}
					}
					if (characterObject3.GetPerkValue(DefaultPerks.Medicine.MinisterOfHealth))
					{
						int num = (int)((float)MathF.Max(characterObject3.GetSkillValue(DefaultSkills.Medicine) - Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus, 0) * DefaultPerks.Medicine.MinisterOfHealth.PrimaryBonus);
						if (num > 0)
						{
							explainedNumber.Add((float)num, null, null);
						}
					}
				}
			}
			else
			{
				Agent riderAgent = agent.RiderAgent;
				if (riderAgent != null)
				{
					CharacterObject characterObject4 = ((riderAgent != null) ? riderAgent.Character : null) as CharacterObject;
					object obj;
					if (riderAgent == null)
					{
						obj = null;
					}
					else
					{
						IAgentOriginBase origin = riderAgent.Origin;
						obj = ((origin != null) ? origin.BattleCombatant : null);
					}
					PartyBase partyBase2 = (PartyBase)obj;
					MobileParty mobileParty2 = ((partyBase2 != null) ? partyBase2.MobileParty : null);
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.Sledges, mobileParty2, false, ref explainedNumber);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.Veterinary, characterObject4, true, ref explainedNumber);
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Riding.Veterinary, mobileParty2, false, ref explainedNumber);
				}
			}
			return explainedNumber.ResultNumber;
		}

		public override float GetEnvironmentSpeedFactor(Agent agent)
		{
			Scene scene = agent.Mission.Scene;
			float num = 1f;
			if (!agent.Mission.Scene.IsAtmosphereIndoor)
			{
				if (agent.Mission.Scene.GetRainDensity() > 0f)
				{
					num *= 0.9f;
				}
				if (!agent.IsHuman && CampaignTime.Now.IsNightTime)
				{
					num *= 0.9f;
				}
			}
			return num;
		}

		private string GetDebugPerkInfoForAgent(CharacterObject agentCharacter, bool filterPerkRole = false, SkillEffect.PerkRole perkRole = 12)
		{
			string text = "";
			string text2 = "{0,-18}";
			if (this.GetPerksOfAgent(agentCharacter, null, filterPerkRole, perkRole).Count > 0)
			{
				foreach (SkillObject skillObject in Skills.All)
				{
					List<PerkObject> perksOfAgent = this.GetPerksOfAgent(agentCharacter, skillObject, filterPerkRole, perkRole);
					if (perksOfAgent != null && perksOfAgent.Count > 0)
					{
						string text3 = string.Format(text2, skillObject.Name) + ": ";
						int num = 5;
						int num2 = 0;
						foreach (PerkObject perkObject in perksOfAgent)
						{
							string text4 = perkObject.Name.ToString();
							if (num2 == num)
							{
								text3 = text3 + "\n" + string.Format(text2, "") + "  ";
								num2 = 0;
							}
							text3 = text3 + text4 + ", ";
							num2++;
						}
						text3 = text3.Remove(text3.LastIndexOf(","));
						text = text + text3 + "\n";
					}
				}
			}
			return text;
		}

		private void UpdateHumanStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			Equipment spawnEquipment = agent.SpawnEquipment;
			agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum();
			agentDrivenProperties.ArmorTorso = spawnEquipment.GetHumanBodyArmorSum();
			agentDrivenProperties.ArmorLegs = spawnEquipment.GetLegArmorSum();
			agentDrivenProperties.ArmorArms = spawnEquipment.GetArmArmorSum();
			BasicCharacterObject character = agent.Character;
			CharacterObject characterObject = character as CharacterObject;
			MissionEquipment equipment = agent.Equipment;
			float num = equipment.GetTotalWeightOfWeapons();
			float effectiveArmorEncumbrance = this.GetEffectiveArmorEncumbrance(agent, spawnEquipment);
			int weight = agent.Monster.Weight;
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(0);
			EquipmentIndex wieldedItemIndex2 = agent.GetWieldedItemIndex(1);
			if (wieldedItemIndex != -1)
			{
				ItemObject item = equipment[wieldedItemIndex].Item;
				WeaponComponent weaponComponent = item.WeaponComponent;
				if (weaponComponent != null)
				{
					ItemObject.ItemTypeEnum itemType = weaponComponent.GetItemType();
					bool flag = false;
					if (characterObject != null)
					{
						bool flag2 = itemType == 8 && characterObject.GetPerkValue(DefaultPerks.Bow.RangersSwiftness);
						bool flag3 = itemType == 9 && characterObject.GetPerkValue(DefaultPerks.Crossbow.LooseAndMove);
						flag = flag2 || flag3;
					}
					if (!flag)
					{
						float realWeaponLength = weaponComponent.PrimaryWeapon.GetRealWeaponLength();
						num += 4f * MathF.Sqrt(realWeaponLength) * item.Weight;
					}
				}
			}
			if (wieldedItemIndex2 != -1)
			{
				ItemObject item2 = equipment[wieldedItemIndex2].Item;
				WeaponComponentData primaryWeapon = item2.PrimaryWeapon;
				if (primaryWeapon != null && Extensions.HasAnyFlag<WeaponFlags>(primaryWeapon.WeaponFlags, 268435456L) && (characterObject == null || !characterObject.GetPerkValue(DefaultPerks.OneHanded.ShieldBearer)))
				{
					num += 1.5f * item2.Weight;
				}
			}
			agentDrivenProperties.WeaponsEncumbrance = num;
			agentDrivenProperties.ArmorEncumbrance = effectiveArmorEncumbrance;
			float num2 = effectiveArmorEncumbrance + num;
			EquipmentIndex wieldedItemIndex3 = agent.GetWieldedItemIndex(0);
			WeaponComponentData weaponComponentData = ((wieldedItemIndex3 != -1) ? equipment[wieldedItemIndex3].CurrentUsageItem : null);
			EquipmentIndex wieldedItemIndex4 = agent.GetWieldedItemIndex(1);
			WeaponComponentData weaponComponentData2 = ((wieldedItemIndex4 != -1) ? equipment[wieldedItemIndex4].CurrentUsageItem : null);
			agentDrivenProperties.SwingSpeedMultiplier = 0.93f;
			agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = 0.93f;
			agentDrivenProperties.HandlingMultiplier = 1f;
			agentDrivenProperties.ShieldBashStunDurationMultiplier = 1f;
			agentDrivenProperties.KickStunDurationMultiplier = 1f;
			agentDrivenProperties.ReloadSpeed = 0.93f;
			agentDrivenProperties.MissileSpeedMultiplier = 1f;
			agentDrivenProperties.ReloadMovementPenaltyFactor = 1f;
			base.SetAllWeaponInaccuracy(agent, agentDrivenProperties, wieldedItemIndex3, weaponComponentData);
			IAgentOriginBase origin = agent.Origin;
			Formation formation = agent.Formation;
			int effectiveSkill = this.GetEffectiveSkill(agent, DefaultSkills.Athletics);
			int effectiveSkill2 = this.GetEffectiveSkill(agent, DefaultSkills.Riding);
			if (weaponComponentData != null)
			{
				WeaponComponentData weaponComponentData3 = weaponComponentData;
				int effectiveSkillForWeapon = this.GetEffectiveSkillForWeapon(agent, weaponComponentData3);
				if (weaponComponentData3.IsRangedWeapon)
				{
					int thrustSpeed = weaponComponentData3.ThrustSpeed;
					if (!agent.HasMount)
					{
						float num3 = MathF.Max(0f, 1f - (float)effectiveSkillForWeapon / 500f);
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = MathF.Max(0f, 0.125f * num3);
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = MathF.Max(0f, 0.1f * num3);
					}
					else
					{
						float num4 = MathF.Max(0f, (1f - (float)effectiveSkillForWeapon / 500f) * (1f - (float)effectiveSkill2 / 1800f));
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = MathF.Max(0f, 0.025f * num4);
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = MathF.Max(0f, 0.12f * num4);
					}
					if (weaponComponentData3.RelevantSkill == DefaultSkills.Bow)
					{
						float num5 = ((float)thrustSpeed - 45f) / 90f;
						num5 = MBMath.ClampFloat(num5, 0f, 1f);
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 6f;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 4.5f / MBMath.Lerp(0.75f, 2f, num5, 1E-05f);
					}
					else if (weaponComponentData3.RelevantSkill == DefaultSkills.Throwing)
					{
						float num6 = ((float)thrustSpeed - 89f) / 13f;
						num6 = MBMath.ClampFloat(num6, 0f, 1f);
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 3.5f * MBMath.Lerp(1.5f, 0.8f, num6, 1E-05f);
					}
					else if (weaponComponentData3.RelevantSkill == DefaultSkills.Crossbow)
					{
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 2.5f;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 1.2f;
					}
					if (weaponComponentData3.WeaponClass == 15)
					{
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.3f + (95.75f - (float)thrustSpeed) * 0.005f;
						float num7 = ((float)thrustSpeed - 45f) / 90f;
						num7 = MBMath.ClampFloat(num7, 0f, 1f);
						agentDrivenProperties.WeaponUnsteadyBeginTime = 0.6f + (float)effectiveSkillForWeapon * 0.01f * MBMath.Lerp(2f, 4f, num7, 1E-05f);
						if (agent.IsAIControlled)
						{
							agentDrivenProperties.WeaponUnsteadyBeginTime *= 4f;
						}
						agentDrivenProperties.WeaponUnsteadyEndTime = 2f + agentDrivenProperties.WeaponUnsteadyBeginTime;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
					}
					else if (weaponComponentData3.WeaponClass == 21 || weaponComponentData3.WeaponClass == 19 || weaponComponentData3.WeaponClass == 20)
					{
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.4f + (89f - (float)thrustSpeed) * 0.03f;
						agentDrivenProperties.WeaponUnsteadyBeginTime = 2.5f + (float)effectiveSkillForWeapon * 0.01f;
						agentDrivenProperties.WeaponUnsteadyEndTime = 10f + agentDrivenProperties.WeaponUnsteadyBeginTime;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.025f;
					}
					else
					{
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.1f;
						agentDrivenProperties.WeaponUnsteadyBeginTime = 0f;
						agentDrivenProperties.WeaponUnsteadyEndTime = 0f;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
					}
				}
				else if (Extensions.HasAllFlags<WeaponFlags>(weaponComponentData3.WeaponFlags, 64L))
				{
					agentDrivenProperties.WeaponUnsteadyBeginTime = 1f + (float)effectiveSkillForWeapon * 0.005f;
					agentDrivenProperties.WeaponUnsteadyEndTime = 3f + (float)effectiveSkillForWeapon * 0.01f;
				}
			}
			agentDrivenProperties.TopSpeedReachDuration = 2.5f + MathF.Max(5f - (1f + (float)effectiveSkill * 0.01f), 1f) / 3.5f - MathF.Min((float)weight / ((float)weight + num2), 0.8f);
			float num8 = 0.7f * (1f + 3f * DefaultSkillEffects.AthleticsSpeedFactor.PrimaryBonus);
			float num9 = (num8 - 0.7f) / 300f;
			float num10 = 0.7f + num9 * (float)effectiveSkill;
			float num11 = MathF.Max(0.2f * (1f - DefaultSkillEffects.AthleticsWeightFactor.GetPrimaryValue(effectiveSkill) * 0.01f), 0f) * num2 / (float)weight;
			float num12 = MBMath.ClampFloat(num10 - num11, 0f, num8);
			agentDrivenProperties.MaxSpeedMultiplier = this.GetEnvironmentSpeedFactor(agent) * num12;
			float managedParameter = ManagedParameters.Instance.GetManagedParameter(5);
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(6);
			float num13 = MathF.Min(num2 / (float)weight, 1f);
			agentDrivenProperties.CombatMaxSpeedMultiplier = MathF.Min(MBMath.Lerp(managedParameter2, managedParameter, num13, 1E-05f), 1f);
			agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder = 0.3f;
			Agent mountAgent = agent.MountAgent;
			float num14 = ((mountAgent != null) ? mountAgent.GetAgentDrivenPropertyValue(68) : 1f);
			agentDrivenProperties.AttributeRiding = (float)effectiveSkill2 * num14;
			agentDrivenProperties.AttributeHorseArchery = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateHorseArcheryFactor(character);
			agentDrivenProperties.BipedalRangedReadySpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(7);
			agentDrivenProperties.BipedalRangedReloadSpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(8);
			this.GetSkillEffectsOnAgent(agent, agentDrivenProperties, weaponComponentData);
			this.GetPerkAndBannerEffectsOnAgent(agent, agentDrivenProperties, weaponComponentData);
			base.SetAiRelatedProperties(agent, agentDrivenProperties, weaponComponentData, weaponComponentData2);
			float num15 = 1f;
			if (!agent.Mission.Scene.IsAtmosphereIndoor)
			{
				float rainDensity = agent.Mission.Scene.GetRainDensity();
				float fog = agent.Mission.Scene.GetFog();
				if (rainDensity > 0f || fog > 0f)
				{
					num15 += MathF.Min(0.3f, rainDensity + fog);
				}
				if (!MBMath.IsBetween(agent.Mission.Scene.TimeOfDay, 4f, 20.01f))
				{
					num15 += 0.1f;
				}
			}
			agentDrivenProperties.AiShooterError *= num15;
		}

		private void UpdateHorseStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			Equipment spawnEquipment = agent.SpawnEquipment;
			EquipmentElement equipmentElement = spawnEquipment[10];
			ItemObject item = equipmentElement.Item;
			EquipmentElement equipmentElement2 = spawnEquipment[11];
			agentDrivenProperties.AiSpeciesIndex = (int)item.Id.InternalValue;
			agentDrivenProperties.AttributeRiding = 0.8f + ((equipmentElement2.Item != null) ? 0.2f : 0f);
			float num = 0f;
			for (int i = 1; i < 12; i++)
			{
				if (spawnEquipment[i].Item != null)
				{
					num += (float)spawnEquipment[i].GetModifiedMountBodyArmor();
				}
			}
			agentDrivenProperties.ArmorTorso = num;
			int modifiedMountManeuver = equipmentElement.GetModifiedMountManeuver(ref equipmentElement2);
			int num2 = equipmentElement.GetModifiedMountSpeed(ref equipmentElement2) + 1;
			int num3 = 0;
			float environmentSpeedFactor = this.GetEnvironmentSpeedFactor(agent);
			bool flag = Campaign.Current.Models.MapWeatherModel.GetWeatherEffectOnTerrainForPosition(MobileParty.MainParty.Position2D) == 1;
			Agent riderAgent = agent.RiderAgent;
			if (riderAgent != null)
			{
				CharacterObject characterObject = riderAgent.Character as CharacterObject;
				Formation formation = riderAgent.Formation;
				Agent agent2 = ((formation != null) ? formation.Captain : null);
				if (agent2 == riderAgent)
				{
					agent2 = null;
				}
				CharacterObject characterObject2 = ((agent2 != null) ? agent2.Character : null) as CharacterObject;
				BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(formation);
				ExplainedNumber explainedNumber;
				explainedNumber..ctor((float)modifiedMountManeuver, false, null);
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor((float)num2, false, null);
				num3 = this.GetEffectiveSkill(agent.RiderAgent, DefaultSkills.Riding);
				SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Riding, DefaultSkillEffects.HorseManeuver, agent.RiderAgent.Character as CharacterObject, ref explainedNumber, num3, true, 0);
				SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Riding, DefaultSkillEffects.HorseSpeed, agent.RiderAgent.Character as CharacterObject, ref explainedNumber2, num3, true, 0);
				if (activeBanner != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedMountMovementSpeed, activeBanner, ref explainedNumber2);
				}
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.NimbleSteed, characterObject, true, ref explainedNumber);
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.SweepingWind, characterObject, true, ref explainedNumber2);
				ExplainedNumber explainedNumber3;
				explainedNumber3..ctor(agentDrivenProperties.ArmorTorso, false, null);
				PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.ToughSteed, characterObject2, ref explainedNumber3);
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.ToughSteed, characterObject, true, ref explainedNumber3);
				if (characterObject.GetPerkValue(DefaultPerks.Riding.TheWayOfTheSaddle))
				{
					float num4 = (float)MathF.Max(num3 - Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus, 0) * DefaultPerks.Riding.TheWayOfTheSaddle.PrimaryBonus;
					explainedNumber.Add(num4, null, null);
				}
				if (equipmentElement2.Item == null)
				{
					explainedNumber.AddFactor(-0.1f, null);
					explainedNumber2.AddFactor(-0.1f, null);
				}
				if (flag)
				{
					explainedNumber2.AddFactor(-0.25f, null);
				}
				agentDrivenProperties.ArmorTorso = explainedNumber3.ResultNumber;
				agentDrivenProperties.MountManeuver = explainedNumber.ResultNumber;
				agentDrivenProperties.MountSpeed = environmentSpeedFactor * 0.22f * (1f + explainedNumber2.ResultNumber);
			}
			else
			{
				agentDrivenProperties.MountManeuver = (float)modifiedMountManeuver;
				agentDrivenProperties.MountSpeed = environmentSpeedFactor * 0.22f * (float)(1 + num2);
			}
			float num5 = equipmentElement.Weight / 2f + (equipmentElement2.IsEmpty ? 0f : equipmentElement2.Weight);
			agentDrivenProperties.MountDashAccelerationMultiplier = ((num5 > 200f) ? ((num5 < 300f) ? (1f - (num5 - 200f) / 111f) : 0.1f) : 1f);
			if (flag)
			{
				agentDrivenProperties.MountDashAccelerationMultiplier *= 0.75f;
			}
			agentDrivenProperties.TopSpeedReachDuration = Game.Current.BasicModels.RidingModel.CalculateAcceleration(ref equipmentElement, ref equipmentElement2, num3);
			agentDrivenProperties.MountChargeDamage = (float)equipmentElement.GetModifiedMountCharge(ref equipmentElement2) * 0.004f;
			agentDrivenProperties.MountDifficulty = (float)equipmentElement.Item.Difficulty;
		}

		private void GetPerkAndBannerEffectsOnAgent(Agent agent, AgentDrivenProperties agentDrivenProperties, WeaponComponentData rightHandEquippedItem)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			if (characterObject != null)
			{
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
				CharacterObject characterObject2 = obj as CharacterObject;
				Formation formation2 = agent.Formation;
				if (((formation2 != null) ? formation2.Captain : null) == agent)
				{
					characterObject2 = null;
				}
				ItemObject itemObject = null;
				EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(1);
				if (wieldedItemIndex != -1)
				{
					itemObject = agent.Equipment[wieldedItemIndex].Item;
				}
				BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(agent.Formation);
				bool flag = rightHandEquippedItem != null && rightHandEquippedItem.IsRangedWeapon;
				bool flag2 = rightHandEquippedItem != null && rightHandEquippedItem.IsMeleeWeapon;
				bool flag3 = itemObject != null && itemObject.PrimaryWeapon.IsShield;
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(agentDrivenProperties.CombatMaxSpeedMultiplier, false, null);
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(agentDrivenProperties.MaxSpeedMultiplier, false, null);
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.FleetOfFoot, characterObject, true, ref explainedNumber);
				ExplainedNumber explainedNumber3;
				explainedNumber3..ctor(agentDrivenProperties.KickStunDurationMultiplier, false, null);
				PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Roguery.DirtyFighting, characterObject, true, ref explainedNumber3);
				agentDrivenProperties.KickStunDurationMultiplier = explainedNumber3.ResultNumber;
				if (rightHandEquippedItem != null)
				{
					ExplainedNumber explainedNumber4;
					explainedNumber4..ctor(agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier, false, null);
					if (flag2)
					{
						ExplainedNumber explainedNumber5;
						explainedNumber5..ctor(agentDrivenProperties.SwingSpeedMultiplier, false, null);
						ExplainedNumber explainedNumber6;
						explainedNumber6..ctor(agentDrivenProperties.HandlingMultiplier, false, null);
						if (!agent.HasMount)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.Fury, characterObject, true, ref explainedNumber6);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.Fury, characterObject2, ref explainedNumber6);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.OnTheEdge, characterObject2, ref explainedNumber5);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.BladeMaster, characterObject2, ref explainedNumber5);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.SwiftSwing, characterObject2, ref explainedNumber5);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.BladeMaster, characterObject2, ref explainedNumber4);
							}
						}
						if (rightHandEquippedItem.RelevantSkill == DefaultSkills.OneHanded)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.SwiftStrike, characterObject, true, ref explainedNumber5);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.OneHanded.WayOfTheSword, characterObject, DefaultSkills.OneHanded, true, ref explainedNumber5, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.OneHanded.WayOfTheSword, characterObject, DefaultSkills.OneHanded, true, ref explainedNumber4, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.WrappedHandles, characterObject, true, ref explainedNumber6);
						}
						else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.TwoHanded)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.OnTheEdge, characterObject, true, ref explainedNumber5);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.TwoHanded.WayOfTheGreatAxe, characterObject, DefaultSkills.TwoHanded, true, ref explainedNumber5, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.TwoHanded.WayOfTheGreatAxe, characterObject, DefaultSkills.TwoHanded, true, ref explainedNumber4, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.StrongGrip, characterObject, true, ref explainedNumber6);
						}
						else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Polearm)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.Footwork, characterObject, true, ref explainedNumber);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.SwiftSwing, characterObject, true, ref explainedNumber5);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Polearm.WayOfTheSpear, characterObject, DefaultSkills.Polearm, true, ref explainedNumber5, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Polearm.WayOfTheSpear, characterObject, DefaultSkills.Polearm, true, ref explainedNumber4, Campaign.Current.Models.CharacterDevelopmentModel.MaxSkillRequiredForEpicPerkBonus);
							if (rightHandEquippedItem.SwingDamageType != -1)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.CounterWeight, characterObject, true, ref explainedNumber6);
							}
						}
						agentDrivenProperties.SwingSpeedMultiplier = explainedNumber5.ResultNumber;
						agentDrivenProperties.HandlingMultiplier = explainedNumber6.ResultNumber;
					}
					if (flag)
					{
						ExplainedNumber explainedNumber7;
						explainedNumber7..ctor(agentDrivenProperties.WeaponInaccuracy, false, null);
						ExplainedNumber explainedNumber8;
						explainedNumber8..ctor(agentDrivenProperties.WeaponMaxMovementAccuracyPenalty, false, null);
						ExplainedNumber explainedNumber9;
						explainedNumber9..ctor(agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty, false, null);
						ExplainedNumber explainedNumber10;
						explainedNumber10..ctor(agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians, false, null);
						ExplainedNumber explainedNumber11;
						explainedNumber11..ctor(agentDrivenProperties.WeaponUnsteadyBeginTime, false, null);
						ExplainedNumber explainedNumber12;
						explainedNumber12..ctor(agentDrivenProperties.WeaponUnsteadyEndTime, false, null);
						ExplainedNumber explainedNumber13;
						explainedNumber13..ctor(agentDrivenProperties.ReloadMovementPenaltyFactor, false, null);
						ExplainedNumber explainedNumber14;
						explainedNumber14..ctor(agentDrivenProperties.ReloadSpeed, false, null);
						ExplainedNumber explainedNumber15;
						explainedNumber15..ctor(agentDrivenProperties.MissileSpeedMultiplier, false, null);
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.NockingPoint, characterObject, true, ref explainedNumber13);
						if (characterObject2 != null)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.LooseAndMove, characterObject2, ref explainedNumber2);
						}
						if (activeBanner != null)
						{
							BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedRangedAccuracyPenalty, activeBanner, ref explainedNumber7);
						}
						if (agent.HasMount)
						{
							if (characterObject.GetPerkValue(DefaultPerks.Riding.Sagittarius))
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.Sagittarius, characterObject, true, ref explainedNumber8);
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Riding.Sagittarius, characterObject, true, ref explainedNumber9);
							}
							if (characterObject2 != null && characterObject2.GetPerkValue(DefaultPerks.Riding.Sagittarius))
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.Sagittarius, characterObject2, ref explainedNumber8);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.Sagittarius, characterObject2, ref explainedNumber9);
							}
							if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Bow && characterObject.GetPerkValue(DefaultPerks.Bow.MountedArchery))
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.MountedArchery, characterObject, true, ref explainedNumber8);
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.MountedArchery, characterObject, true, ref explainedNumber9);
							}
							if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Throwing && characterObject.GetPerkValue(DefaultPerks.Throwing.MountedSkirmisher))
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.MountedSkirmisher, characterObject, true, ref explainedNumber8);
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.MountedSkirmisher, characterObject, true, ref explainedNumber9);
							}
						}
						bool flag4 = false;
						if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Bow)
						{
							flag4 = true;
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.BowControl, characterObject, true, ref explainedNumber8);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.RapidFire, characterObject, true, ref explainedNumber14);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.QuickAdjustments, characterObject, true, ref explainedNumber10);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.Discipline, characterObject, true, ref explainedNumber11);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.Discipline, characterObject, true, ref explainedNumber12);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.QuickDraw, characterObject, true, ref explainedNumber4);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.RapidFire, characterObject2, ref explainedNumber14);
								if (!agent.HasMount)
								{
									PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.NockingPoint, characterObject2, ref explainedNumber2);
								}
							}
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Bow.Deadshot, characterObject, DefaultSkills.Bow, true, ref explainedNumber14, Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus);
						}
						else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Crossbow)
						{
							flag4 = true;
							if (agent.HasMount)
							{
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Steady, characterObject, true, ref explainedNumber8);
								PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Steady, characterObject, true, ref explainedNumber10);
							}
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.WindWinder, characterObject, true, ref explainedNumber14);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.WindWinder, characterObject2, ref explainedNumber14);
							}
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.DonkeysSwiftness, characterObject, true, ref explainedNumber8);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Marksmen, characterObject, true, ref explainedNumber4);
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Crossbow.MightyPull, characterObject, DefaultSkills.Crossbow, true, ref explainedNumber14, Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus);
						}
						else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Throwing)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.QuickDraw, characterObject, true, ref explainedNumber14);
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.PerfectTechnique, characterObject, true, ref explainedNumber15);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.QuickDraw, characterObject2, ref explainedNumber14);
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.PerfectTechnique, characterObject2, ref explainedNumber15);
							}
							PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Throwing.UnstoppableForce, characterObject, DefaultSkills.Throwing, true, ref explainedNumber15, Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus);
						}
						if (flag4 && Campaign.Current.Models.MapWeatherModel.GetWeatherEffectOnTerrainForPosition(MobileParty.MainParty.Position2D) == 1)
						{
							explainedNumber15.AddFactor(-0.2f, null);
						}
						agentDrivenProperties.ReloadMovementPenaltyFactor = explainedNumber13.ResultNumber;
						agentDrivenProperties.ReloadSpeed = explainedNumber14.ResultNumber;
						agentDrivenProperties.MissileSpeedMultiplier = explainedNumber15.ResultNumber;
						agentDrivenProperties.WeaponInaccuracy = explainedNumber7.ResultNumber;
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = explainedNumber8.ResultNumber;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = explainedNumber9.ResultNumber;
						agentDrivenProperties.WeaponUnsteadyBeginTime = explainedNumber11.ResultNumber;
						agentDrivenProperties.WeaponUnsteadyEndTime = explainedNumber12.ResultNumber;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = explainedNumber10.ResultNumber;
					}
					agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = explainedNumber4.ResultNumber;
				}
				if (flag3)
				{
					ExplainedNumber explainedNumber16;
					explainedNumber16..ctor(agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder, false, null);
					if (characterObject2 != null)
					{
						Formation formation3 = agent.Formation;
						if (formation3 != null && formation3.ArrangementOrder.OrderEnum == 5)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.ShieldWall, characterObject2, ref explainedNumber16);
						}
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.ArrowCatcher, characterObject2, ref explainedNumber16);
					}
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.ArrowCatcher, characterObject, true, ref explainedNumber16);
					agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder = explainedNumber16.ResultNumber;
					ExplainedNumber explainedNumber17;
					explainedNumber17..ctor(agentDrivenProperties.ShieldBashStunDurationMultiplier, false, null);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.Basher, characterObject, true, ref explainedNumber17);
					agentDrivenProperties.ShieldBashStunDurationMultiplier = explainedNumber17.ResultNumber;
				}
				else
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.MorningExercise, characterObject, true, ref explainedNumber2);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.SelfMedication, characterObject, false, ref explainedNumber2);
					if (!flag3 && !flag)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.Sprint, characterObject, true, ref explainedNumber2);
					}
					if (rightHandEquippedItem == null && itemObject == null)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Roguery.FleetFooted, characterObject, true, ref explainedNumber2);
					}
					if (characterObject2 != null)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.MorningExercise, characterObject2, ref explainedNumber2);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.ShieldBearer, characterObject2, ref explainedNumber2);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.OneHanded.FleetOfFoot, characterObject2, ref explainedNumber2);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.TwoHanded.RecklessCharge, characterObject2, ref explainedNumber2);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Footwork, characterObject2, ref explainedNumber2);
						if (characterObject.Tier >= 3)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.FormFittingArmor, characterObject2, ref explainedNumber2);
						}
						if (characterObject.IsInfantry)
						{
							PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.Sprint, characterObject2, ref explainedNumber2);
						}
					}
				}
				if (agent.IsHero)
				{
					ItemObject item = (Mission.Current.DoesMissionRequireCivilianEquipment ? characterObject.FirstCivilianEquipment : characterObject.FirstBattleEquipment)[6].Item;
					if (item != null && item.IsCivilian && characterObject.GetPerkValue(DefaultPerks.Roguery.SmugglerConnections))
					{
						agentDrivenProperties.ArmorTorso += DefaultPerks.Roguery.SmugglerConnections.PrimaryBonus;
					}
				}
				float num = 0f;
				float num2 = 0f;
				bool flag5 = false;
				if (characterObject2 != null)
				{
					if (agent.HasMount && characterObject2.GetPerkValue(DefaultPerks.Riding.DauntlessSteed))
					{
						num += DefaultPerks.Riding.DauntlessSteed.SecondaryBonus;
						flag5 = true;
					}
					else if (!agent.HasMount && characterObject2.GetPerkValue(DefaultPerks.Athletics.IgnorePain))
					{
						num += DefaultPerks.Athletics.IgnorePain.SecondaryBonus;
						flag5 = true;
					}
					if (characterObject2.GetPerkValue(DefaultPerks.Engineering.Metallurgy))
					{
						num += DefaultPerks.Engineering.Metallurgy.SecondaryBonus;
						flag5 = true;
					}
				}
				if (!agent.HasMount && characterObject.GetPerkValue(DefaultPerks.Athletics.IgnorePain))
				{
					num2 += DefaultPerks.Athletics.IgnorePain.PrimaryBonus;
					flag5 = true;
				}
				if (flag5)
				{
					float num3 = 1f + num2;
					agentDrivenProperties.ArmorHead = MathF.Max(0f, (agentDrivenProperties.ArmorHead + num) * num3);
					agentDrivenProperties.ArmorTorso = MathF.Max(0f, (agentDrivenProperties.ArmorTorso + num) * num3);
					agentDrivenProperties.ArmorArms = MathF.Max(0f, (agentDrivenProperties.ArmorArms + num) * num3);
					agentDrivenProperties.ArmorLegs = MathF.Max(0f, (agentDrivenProperties.ArmorLegs + num) * num3);
				}
				if (Mission.Current != null && Mission.Current.HasValidTerrainType)
				{
					TerrainType terrainType = Mission.Current.TerrainType;
					if (terrainType == 2 || terrainType == 10)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.ExtendedSkirmish, characterObject2, ref explainedNumber2);
					}
					else if (terrainType == 4 || terrainType == 3 || terrainType == 5)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.DecisiveBattle, characterObject2, ref explainedNumber2);
					}
				}
				if (characterObject.Tier >= 3 && characterObject.IsInfantry)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.FormFittingArmor, characterObject2, ref explainedNumber2);
				}
				if (agent.Formation != null && agent.Formation.CountOfUnits <= 15)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Tactics.SmallUnitTactics, characterObject2, ref explainedNumber2);
				}
				if (activeBanner != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedTroopMovementSpeed, activeBanner, ref explainedNumber2);
				}
				agentDrivenProperties.MaxSpeedMultiplier = explainedNumber2.ResultNumber;
				agentDrivenProperties.CombatMaxSpeedMultiplier = explainedNumber.ResultNumber;
			}
		}

		private void GetSkillEffectsOnAgent(Agent agent, AgentDrivenProperties agentDrivenProperties, WeaponComponentData rightHandEquippedItem)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			float swingSpeedMultiplier = agentDrivenProperties.SwingSpeedMultiplier;
			float thrustOrRangedReadySpeedMultiplier = agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier;
			float reloadSpeed = agentDrivenProperties.ReloadSpeed;
			if (characterObject != null && rightHandEquippedItem != null)
			{
				int effectiveSkill = this.GetEffectiveSkill(agent, rightHandEquippedItem.RelevantSkill);
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(swingSpeedMultiplier, false, null);
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(thrustOrRangedReadySpeedMultiplier, false, null);
				ExplainedNumber explainedNumber3;
				explainedNumber3..ctor(reloadSpeed, false, null);
				if (rightHandEquippedItem.RelevantSkill == DefaultSkills.OneHanded)
				{
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.OneHanded, DefaultSkillEffects.OneHandedSpeed, characterObject, ref explainedNumber, effectiveSkill, true, 0);
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.OneHanded, DefaultSkillEffects.OneHandedSpeed, characterObject, ref explainedNumber2, effectiveSkill, true, 0);
				}
				else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.TwoHanded)
				{
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.TwoHanded, DefaultSkillEffects.TwoHandedSpeed, characterObject, ref explainedNumber, effectiveSkill, true, 0);
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.TwoHanded, DefaultSkillEffects.TwoHandedSpeed, characterObject, ref explainedNumber2, effectiveSkill, true, 0);
				}
				else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Polearm)
				{
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Polearm, DefaultSkillEffects.PolearmSpeed, characterObject, ref explainedNumber, effectiveSkill, true, 0);
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Polearm, DefaultSkillEffects.PolearmSpeed, characterObject, ref explainedNumber2, effectiveSkill, true, 0);
				}
				else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Crossbow)
				{
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Crossbow, DefaultSkillEffects.CrossbowReloadSpeed, characterObject, ref explainedNumber3, effectiveSkill, true, 0);
				}
				else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Throwing)
				{
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Throwing, DefaultSkillEffects.ThrowingSpeed, characterObject, ref explainedNumber2, effectiveSkill, true, 0);
				}
				if (agent.HasMount)
				{
					int effectiveSkill2 = this.GetEffectiveSkill(agent, DefaultSkills.Riding);
					float num = -0.01f * MathF.Max(0f, DefaultSkillEffects.HorseWeaponSpeedPenalty.GetPrimaryValue(effectiveSkill2));
					explainedNumber.AddFactor(num, null);
					explainedNumber2.AddFactor(num, null);
					explainedNumber3.AddFactor(num, null);
				}
				agentDrivenProperties.SwingSpeedMultiplier = explainedNumber.ResultNumber;
				agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = explainedNumber2.ResultNumber;
				agentDrivenProperties.ReloadSpeed = explainedNumber3.ResultNumber;
			}
		}

		public static float CalculateMaximumSpeedMultiplier(int athletics, float baseWeight, float totalEncumbrance)
		{
			return MathF.Min((200f + (float)athletics) / 300f * (baseWeight * 2f / (baseWeight * 2f + totalEncumbrance)), 1f);
		}
	}
}
