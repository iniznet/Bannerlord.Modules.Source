using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class AgingCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
			CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroComesOfAge));
			CampaignEvents.HeroReachesTeenAgeEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroReachesTeenAge));
			CampaignEvents.HeroGrowsOutOfInfancyEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroGrowsOutOfInfancy));
			CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, new Action<Hero, PerkObject>(this.OnPerkOpened));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Hero, int>>("_extraLivesContainer", ref this._extraLivesContainer);
			dataStore.SyncData<Dictionary<Hero, int>>("_heroesYoungerThanHeroComesOfAge", ref this._heroesYoungerThanHeroComesOfAge);
		}

		private void OnHeroCreated(Hero hero, bool isBornNaturally)
		{
			int num = (int)hero.Age;
			if (num < Campaign.Current.Models.AgeModel.HeroComesOfAge)
			{
				this._heroesYoungerThanHeroComesOfAge.Add(hero, num);
			}
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (this._heroesYoungerThanHeroComesOfAge.ContainsKey(victim))
			{
				this._heroesYoungerThanHeroComesOfAge.Remove(victim);
			}
		}

		private void AddExtraLife(Hero hero)
		{
			if (hero.IsAlive)
			{
				if (this._extraLivesContainer.ContainsKey(hero))
				{
					Dictionary<Hero, int> extraLivesContainer = this._extraLivesContainer;
					extraLivesContainer[hero]++;
					return;
				}
				this._extraLivesContainer.Add(hero, 1);
			}
		}

		private void OnPerkOpened(Hero hero, PerkObject perk)
		{
			if (perk == DefaultPerks.Medicine.CheatDeath)
			{
				this.AddExtraLife(hero);
			}
			if (perk == DefaultPerks.Medicine.HealthAdvise)
			{
				Clan clan = hero.Clan;
				if (((clan != null) ? clan.Leader : null) == hero)
				{
					foreach (Hero hero2 in hero.Clan.Heroes.Where((Hero x) => x.IsAlive))
					{
						this.AddExtraLife(hero2);
					}
				}
			}
		}

		private void DailyTickHero(Hero hero)
		{
			bool flag = (int)CampaignTime.Now.ToDays == this._gameStartDay;
			if (!CampaignOptions.IsLifeDeathCycleDisabled && !flag && !hero.IsTemplate)
			{
				if (hero.IsAlive && hero.CanDie(KillCharacterAction.KillCharacterActionDetail.DiedOfOldAge))
				{
					if (hero.DeathMark != KillCharacterAction.KillCharacterActionDetail.None && (hero.PartyBelongedTo == null || (hero.PartyBelongedTo.MapEvent == null && hero.PartyBelongedTo.SiegeEvent == null)))
					{
						KillCharacterAction.ApplyByDeathMark(hero, false);
					}
					else
					{
						this.IsItTimeOfDeath(hero);
					}
				}
				int num;
				if (this._heroesYoungerThanHeroComesOfAge.TryGetValue(hero, out num))
				{
					int num2 = (int)hero.Age;
					if (num != num2)
					{
						if (num2 >= Campaign.Current.Models.AgeModel.HeroComesOfAge)
						{
							this._heroesYoungerThanHeroComesOfAge.Remove(hero);
							CampaignEventDispatcher.Instance.OnHeroComesOfAge(hero);
						}
						else
						{
							this._heroesYoungerThanHeroComesOfAge[hero] = num2;
							if (num2 == Campaign.Current.Models.AgeModel.BecomeTeenagerAge)
							{
								CampaignEventDispatcher.Instance.OnHeroReachesTeenAge(hero);
							}
							else if (num2 == Campaign.Current.Models.AgeModel.BecomeChildAge)
							{
								CampaignEventDispatcher.Instance.OnHeroGrowsOutOfInfancy(hero);
							}
						}
					}
				}
				if (hero == Hero.MainHero && Hero.IsMainHeroIll && Hero.MainHero.HeroState != Hero.CharacterStates.Dead)
				{
					Campaign.Current.MainHeroIllDays++;
					if (Campaign.Current.MainHeroIllDays > 3)
					{
						Hero.MainHero.HitPoints -= MathF.Ceiling((float)Hero.MainHero.HitPoints * (0.05f * (float)Campaign.Current.MainHeroIllDays));
						if (Hero.MainHero.HitPoints <= 1 && Hero.MainHero.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
						{
							int num3;
							if (this._extraLivesContainer.TryGetValue(Hero.MainHero, out num3))
							{
								if (num3 == 0)
								{
									this.KillMainHeroWithIllness();
									return;
								}
								Campaign.Current.MainHeroIllDays = -1;
								this._extraLivesContainer[Hero.MainHero] = num3 - 1;
								if (this._extraLivesContainer[Hero.MainHero] == 0)
								{
									this._extraLivesContainer.Remove(Hero.MainHero);
									return;
								}
							}
							else
							{
								this.KillMainHeroWithIllness();
							}
						}
					}
				}
			}
		}

		private void KillMainHeroWithIllness()
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			Hero.MainHero.AddDeathMark(null, KillCharacterAction.KillCharacterActionDetail.DiedOfOldAge);
			KillCharacterAction.ApplyByOldAge(Hero.MainHero, true);
		}

		private void OnCharacterCreationIsOver()
		{
			this._gameStartDay = (int)CampaignTime.Now.ToDays;
			if (!CampaignOptions.IsLifeDeathCycleDisabled)
			{
				this.InitializeHeroesYoungerThanHeroComesOfAge();
			}
		}

		private void OnHeroGrowsOutOfInfancy(Hero hero)
		{
			if (hero.Clan != Clan.PlayerClan)
			{
				hero.HeroDeveloper.DeriveSkillsFromTraits(true, null);
			}
		}

		private void OnHeroReachesTeenAge(Hero hero)
		{
			MBEquipmentRoster randomElementInefficiently = Campaign.Current.Models.EquipmentSelectionModel.GetEquipmentRostersForHeroReachesTeenAge(hero).GetRandomElementInefficiently<MBEquipmentRoster>();
			if (randomElementInefficiently != null)
			{
				Equipment randomElementInefficiently2 = randomElementInefficiently.GetCivilianEquipments().GetRandomElementInefficiently<Equipment>();
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, randomElementInefficiently2);
				new Equipment(false).FillFrom(randomElementInefficiently2, false);
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, randomElementInefficiently2);
			}
			else
			{
				Debug.FailedAssert("Cant find child equipment template for " + hero.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\AgingCampaignBehavior.cs", "OnHeroReachesTeenAge", 213);
			}
			if (hero.Clan != Clan.PlayerClan)
			{
				foreach (TraitObject traitObject in DefaultTraits.Personality)
				{
					int num = hero.GetTraitLevel(traitObject);
					if (hero.Father == null && hero.Mother == null)
					{
						hero.SetTraitLevel(traitObject, hero.Template.GetTraitLevel(traitObject));
					}
					else
					{
						float randomFloat = MBRandom.RandomFloat;
						float randomFloat2 = MBRandom.RandomFloat;
						if ((double)randomFloat < 0.2 && hero.Father != null)
						{
							num = hero.Father.GetTraitLevel(traitObject);
						}
						else if ((double)randomFloat < 0.6 && !hero.CharacterObject.IsFemale && hero.Father != null)
						{
							num = hero.Father.GetTraitLevel(traitObject);
						}
						else if ((double)randomFloat < 0.6 && hero.Mother != null)
						{
							num = hero.Mother.GetTraitLevel(traitObject);
						}
						else if ((double)randomFloat < 0.7 && hero.Mother != null)
						{
							num = hero.Mother.GetTraitLevel(traitObject);
						}
						else if ((double)randomFloat2 < 0.3)
						{
							num--;
						}
						else if ((double)randomFloat2 >= 0.7)
						{
							num++;
						}
						num = MBMath.ClampInt(num, traitObject.MinValue, traitObject.MaxValue);
						if (num != hero.GetTraitLevel(traitObject))
						{
							hero.SetTraitLevel(traitObject, num);
						}
					}
				}
				hero.HeroDeveloper.DeriveSkillsFromTraits(true, null);
			}
		}

		private void OnHeroComesOfAge(Hero hero)
		{
			if (hero.HeroState != Hero.CharacterStates.Active)
			{
				bool flag = !hero.IsFemale || hero.Clan == Hero.MainHero.Clan || (hero.Mother != null && !hero.Mother.IsNoncombatant) || (hero.RandomIntWithSeed(17U, 0, 1) == 0 && hero.GetTraitLevel(DefaultTraits.Valor) == 1);
				if (hero.Clan != Clan.PlayerClan)
				{
					foreach (TraitObject traitObject in DefaultTraits.SkillCategories)
					{
						hero.SetTraitLevel(traitObject, 0);
					}
					if (flag)
					{
						hero.SetTraitLevel(DefaultTraits.CavalryFightingSkills, 5);
						int num = MathF.Max(DefaultTraits.Commander.MinValue, 3 + hero.GetTraitLevel(DefaultTraits.Valor) + hero.GetTraitLevel(DefaultTraits.Generosity) + hero.RandomIntWithSeed(18U, -1, 2));
						hero.SetTraitLevel(DefaultTraits.Commander, num);
					}
					int num2 = MathF.Max(DefaultTraits.Manager.MinValue, 3 + hero.GetTraitLevel(DefaultTraits.Honor) + hero.RandomIntWithSeed(19U, -1, 2));
					hero.SetTraitLevel(DefaultTraits.Manager, num2);
					int num3 = MathF.Max(DefaultTraits.Politician.MinValue, 3 + hero.GetTraitLevel(DefaultTraits.Calculating) + hero.RandomIntWithSeed(20U, -1, 2));
					hero.SetTraitLevel(DefaultTraits.Politician, num3);
					hero.HeroDeveloper.DeriveSkillsFromTraits(true, null);
				}
				else
				{
					hero.HeroDeveloper.SetInitialLevel(hero.Level);
				}
				MBList<MBEquipmentRoster> equipmentRostersForHeroComeOfAge = Campaign.Current.Models.EquipmentSelectionModel.GetEquipmentRostersForHeroComeOfAge(hero, false);
				MBList<MBEquipmentRoster> equipmentRostersForHeroComeOfAge2 = Campaign.Current.Models.EquipmentSelectionModel.GetEquipmentRostersForHeroComeOfAge(hero, true);
				MBEquipmentRoster randomElement = equipmentRostersForHeroComeOfAge.GetRandomElement<MBEquipmentRoster>();
				MBEquipmentRoster randomElement2 = equipmentRostersForHeroComeOfAge2.GetRandomElement<MBEquipmentRoster>();
				Equipment randomElement3 = randomElement.AllEquipments.GetRandomElement<Equipment>();
				Equipment randomElement4 = randomElement2.AllEquipments.GetRandomElement<Equipment>();
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, randomElement3);
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, randomElement4);
			}
		}

		private void IsItTimeOfDeath(Hero hero)
		{
			if (hero.IsAlive && hero.Age >= (float)Campaign.Current.Models.AgeModel.BecomeOldAge && !CampaignOptions.IsLifeDeathCycleDisabled && hero.DeathMark == KillCharacterAction.KillCharacterActionDetail.None && MBRandom.RandomFloat < hero.ProbabilityOfDeath)
			{
				int num;
				if (this._extraLivesContainer.TryGetValue(hero, out num) && num > 0)
				{
					this._extraLivesContainer[hero] = num - 1;
					if (this._extraLivesContainer[hero] == 0)
					{
						this._extraLivesContainer.Remove(hero);
						return;
					}
				}
				else
				{
					if (hero == Hero.MainHero && !Hero.IsMainHeroIll)
					{
						Campaign.Current.MainHeroIllDays++;
						Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=2duoimiP}Caught Illness", null).ToString(), new TextObject("{=vo3MqtMn}You are at death's door, wracked by fever, drifting in and out of consciousness. The healers do not believe that you can recover. You should resolve your final affairs and determine a heir for your clan while you still have the strength to speak.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), "", null, null, "event:/ui/notification/quest_fail", 0f, null, null, null), false, false);
						return;
					}
					if (hero != Hero.MainHero)
					{
						KillCharacterAction.ApplyByOldAge(hero, true);
					}
				}
			}
		}

		private void MainHeroHealCheck()
		{
			if (MBRandom.RandomFloat <= 0.05f && Hero.MainHero.IsAlive)
			{
				Campaign.Current.MainHeroIllDays = -1;
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=M5eUjgQl}Cured", null).ToString(), new TextObject("{=T5H3L9Kw}The fever has broken. You are weak but you feel you will recover. You rise from your bed from the first time in days, blinking in the sunlight.", null).ToString(), true, false, new TextObject("{=yQtzabbe}Close", null).ToString(), "", null, null, "event:/ui/notification/quest_finished", 0f, null, null, null), false, false);
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			}
		}

		private void InitializeHeroesYoungerThanHeroComesOfAge()
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				int num = (int)hero.Age;
				if (num < Campaign.Current.Models.AgeModel.HeroComesOfAge && !this._heroesYoungerThanHeroComesOfAge.ContainsKey(hero))
				{
					this._heroesYoungerThanHeroComesOfAge.Add(hero, num);
				}
			}
		}

		private Dictionary<Hero, int> _extraLivesContainer = new Dictionary<Hero, int>();

		private Dictionary<Hero, int> _heroesYoungerThanHeroComesOfAge = new Dictionary<Hero, int>();

		private int _gameStartDay;
	}
}
