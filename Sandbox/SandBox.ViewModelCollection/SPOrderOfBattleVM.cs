using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace SandBox.ViewModelCollection
{
	public class SPOrderOfBattleVM : OrderOfBattleVM
	{
		public SPOrderOfBattleVM()
		{
			this.RefreshValues();
		}

		protected override void LoadConfiguration()
		{
			base.LoadConfiguration();
			this._orderOfBattleBehavior = Campaign.Current.GetCampaignBehavior<OrderOfBattleCampaignBehavior>();
			base.IsOrderPreconfigured = false;
			if (!base.IsPlayerGeneral)
			{
				return;
			}
			for (int i = 0; i < base.TotalFormationCount; i++)
			{
				OrderOfBattleCampaignBehavior.OrderOfBattleFormationData formationInfo = this._orderOfBattleBehavior.GetFormationDataAtIndex(i, Mission.Current.IsSiegeBattle);
				if (formationInfo != null && formationInfo.FormationClass != null)
				{
					base.IsOrderPreconfigured = true;
					bool flag = formationInfo.PrimaryClassWeight > 0 || formationInfo.SecondaryClassWeight > 0;
					if (formationInfo.FormationClass == 1)
					{
						this._allFormations[i].Classes[0].Class = 0;
					}
					else if (formationInfo.FormationClass == 2)
					{
						this._allFormations[i].Classes[0].Class = 1;
					}
					else if (formationInfo.FormationClass == 3)
					{
						this._allFormations[i].Classes[0].Class = 2;
					}
					else if (formationInfo.FormationClass == 4)
					{
						this._allFormations[i].Classes[0].Class = 3;
					}
					else if (formationInfo.FormationClass == 5)
					{
						this._allFormations[i].Classes[0].Class = 0;
						this._allFormations[i].Classes[1].Class = 1;
					}
					else if (formationInfo.FormationClass == 6)
					{
						this._allFormations[i].Classes[0].Class = 2;
						this._allFormations[i].Classes[1].Class = 3;
					}
					if (flag)
					{
						bool flag2;
						formationInfo.Filters.TryGetValue(1, out flag2);
						bool flag3;
						formationInfo.Filters.TryGetValue(2, out flag3);
						bool flag4;
						formationInfo.Filters.TryGetValue(3, out flag4);
						bool flag5;
						formationInfo.Filters.TryGetValue(4, out flag5);
						bool flag6;
						formationInfo.Filters.TryGetValue(5, out flag6);
						bool flag7;
						formationInfo.Filters.TryGetValue(6, out flag7);
						this._allFormations[i].FilterItems.FirstOrDefault((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 1).IsActive = flag2;
						this._allFormations[i].FilterItems.FirstOrDefault((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 2).IsActive = flag3;
						this._allFormations[i].FilterItems.FirstOrDefault((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 3).IsActive = flag4;
						this._allFormations[i].FilterItems.FirstOrDefault((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 4).IsActive = flag5;
						this._allFormations[i].FilterItems.FirstOrDefault((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 5).IsActive = flag6;
						this._allFormations[i].FilterItems.FirstOrDefault((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 6).IsActive = flag7;
					}
					else
					{
						base.ClearFormationItem(this._allFormations[i]);
					}
					DeploymentFormationClass deploymentFormationClass = formationInfo.FormationClass;
					if (Mission.Current.IsSiegeBattle)
					{
						if (deploymentFormationClass == 4)
						{
							deploymentFormationClass = 2;
						}
						else if (deploymentFormationClass == 3)
						{
							deploymentFormationClass = 1;
						}
						else if (deploymentFormationClass == 6)
						{
							deploymentFormationClass = 5;
						}
					}
					this._allFormations[i].RefreshFormation(this._allFormations[i].Formation, deploymentFormationClass, flag);
					if (flag && formationInfo.Commander != null)
					{
						OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = this._allHeroes.FirstOrDefault((OrderOfBattleHeroItemVM c) => c.Agent.Character == formationInfo.Commander.CharacterObject);
						if (orderOfBattleHeroItemVM != null)
						{
							base.AssignCommander(orderOfBattleHeroItemVM.Agent, this._allFormations[i]);
						}
					}
					if (flag && formationInfo.HeroTroops != null)
					{
						Hero[] heroTroops = formationInfo.HeroTroops;
						for (int j = 0; j < heroTroops.Length; j++)
						{
							Hero heroTroop = heroTroops[j];
							OrderOfBattleHeroItemVM orderOfBattleHeroItemVM2 = this._allHeroes.FirstOrDefault((OrderOfBattleHeroItemVM ht) => ht.Agent.Character == heroTroop.CharacterObject);
							if (orderOfBattleHeroItemVM2 != null)
							{
								this._allFormations[i].AddHeroTroop(orderOfBattleHeroItemVM2);
							}
						}
					}
				}
				else if (formationInfo != null)
				{
					base.ClearFormationItem(this._allFormations[i]);
				}
			}
			for (int k = 0; k < base.TotalFormationCount; k++)
			{
				OrderOfBattleCampaignBehavior.OrderOfBattleFormationData formationDataAtIndex = this._orderOfBattleBehavior.GetFormationDataAtIndex(k, Mission.Current.IsSiegeBattle);
				if (formationDataAtIndex != null && formationDataAtIndex.FormationClass != null)
				{
					if (this._allFormations[k].Classes[0].Class != 10)
					{
						this._allFormations[k].Classes[0].Weight = formationDataAtIndex.PrimaryClassWeight;
					}
					if (this._allFormations[k].Classes[1].Class != 10)
					{
						this._allFormations[k].Classes[1].Weight = formationDataAtIndex.SecondaryClassWeight;
					}
				}
			}
		}

		protected override void SaveConfiguration()
		{
			base.SaveConfiguration();
			bool flag = MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle();
			if (base.IsPlayerGeneral && flag)
			{
				List<OrderOfBattleCampaignBehavior.OrderOfBattleFormationData> list = new List<OrderOfBattleCampaignBehavior.OrderOfBattleFormationData>();
				for (int i = 0; i < base.TotalFormationCount; i++)
				{
					OrderOfBattleFormationItemVM formationItemVM = this._allFormations[i];
					Hero hero = null;
					if (formationItemVM.Commander.Agent != null)
					{
						hero = Hero.FindFirst((Hero h) => h.CharacterObject == formationItemVM.Commander.Agent.Character);
					}
					List<Hero> list2 = (from ht in formationItemVM.HeroTroops
						select Hero.FindFirst((Hero hero) => hero.CharacterObject == ht.Agent.Character) into h
						where h != null
						select h).ToList<Hero>();
					DeploymentFormationClass orderOfBattleClass = formationItemVM.GetOrderOfBattleClass();
					bool flag2 = orderOfBattleClass == 0;
					int num = (flag2 ? 0 : formationItemVM.Classes[0].Weight);
					int num2 = (flag2 ? 0 : formationItemVM.Classes[1].Weight);
					Dictionary<FormationFilterType, bool> dictionary = new Dictionary<FormationFilterType, bool>();
					dictionary[1] = !flag2 && formationItemVM.HasFilter(1);
					dictionary[2] = !flag2 && formationItemVM.HasFilter(2);
					dictionary[3] = !flag2 && formationItemVM.HasFilter(3);
					dictionary[4] = !flag2 && formationItemVM.HasFilter(4);
					dictionary[5] = !flag2 && formationItemVM.HasFilter(5);
					dictionary[6] = !flag2 && formationItemVM.HasFilter(6);
					Dictionary<FormationFilterType, bool> dictionary2 = dictionary;
					list.Add(new OrderOfBattleCampaignBehavior.OrderOfBattleFormationData(hero, list2, orderOfBattleClass, num, num2, dictionary2));
				}
				this._orderOfBattleBehavior.SetFormationInfos(list, Mission.Current.IsSiegeBattle);
			}
		}

		protected override List<TooltipProperty> GetAgentTooltip(Agent agent)
		{
			List<TooltipProperty> agentTooltip = base.GetAgentTooltip(agent);
			if (agent != null)
			{
				Hero hero = Hero.FindFirst((Hero h) => h.StringId == agent.Character.StringId);
				foreach (SkillObject skillObject in Skills.All)
				{
					if (skillObject.StringId == "OneHanded" || skillObject.StringId == "TwoHanded" || skillObject.StringId == "Polearm" || skillObject.StringId == "Bow" || skillObject.StringId == "Crossbow" || skillObject.StringId == "Throwing" || skillObject.StringId == "Riding" || skillObject.StringId == "Athletics" || skillObject.StringId == "Tactics" || skillObject.StringId == "Leadership")
					{
						agentTooltip.Add(new TooltipProperty(skillObject.Name.ToString(), agent.Character.GetSkillValue(skillObject).ToString(), 0, false, 0)
						{
							OnlyShowWhenNotExtended = true
						});
					}
				}
				agentTooltip.Add(new TooltipProperty("", string.Empty, 0, false, 1024)
				{
					OnlyShowWhenNotExtended = true
				});
				List<PerkObject> list;
				float captainRatingForTroopUsages = Campaign.Current.Models.BattleCaptainModel.GetCaptainRatingForTroopUsages(hero, FormationClassExtensions.GetTroopUsageFlags(0), ref list);
				List<PerkObject> list2;
				float captainRatingForTroopUsages2 = Campaign.Current.Models.BattleCaptainModel.GetCaptainRatingForTroopUsages(hero, FormationClassExtensions.GetTroopUsageFlags(1), ref list2);
				List<PerkObject> list3;
				float captainRatingForTroopUsages3 = Campaign.Current.Models.BattleCaptainModel.GetCaptainRatingForTroopUsages(hero, FormationClassExtensions.GetTroopUsageFlags(2), ref list3);
				List<PerkObject> list4;
				float captainRatingForTroopUsages4 = Campaign.Current.Models.BattleCaptainModel.GetCaptainRatingForTroopUsages(hero, FormationClassExtensions.GetTroopUsageFlags(3), ref list4);
				agentTooltip.Add(new TooltipProperty(this._infantryInfluenceText.ToString(), ((int)(captainRatingForTroopUsages * 100f)).ToString(), 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				agentTooltip.Add(new TooltipProperty(this._rangedInfluenceText.ToString(), ((int)(captainRatingForTroopUsages2 * 100f)).ToString(), 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				agentTooltip.Add(new TooltipProperty(this._cavalryInfluenceText.ToString(), ((int)(captainRatingForTroopUsages3 * 100f)).ToString(), 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				agentTooltip.Add(new TooltipProperty(this._horseArcherInfluenceText.ToString(), ((int)(captainRatingForTroopUsages4 * 100f)).ToString(), 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				agentTooltip.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				List<PerkObject> list5 = list.Union(list2).Union(list3).Union(list4)
					.ToList<PerkObject>();
				list5.Sort(this._perkComparer);
				bool flag = list5.Count != 0;
				if (flag)
				{
					SPOrderOfBattleVM.AddPerks(this._captainPerksText, agentTooltip, list5);
				}
				if (!flag)
				{
					agentTooltip.Add(new TooltipProperty(this._noPerksText.ToString(), string.Empty, 0, true, 0));
				}
				if (Input.IsGamepadActive)
				{
					GameTexts.SetVariable("EXTEND_KEY", GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", "MapFollowModifier").ToString());
				}
				else
				{
					GameTexts.SetVariable("EXTEND_KEY", Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString());
				}
				agentTooltip.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_map_tooltip_info", null).ToString(), -1, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
			}
			return agentTooltip;
		}

		private static void AddPerks(TextObject title, List<TooltipProperty> tooltipProperties, List<PerkObject> perks)
		{
			tooltipProperties.Add(new TooltipProperty(title.ToString(), string.Empty, 0, true, 4096));
			foreach (PerkObject perkObject in perks)
			{
				if (perkObject.PrimaryRole == 13 || perkObject.SecondaryRole == 13)
				{
					TextObject textObject = ((perkObject.PrimaryRole == 13) ? perkObject.PrimaryDescription : perkObject.SecondaryDescription);
					string genericImageText = HyperlinkTexts.GetGenericImageText(SandBoxUIHelper.GetSkillMeshId(perkObject.Skill, true), 2);
					SPOrderOfBattleVM._perkDefinitionText.SetTextVariable("PERK_NAME", perkObject.Name).SetTextVariable("SKILL", genericImageText).SetTextVariable("SKILL_LEVEL", perkObject.RequiredSkillValue);
					tooltipProperties.Add(new TooltipProperty(SPOrderOfBattleVM._perkDefinitionText.ToString(), textObject.ToString(), 0, true, 0));
				}
			}
		}

		private OrderOfBattleCampaignBehavior _orderOfBattleBehavior;

		private static readonly TextObject _perkDefinitionText = new TextObject("{=jCdZY3i4}{PERK_NAME} ({SKILL_LEVEL} - {SKILL})", null);

		private readonly TextObject _captainPerksText = new TextObject("{=pgXuyHxH}Captain Perks", null);

		private readonly TextObject _infantryInfluenceText = new TextObject("{=SSLUHH6j}Infantry Influence", null);

		private readonly TextObject _rangedInfluenceText = new TextObject("{=0DMM0agr}Ranged Influence", null);

		private readonly TextObject _cavalryInfluenceText = new TextObject("{=X8i3jZn8}Cavalry Influence", null);

		private readonly TextObject _horseArcherInfluenceText = new TextObject("{=gZIOG0wl}Horse Archer Influence", null);

		private readonly TextObject _noPerksText = new TextObject("{=7yaDnyKb}There is no additional perk influence.", null);

		private readonly SkillObjectComparer _perkComparer = new SkillObjectComparer();
	}
}
