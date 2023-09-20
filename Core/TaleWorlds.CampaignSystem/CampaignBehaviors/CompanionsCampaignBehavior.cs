using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class CompanionsCampaignBehavior : CampaignBehaviorBase
	{
		private float _desiredTotalCompanionCount
		{
			get
			{
				return (float)Town.AllTowns.Count * 0.6f;
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.HeroOccupationChangedEvent.AddNonSerializedListener(this, new Action<Hero, Occupation>(this.OnHeroOccupationChanged));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
		}

		private void DailyTick()
		{
			this.TryKillCompanion();
			this.SwapCompanions();
			this.TrySpawnNewCompanion();
		}

		private void WeeklyTick()
		{
			foreach (Hero hero in Hero.DeadOrDisabledHeroes.ToList<Hero>())
			{
				if (hero.IsDead && hero.IsWanderer && hero.DeathDay.ElapsedDaysUntilNow >= 40f)
				{
					Campaign.Current.CampaignObjectManager.UnregisterDeadHero(hero);
				}
			}
		}

		private void RemoveFromAliveCompanions(Hero companion)
		{
			CharacterObject template = companion.Template;
			if (this._aliveCompanionTemplates.Contains(template))
			{
				this._aliveCompanionTemplates.Remove(template);
			}
		}

		private void AddToAliveCompanions(Hero companion)
		{
			CharacterObject template = companion.Template;
			if (this.IsTemplateKnown(template) && !this._aliveCompanionTemplates.Contains(template))
			{
				this._aliveCompanionTemplates.Add(template);
			}
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.RemoveFromAliveCompanions(victim);
			if (victim.IsWanderer && !victim.HasMet && victim.IsDead)
			{
				Campaign.Current.CampaignObjectManager.UnregisterDeadHero(victim);
			}
		}

		private void OnHeroOccupationChanged(Hero hero, Occupation oldOccupation)
		{
			if (oldOccupation == Occupation.Wanderer)
			{
				this.RemoveFromAliveCompanions(hero);
				return;
			}
			if (hero.Occupation == Occupation.Wanderer)
			{
				this.AddToAliveCompanions(hero);
			}
		}

		private void OnHeroCreated(Hero hero, bool showNotification = true)
		{
			if (hero.IsAlive && hero.IsWanderer)
			{
				this.AddToAliveCompanions(hero);
			}
		}

		private void TryKillCompanion()
		{
			if (MBRandom.RandomFloat <= 0.1f)
			{
				CharacterObject randomElementInefficiently = this._aliveCompanionTemplates.GetRandomElementInefficiently<CharacterObject>();
				Hero hero = null;
				foreach (Hero hero2 in Hero.AllAliveHeroes)
				{
					if (hero2.Template == randomElementInefficiently && hero2.IsWanderer)
					{
						hero = hero2;
						break;
					}
				}
				if (hero != null && hero.CompanionOf == null && (hero.CurrentSettlement == null || hero.CurrentSettlement != Hero.MainHero.CurrentSettlement))
				{
					KillCharacterAction.ApplyByRemove(hero, false, true);
				}
			}
		}

		private void TrySpawnNewCompanion()
		{
			if ((float)this._aliveCompanionTemplates.Count < this._desiredTotalCompanionCount)
			{
				Town randomElementWithPredicate = Town.AllTowns.GetRandomElementWithPredicate(delegate(Town x)
				{
					if (x.Settlement != Hero.MainHero.CurrentSettlement && x.Settlement.SiegeEvent == null)
					{
						return x.Settlement.HeroesWithoutParty.AllQ((Hero y) => !y.IsWanderer || y.CompanionOf != null);
					}
					return false;
				});
				Settlement settlement = ((randomElementWithPredicate != null) ? randomElementWithPredicate.Settlement : null);
				if (settlement != null)
				{
					this.CreateCompanionAndAddToSettlement(settlement);
				}
			}
		}

		private void SwapCompanions()
		{
			int num = Town.AllTowns.Count / 2;
			int num2 = MBRandom.RandomInt(Town.AllTowns.Count % 2);
			Town town = Town.AllTowns[num2 + MBRandom.RandomInt(num)];
			Hero hero = town.Settlement.HeroesWithoutParty.Where((Hero x) => x.IsWanderer && x.CompanionOf == null).GetRandomElementInefficiently<Hero>();
			for (int i = 1; i < 2; i++)
			{
				Town town2 = Town.AllTowns[i * num + num2 + MBRandom.RandomInt(num)];
				IEnumerable<Hero> enumerable = town2.Settlement.HeroesWithoutParty.Where((Hero x) => x.IsWanderer && x.CompanionOf == null);
				Hero hero2 = null;
				if (enumerable.Any<Hero>())
				{
					hero2 = enumerable.GetRandomElementInefficiently<Hero>();
					LeaveSettlementAction.ApplyForCharacterOnly(hero2);
				}
				if (hero != null)
				{
					EnterSettlementAction.ApplyForCharacterOnly(hero, town2.Settlement);
				}
				hero = hero2;
			}
			if (hero != null)
			{
				EnterSettlementAction.ApplyForCharacterOnly(hero, town.Settlement);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			this.InitializeCompanionTemplateList();
			List<Town> list = Town.AllTowns.ToListQ<Town>();
			list.Shuffle<Town>();
			int num = 0;
			while ((float)num < this._desiredTotalCompanionCount)
			{
				this.CreateCompanionAndAddToSettlement(list[num].Settlement);
				num++;
			}
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeCompanionTemplateList();
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (hero.IsWanderer)
				{
					this.AddToAliveCompanions(hero);
				}
			}
			foreach (Hero hero2 in Hero.DeadOrDisabledHeroes)
			{
				if (hero2.IsAlive && hero2.IsWanderer)
				{
					this.AddToAliveCompanions(hero2);
				}
			}
		}

		private void AdjustEquipment(Hero hero)
		{
			this.AdjustEquipmentImp(hero.BattleEquipment);
			this.AdjustEquipmentImp(hero.CivilianEquipment);
		}

		private void AdjustEquipmentImp(Equipment equipment)
		{
			ItemModifier @object = MBObjectManager.Instance.GetObject<ItemModifier>("companion_armor");
			ItemModifier object2 = MBObjectManager.Instance.GetObject<ItemModifier>("companion_weapon");
			ItemModifier object3 = MBObjectManager.Instance.GetObject<ItemModifier>("companion_horse");
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				EquipmentElement equipmentElement = equipment[equipmentIndex];
				if (equipmentElement.Item != null)
				{
					if (equipmentElement.Item.ArmorComponent != null)
					{
						equipment[equipmentIndex] = new EquipmentElement(equipmentElement.Item, @object, null, false);
					}
					else if (equipmentElement.Item.HorseComponent != null)
					{
						equipment[equipmentIndex] = new EquipmentElement(equipmentElement.Item, object3, null, false);
					}
					else if (equipmentElement.Item.WeaponComponent != null)
					{
						equipment[equipmentIndex] = new EquipmentElement(equipmentElement.Item, object2, null, false);
					}
				}
			}
		}

		private void InitializeCompanionTemplateList()
		{
			foreach (CultureObject cultureObject in MBObjectManager.Instance.GetObjectTypeList<CultureObject>())
			{
				foreach (CharacterObject characterObject in cultureObject.NotableAndWandererTemplates)
				{
					if (characterObject.Occupation == Occupation.Wanderer)
					{
						this._companionsOfTemplates[this.GetTemplateTypeOfCompanion(characterObject)].Add(characterObject);
					}
				}
			}
		}

		private CompanionsCampaignBehavior.CompanionTemplateType GetTemplateTypeOfCompanion(CharacterObject character)
		{
			CompanionsCampaignBehavior.CompanionTemplateType companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Combat;
			int num = 20;
			foreach (SkillObject skillObject in Skills.All)
			{
				int skillValue = character.GetSkillValue(skillObject);
				if (skillValue > num)
				{
					CompanionsCampaignBehavior.CompanionTemplateType templateTypeForSkill = this.GetTemplateTypeForSkill(skillObject);
					if (templateTypeForSkill != CompanionsCampaignBehavior.CompanionTemplateType.Combat)
					{
						num = skillValue;
						companionTemplateType = templateTypeForSkill;
					}
				}
			}
			foreach (Tuple<SkillObject, int> tuple in Campaign.Current.Models.CharacterDevelopmentModel.GetSkillsDerivedFromTraits(null, character, false))
			{
				int item = tuple.Item2;
				if (item > num)
				{
					CompanionsCampaignBehavior.CompanionTemplateType templateTypeForSkill2 = this.GetTemplateTypeForSkill(tuple.Item1);
					if (templateTypeForSkill2 != CompanionsCampaignBehavior.CompanionTemplateType.Combat)
					{
						num = item;
						companionTemplateType = templateTypeForSkill2;
					}
				}
			}
			return companionTemplateType;
		}

		private void CreateCompanionAndAddToSettlement(Settlement settlement)
		{
			CharacterObject companionTemplate = this.GetCompanionTemplateToSpawn();
			if (companionTemplate != null)
			{
				Town randomElementWithPredicate = Town.AllTowns.GetRandomElementWithPredicate((Town x) => x.Culture == companionTemplate.Culture);
				Settlement settlement2 = ((randomElementWithPredicate != null) ? randomElementWithPredicate.Settlement : null);
				if (settlement2 == null)
				{
					settlement2 = Town.AllTowns.GetRandomElement<Town>().Settlement;
				}
				Hero hero = HeroCreator.CreateSpecialHero(companionTemplate, settlement2, null, null, Campaign.Current.Models.AgeModel.HeroComesOfAge + 5 + MBRandom.RandomInt(27));
				this.AdjustEquipment(hero);
				hero.ChangeState(Hero.CharacterStates.Active);
				EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
			}
		}

		private CompanionsCampaignBehavior.CompanionTemplateType GetCompanionTemplateTypeToSpawn()
		{
			CompanionsCampaignBehavior.CompanionTemplateType companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Combat;
			float num = -1f;
			foreach (KeyValuePair<CompanionsCampaignBehavior.CompanionTemplateType, List<CharacterObject>> keyValuePair in this._companionsOfTemplates)
			{
				float templateTypeScore = this.GetTemplateTypeScore(keyValuePair.Key);
				if (templateTypeScore > 0f)
				{
					int num2 = 0;
					foreach (CharacterObject characterObject in keyValuePair.Value)
					{
						if (this._aliveCompanionTemplates.Contains(characterObject))
						{
							num2++;
						}
					}
					float num3 = (float)num2 / this._desiredTotalCompanionCount;
					float num4 = (templateTypeScore - num3) / templateTypeScore;
					if (num2 < keyValuePair.Value.Count && num4 > num)
					{
						num = num4;
						companionTemplateType = keyValuePair.Key;
					}
				}
			}
			return companionTemplateType;
		}

		private CharacterObject GetCompanionTemplateToSpawn()
		{
			List<CharacterObject> list = this._companionsOfTemplates[this.GetCompanionTemplateTypeToSpawn()];
			list.Shuffle<CharacterObject>();
			CharacterObject characterObject = null;
			foreach (CharacterObject characterObject2 in list)
			{
				if (!this._aliveCompanionTemplates.Contains(characterObject2))
				{
					characterObject = characterObject2;
					break;
				}
			}
			return characterObject;
		}

		private float GetTemplateTypeScore(CompanionsCampaignBehavior.CompanionTemplateType templateType)
		{
			switch (templateType)
			{
			case CompanionsCampaignBehavior.CompanionTemplateType.Engineering:
				return 0.05882353f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Tactics:
				return 0.11764706f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Leadership:
				return 0.0882353f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Steward:
				return 0.0882353f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Trade:
				return 0.0882353f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Roguery:
				return 0.11764706f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Medicine:
				return 0.0882353f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Smithing:
				return 0.05882353f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Scouting:
				return 0.14705883f;
			case CompanionsCampaignBehavior.CompanionTemplateType.Combat:
				return 0.14705883f;
			default:
				return 0f;
			}
		}

		private bool IsTemplateKnown(CharacterObject companionTemplate)
		{
			foreach (KeyValuePair<CompanionsCampaignBehavior.CompanionTemplateType, List<CharacterObject>> keyValuePair in this._companionsOfTemplates)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					if (companionTemplate == keyValuePair.Value[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		private CompanionsCampaignBehavior.CompanionTemplateType GetTemplateTypeForSkill(SkillObject skill)
		{
			CompanionsCampaignBehavior.CompanionTemplateType companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Combat;
			if (skill == DefaultSkills.Engineering)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Engineering;
			}
			else if (skill == DefaultSkills.Tactics)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Tactics;
			}
			else if (skill == DefaultSkills.Leadership)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Leadership;
			}
			else if (skill == DefaultSkills.Steward)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Steward;
			}
			else if (skill == DefaultSkills.Trade)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Trade;
			}
			else if (skill == DefaultSkills.Roguery)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Roguery;
			}
			else if (skill == DefaultSkills.Medicine)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Medicine;
			}
			else if (skill == DefaultSkills.Crafting)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Smithing;
			}
			else if (skill == DefaultSkills.Scouting)
			{
				companionTemplateType = CompanionsCampaignBehavior.CompanionTemplateType.Scouting;
			}
			return companionTemplateType;
		}

		private const int CompanionMoveRandomIndex = 2;

		private const float DesiredCompanionPerTown = 0.6f;

		private const float KillChance = 0.1f;

		private const int SkillThresholdValue = 20;

		private const int RemoveWandererAfterDays = 40;

		private IReadOnlyDictionary<CompanionsCampaignBehavior.CompanionTemplateType, List<CharacterObject>> _companionsOfTemplates = new Dictionary<CompanionsCampaignBehavior.CompanionTemplateType, List<CharacterObject>>
		{
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Engineering,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Tactics,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Leadership,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Steward,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Trade,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Roguery,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Medicine,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Smithing,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Scouting,
				new List<CharacterObject>()
			},
			{
				CompanionsCampaignBehavior.CompanionTemplateType.Combat,
				new List<CharacterObject>()
			}
		};

		private HashSet<CharacterObject> _aliveCompanionTemplates = new HashSet<CharacterObject>();

		private const float EngineerScore = 2f;

		private const float TacticsScore = 4f;

		private const float LeadershipScore = 3f;

		private const float StewardScore = 3f;

		private const float TradeScore = 3f;

		private const float RogueryScore = 4f;

		private const float MedicineScore = 3f;

		private const float SmithingScore = 2f;

		private const float ScoutingScore = 5f;

		private const float CombatScore = 5f;

		private const float AllScore = 34f;

		private enum CompanionTemplateType
		{
			Engineering,
			Tactics,
			Leadership,
			Steward,
			Trade,
			Roguery,
			Medicine,
			Smithing,
			Scouting,
			Combat
		}
	}
}
